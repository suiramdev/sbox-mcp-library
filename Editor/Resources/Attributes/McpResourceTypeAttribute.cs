using System;

namespace SandboxModelContextProtocol.Editor.Resources.Attributes;

/// <summary>
/// Attribute that marks a class as an MCP resource type.
/// </summary>
[AttributeUsage( AttributeTargets.Class )]
public class McpResourceTypeAttribute : Attribute
{
	/// <summary>
	/// Gets the resource type name. If not specified, the class name will be used.
	/// </summary>
	public string? ResourceTypeName { get; }

	/// <summary>
	/// Gets the resource type description.
	/// </summary>
	public string? ResourceTypeDescription { get; }

	/// <summary>
	/// Initializes a new instance of the McpResourceTypeAttribute with automatic resource type name from class name.
	/// </summary>
	public McpResourceTypeAttribute()
	{
		ResourceTypeName = null;
		ResourceTypeDescription = null;
	}

	/// <summary>
	/// Initializes a new instance of the McpResourceTypeAttribute with a custom resource type name.
	/// </summary>
	/// <param name="resourceTypeName">The custom resource type name to use instead of the class name</param>
	public McpResourceTypeAttribute( string resourceTypeName )
	{
		ResourceTypeName = resourceTypeName;
		ResourceTypeDescription = null;
	}

	/// <summary>
	/// Initializes a new instance of the McpResourceTypeAttribute with a custom resource type name and description.
	/// </summary>
	/// <param name="resourceTypeName">The custom resource type name to use instead of the class name</param>
	/// <param name="resourceTypeDescription">The description of the resource type</param>
	public McpResourceTypeAttribute( string resourceTypeName, string resourceTypeDescription )
	{
		ResourceTypeName = resourceTypeName;
		ResourceTypeDescription = resourceTypeDescription;
	}

	/// <summary>
	/// Gets the effective resource type name, using the class name if no custom name was specified.
	/// </summary>
	/// <param name="className">The name of the class this attribute is applied to</param>
	/// <returns>The resource type name to use</returns>
	public string GetResourceTypeName( string className )
	{
		return ResourceTypeName ?? className;
	}
}
