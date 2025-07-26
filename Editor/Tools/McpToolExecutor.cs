using System;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using SandboxModelContextProtocol.Editor.Commands.Attributes;
using SandboxModelContextProtocol.Editor.Tools.Models;

namespace SandboxModelContextProtocol.Editor.Tools;

public static class McpToolExecutor
{
	private static readonly Dictionary<string, MethodInfo> _toolMethods = [];
	private static bool _initialized = false;

	static McpToolExecutor()
	{
		// Initialize tools on startup
		InitializeTools();
	}

	[EditorEvent.Hotload]
	internal static void OnHotload()
	{
		// Reinitialize tools when hotloading
		InitializeTools();
	}

	public static async Task<CallEditorToolResponse> CallEditorTool( CallEditorToolRequest request )
	{
		try
		{
			// Ensure tools are initialized
			if ( !_initialized )
			{
				Log.Info( "Initializing tools" );
				InitializeTools();
			}

			Log.Info( $"Calling tool: {request.Name}" );

			// Find the tool method
			if ( !_toolMethods.TryGetValue( request.Name, out MethodInfo? method ) || method is null )
			{
				Log.Warning( $"Tool not found: {request.Name}" );
				return new CallEditorToolResponse()
				{
					Id = request.Id,
					Name = request.Name,
					Content = [JsonSerializer.SerializeToElement( $"Tool '{request.Name}' not found" )],
					IsError = true,
				};
			}

			var result = await ExecuteOnMainThread( method, request );

			return new CallEditorToolResponse()
			{
				Id = request.Id,
				Name = request.Name,
				Content = [JsonSerializer.SerializeToElement( result )],
				IsError = false,
			};
		}
		catch ( Exception ex )
		{
			Log.Warning( $"Error executing tool '{request.Name}': {ex.InnerException?.Message ?? ex.Message}" );
			return new CallEditorToolResponse()
			{
				Id = request.Id,
				Name = request.Name,
				Content = [JsonSerializer.SerializeToElement( $"Error executing tool '{request.Name}': {ex.InnerException?.Message ?? ex.Message}" )],
				IsError = true,
			};
		}
	}

	private static async Task<object?> ExecuteOnMainThread( MethodInfo method, CallEditorToolRequest request )
	{
		var tcs = new TaskCompletionSource<object?>();

		// Queue the method execution on the main thread
		MainThread.Queue( () =>
		{
			try
			{
				// Prepare arguments for method invocation
				object?[] parameters = PrepareMethodParameters( method, request.Arguments );

				Log.Info( $"Invoking method: {method.Name} with parameters: {JsonSerializer.Serialize( parameters )}" );

				// Invoke the method
				object? result = method.Invoke( null, parameters );

				// Handle async methods
				if ( result is Task task )
				{
					// For async methods, we need to wait for completion and get the result
					task.ContinueWith( t =>
					{
						try
						{
							if ( t.IsFaulted )
							{
								tcs.SetException( t.Exception?.InnerException ?? new Exception( "Unknown error in async method" ) );
							}
							else if ( t.IsCanceled )
							{
								tcs.SetCanceled();
							}
							else
							{
								// Get the result from Task<T>
								if ( t.GetType().IsGenericType )
								{
									PropertyInfo? resultProperty = t.GetType().GetProperty( "Result" );
									var taskResult = resultProperty?.GetValue( t );
									tcs.SetResult( taskResult );
								}
								else
								{
									tcs.SetResult( null ); // Task without return value
								}
							}
						}
						catch ( Exception ex )
						{
							tcs.SetException( ex );
						}
					} );
				}
				else
				{
					// Synchronous method - set result immediately
					tcs.SetResult( result );
				}
			}
			catch ( Exception ex )
			{
				tcs.SetException( ex );
			}
		} );

		return await tcs.Task;
	}

	private static void InitializeTools()
	{
		_toolMethods.Clear();

		try
		{
			// Get all assemblies in the current domain
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

			foreach ( Assembly assembly in assemblies )
			{
				try
				{
					// Find all types with McpEditorToolTypeAttribute
					Type[] toolTypes = [.. assembly.GetTypes().Where( t => t.GetCustomAttribute<McpEditorToolTypeAttribute>() != null )];

					foreach ( Type toolType in toolTypes )
					{
						// Find all methods with McpEditorToolAttribute
						MethodInfo[] toolMethods = [.. toolType.GetMethods( BindingFlags.Public | BindingFlags.Static ).Where( m => m.GetCustomAttribute<McpEditorToolAttribute>() != null )];

						Log.Info( $"Tool: {toolType.Name} {toolMethods.Length}" );

						foreach ( MethodInfo method in toolMethods )
						{
							McpEditorToolAttribute? attribute = method.GetCustomAttribute<McpEditorToolAttribute>();
							if ( attribute != null )
							{
								string commandName = attribute.GetCommandName( method.Name );
								_toolMethods[commandName] = method;
							}
						}
					}
				}
				catch ( Exception ex )
				{
					// Skip assemblies that can't be reflected (e.g., native assemblies)
					Log.Error( $"Warning: Could not reflect assembly {assembly.FullName}: {ex.Message}" );
				}
			}

			_initialized = true;
		}
		catch ( Exception ex )
		{
			Log.Error( $"Error initializing MCP tools: {ex.Message}" );
			_initialized = false;
		}
	}

	private static object?[] PrepareMethodParameters( MethodInfo method, IReadOnlyDictionary<string, JsonElement>? arguments )
	{
		ParameterInfo[] parameters = method.GetParameters();

		object?[] parameterValues = new object?[parameters.Length];

		for ( int i = 0; i < parameters.Length; i++ )
		{
			ParameterInfo parameter = parameters[i];
			Log.Info( "test 2" );

			if ( arguments != null && arguments.TryGetValue( parameter.Name ?? string.Empty, out JsonElement argumentValue ) )
			{
				try
				{
					// Deserialize the JSON element to the parameter type
					parameterValues[i] = JsonSerializer.Deserialize( argumentValue, parameter.ParameterType );
				}
				catch ( Exception )
				{
					// If deserialization fails, try to convert the raw value
					parameterValues[i] = ConvertJsonElement( argumentValue, parameter.ParameterType );
				}
			}
			else if ( parameter.HasDefaultValue )
			{
				parameterValues[i] = parameter.DefaultValue;
			}
			else if ( parameter.ParameterType.IsValueType )
			{
				parameterValues[i] = Activator.CreateInstance( parameter.ParameterType );
			}
			else
			{
				parameterValues[i] = null;
			}
		}

		return parameterValues;
	}

	private static object? ConvertJsonElement( JsonElement element, Type targetType )
	{
		try
		{
			return element.ValueKind switch
			{
				JsonValueKind.String => element.GetString(),
				JsonValueKind.Number when targetType == typeof( int ) || targetType == typeof( int? ) => element.GetInt32(),
				JsonValueKind.Number when targetType == typeof( long ) || targetType == typeof( long? ) => element.GetInt64(),
				JsonValueKind.Number when targetType == typeof( float ) || targetType == typeof( float? ) => element.GetSingle(),
				JsonValueKind.Number when targetType == typeof( double ) || targetType == typeof( double? ) => element.GetDouble(),
				JsonValueKind.Number when targetType == typeof( decimal ) || targetType == typeof( decimal? ) => element.GetDecimal(),
				JsonValueKind.True or JsonValueKind.False when targetType == typeof( bool ) || targetType == typeof( bool? ) => element.GetBoolean(),
				JsonValueKind.Null => null,
				_ => element.GetString()
			};
		}
		catch
		{
			return targetType.IsValueType ? Activator.CreateInstance( targetType ) : null;
		}
	}
}
