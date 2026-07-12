using System;

namespace SandboxModelContextProtocol.Editor.Tools.Attributes;

/// <summary>
/// Attribute that marks a class as an MCP tool type.
/// </summary>
[AttributeUsage( AttributeTargets.Class )]
public class McpToolTypeAttribute : Attribute
{
	/// <summary>
	/// Gets the tool type name. If not specified, the class name will be used.
	/// </summary>
	public string? ToolTypeName { get; }

	/// <summary>
	/// Gets the tool type description.
	/// </summary>
	public string? ToolTypeDescription { get; }

	/// <summary>
	/// Initializes a new instance of the McpToolTypeAttribute with automatic tool type name from class name.
	/// </summary>
	public McpToolTypeAttribute()
	{
		ToolTypeName = null;
		ToolTypeDescription = null;
	}

	/// <summary>
	/// Initializes a new instance of the McpToolTypeAttribute with a custom tool type name.
	/// </summary>
	/// <param name="toolTypeName">The custom tool type name to use instead of the class name</param>
	public McpToolTypeAttribute( string toolTypeName )
	{
		ToolTypeName = toolTypeName;
		ToolTypeDescription = null;
	}

	/// <summary>
	/// Initializes a new instance of the McpToolTypeAttribute with a custom tool type name and description.
	/// </summary>
	/// <param name="toolTypeName">The custom tool type name to use instead of the class name</param>
	/// <param name="toolTypeDescription">The description of the tool type</param>
	public McpToolTypeAttribute( string toolTypeName, string toolTypeDescription )
	{
		ToolTypeName = toolTypeName;
		ToolTypeDescription = toolTypeDescription;
	}

	/// <summary>
	/// Gets the effective tool type name, using the class name if no custom name was specified.
	/// </summary>
	/// <param name="className">The name of the class this attribute is applied to</param>
	/// <returns>The tool type name to use</returns>
	public string GetToolTypeName( string className )
	{
		return ToolTypeName ?? className;
	}
}
