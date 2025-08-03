using System.Text.Json.Nodes;
using SandboxModelContextProtocol.Editor.Resources.Attributes;

namespace SandboxModelContextProtocol.Editor.Resources;

[McpResourceType()]
public class GameObjectResource
{
	[McpResource()]
	public static JsonObject GetGameObject( string id )
	{
		return new JsonObject();
	}
}
