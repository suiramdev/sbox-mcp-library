using System.Text.Json;
using System.Text.Json.Serialization;

namespace SandboxModelContextProtocol.Editor.Connection.Models;

public class CallResponse
{
	[JsonPropertyName( "id" )]
	public required string Id { get; init; }

	[JsonPropertyName( "type" )]
	public virtual string? Type { get; init; }

	[JsonPropertyName( "name" )]
	public required string Name { get; init; }

	[JsonPropertyName( "content" )]
	public List<JsonElement> Content { get; init; } = [];

	[JsonPropertyName( "isError" )]
	public bool IsError { get; init; }
}
