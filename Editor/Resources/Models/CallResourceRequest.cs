using System.Text.Json;
using System.Text.Json.Serialization;

namespace SandboxModelContextProtocol.Editor.Resources.Models;

public class CallResourceRequest
{
	[JsonPropertyName( "id" )]
	public required string Id { get; init; }

	[JsonPropertyName( "name" )]
	public required string Name { get; init; }

	[JsonPropertyName( "arguments" )]
	public IReadOnlyDictionary<string, JsonElement>? Arguments { get; init; }
}
