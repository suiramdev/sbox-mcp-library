using System;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using SandboxModelContextProtocol.Editor.Resources.Attributes;
using SandboxModelContextProtocol.Editor.Resources.Models;

namespace SandboxModelContextProtocol.Editor.Resources;

public static class McpResourceExecutor
{
	private static readonly Dictionary<string, MethodInfo> _resourceMethods = [];
	private static bool _initialized = false;

	static McpResourceExecutor()
	{
		// Initialize resources on startup
		InitializeResources();
	}

	[EditorEvent.Hotload]
	internal static void OnHotload()
	{
		// Reinitialize resources when hotloading
		InitializeResources();
	}

	public static async Task<CallResourceResponse> CallResource( CallResourceRequest request )
	{
		try
		{
			// Ensure resources are initialized
			if ( !_initialized )
			{
				Log.Info( "Initializing resources" );
				InitializeResources();
			}

			Log.Info( $"Calling resource: {request.Name}" );

			// Find the resource method
			if ( !_resourceMethods.TryGetValue( request.Name, out MethodInfo? method ) || method is null )
			{
				Log.Warning( $"Resource not found: {request.Name}" );
				return new CallResourceResponse()
				{
					Id = request.Id,
					Name = request.Name,
					Content = [JsonSerializer.SerializeToElement( $"Resource '{request.Name}' not found" )],
					IsError = true,
				};
			}

			var result = await ExecuteOnMainThread( method, request );

			return new CallResourceResponse()
			{
				Id = request.Id,
				Name = request.Name,
				Content = [JsonSerializer.SerializeToElement( result )],
				IsError = false,
			};
		}
		catch ( Exception ex )
		{
			Log.Warning( $"Error executing resource '{request.Name}': {ex.InnerException?.Message ?? ex.Message}" );
			return new CallResourceResponse()
			{
				Id = request.Id,
				Name = request.Name,
				Content = [JsonSerializer.SerializeToElement( $"Error executing resource '{request.Name}': {ex.InnerException?.Message ?? ex.Message}" )],
				IsError = true,
			};
		}
	}

	private static async Task<object?> ExecuteOnMainThread( MethodInfo method, CallResourceRequest request )
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

	private static void InitializeResources()
	{
		_resourceMethods.Clear();

		try
		{
			// Get all assemblies in the current domain
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

			foreach ( Assembly assembly in assemblies )
			{
				try
				{
					// Find all types with McpResourceTypeAttribute
					Type[] resourceTypes = [.. assembly.GetTypes().Where( t => t.GetCustomAttribute<McpResourceTypeAttribute>() != null )];

					foreach ( Type resourceType in resourceTypes )
					{
						// Find all methods with McpResourceAttribute
						MethodInfo[] resourceMethods = [.. resourceType.GetMethods( BindingFlags.Public | BindingFlags.Static ).Where( m => m.GetCustomAttribute<McpResourceAttribute>() != null )];

						Log.Info( $"Resource: {resourceType.Name} {resourceMethods.Length}" );

						foreach ( MethodInfo method in resourceMethods )
						{
							McpResourceAttribute? attribute = method.GetCustomAttribute<McpResourceAttribute>();
							if ( attribute != null )
							{
								string resourceName = attribute.GetResourceName( method.Name );
								_resourceMethods[resourceName] = method;
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
			Log.Error( $"Error initializing MCP resources: {ex.Message}" );
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
