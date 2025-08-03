using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SandboxModelContextProtocol.Editor.Connection.Models;

public class CallRequest
{
	[JsonPropertyName( "id" )]
	public string Id { get; init; } = Guid.NewGuid().ToString();

	[JsonPropertyName( "type" )]
	public virtual string? Type { get; init; }

	[JsonPropertyName( "name" )]
	public required string Name { get; init; }

	[JsonPropertyName( "arguments" )]
	public IReadOnlyDictionary<string, JsonElement>? Arguments { get; init; }
}
