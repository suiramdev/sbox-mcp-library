using System;

namespace SandboxModelContextProtocol.Editor.Resources.Attributes;

/// <summary>
/// Attribute that marks a method as an MCP resource.
/// The method name is automatically used as the resource name unless overridden.
/// </summary>
[AttributeUsage( AttributeTargets.Method )]
public class McpResourceAttribute : Attribute
{
	/// <summary>
	/// Gets the resource name. If not specified, the method name will be used.
	/// </summary>
	public string? ResourceName { get; }

	/// <summary>
	/// Gets the resource description.
	/// </summary>
	public string? Description { get; }

	/// <summary>
	/// Initializes a new instance of the McpResourceAttribute with automatic resource name from method name.
	/// </summary>
	public McpResourceAttribute()
	{
		ResourceName = null;
		Description = null;
	}

	/// <summary>
	/// Initializes a new instance of the McpResourceAttribute with a custom resource name.
	/// </summary>
	/// <param name="resourceName">The custom resource name to use instead of the method name</param>
	public McpResourceAttribute( string resourceName )
	{
		ResourceName = resourceName;
		Description = null;
	}

	/// <summary>
	/// Initializes a new instance of the McpResourceAttribute with a custom resource name and description.
	/// </summary>
	/// <param name="resourceName">The custom resource name to use instead of the method name</param>
	/// <param name="description">The description of the resource</param>
	public McpResourceAttribute( string resourceName, string description )
	{
		ResourceName = resourceName;
		Description = description;
	}

	/// <summary>
	/// Gets the effective resource name, using the method name if no custom name was specified.
	/// </summary>
	/// <param name="methodName">The name of the method this attribute is applied to</param>
	/// <returns>The resource name to use</returns>
	public string GetResourceName( string methodName )
	{
		return ResourceName ?? methodName;
	}
}
