
using System;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using SandboxModelContextProtocol.Editor.Tools.Attributes;

namespace SandboxModelContextProtocol.Editor.Tools;

[McpToolType]
public class EditorSessionTool
{
	[McpTool]
	public static void SaveAllEditorSessions()
	{
		EditorScene.SaveAllSessions();
	}

	[McpTool]
	public static void SaveActiveEditorSession()
	{
		EditorScene.SaveSession();
	}

	[McpTool]
	public static JsonArray GetAllEditorSessions()
	{
		return new JsonArray( SceneEditorSession.All.Select( s => s.Scene.Serialize() ).ToArray() );
	}

	[McpTool]
	public static JsonObject GetActiveEditorSession()
	{
		var activeSession = SceneEditorSession.Active;
		if ( activeSession?.Scene == null )
		{
			throw new InvalidOperationException( "No active editor session found" );
		}

		return activeSession.Scene.Serialize();
	}
}
