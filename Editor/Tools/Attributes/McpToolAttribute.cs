using System;

namespace SandboxModelContextProtocol.Editor.Tools.Attributes;

/// <summary>
/// Attribute that marks a method as an MCP tool command.
/// The method name is automatically used as the command name unless overridden.
/// </summary>
[AttributeUsage( AttributeTargets.Method )]
public class McpToolAttribute : Attribute
{
	/// <summary>
	/// Gets the tool name. If not specified, the method name will be used.
	/// </summary>
	public string? ToolName { get; }

	/// <summary>
	/// Gets the tool description.
	/// </summary>
	public string? Description { get; }

	/// <summary>
	/// Initializes a new instance of the McpToolAttribute with automatic tool name from method name.
	/// </summary>
	public McpToolAttribute()
	{
		ToolName = null;
		Description = null;
	}

	/// <summary>
	/// Initializes a new instance of the McpToolAttribute with a custom tool name.
	/// </summary>
	/// <param name="toolName">The custom tool name to use instead of the method name</param>
	public McpToolAttribute( string toolName )
	{
		ToolName = toolName;
		Description = null;
	}

	/// <summary>
	/// Initializes a new instance of the McpToolAttribute with a custom tool name and description.
	/// </summary>
	/// <param name="toolName">The custom tool name to use instead of the method name</param>
	/// <param name="description">The description of the tool</param>
	public McpToolAttribute( string toolName, string description )
	{
		ToolName = toolName;
		Description = description;
	}

	/// <summary>
	/// Gets the effective tool name, using the method name if no custom name was specified.
	/// </summary>
	/// <param name="methodName">The name of the method this attribute is applied to</param>
	/// <returns>The tool name to use</returns>
	public string GetToolName( string methodName )
	{
		return ToolName ?? methodName;
	}
}
