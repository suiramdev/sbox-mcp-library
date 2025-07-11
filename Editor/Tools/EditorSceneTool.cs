using System;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Editor;
using Sandbox;
using SandboxModelContextProtocol.Editor.Commands.Attributes;

namespace SandboxModelContextProtocol.Editor.Tools;

[McpEditorToolType]
public class EditorSceneTool
{
	[McpEditorTool]
	public static JsonObject GetActiveEditorScene()
	{
		Scene? scene = SceneEditorSession.Active.Scene;
		if ( scene == null )
		{
			return new JsonObject( null );
		}

		return scene.Serialize();
	}

	[McpEditorTool]
	public static async Task LoadEditorSceneFromPath( string path )
	{
		// Validate input
		if ( string.IsNullOrWhiteSpace( path ) )
		{
			throw new ArgumentException( "Scene path cannot be null or empty", nameof( path ) );
		}

		// Try multiple path formats
		string[] pathVariants = {
			path,
			path.Replace( '\\', '/' ),
			$"Assets/scenes/{System.IO.Path.GetFileName( path )}",
			System.IO.Path.GetFileName( path ).Replace( ".scene", "" )
		};
		
		SceneFile? sceneFile = null;
		foreach ( string pathVariant in pathVariants )
		{
			try
			{
				if ( ResourceLibrary.TryGet( pathVariant, out sceneFile ) && sceneFile != null )
					break;
			}
			catch ( Exception ex )
			{
				Log.Warning( $"Failed to load scene from path '{pathVariant}': {ex.Message}" );
			}
		}
		
		if ( sceneFile == null )
		{
			throw new InvalidOperationException( $"Scene file not found. Tried paths: {string.Join( ", ", pathVariants )}" );
		}

		try
		{
			await EditorScene.LoadFromScene( sceneFile );
		}
		catch ( Exception ex )
		{
			throw new InvalidOperationException( $"Failed to load scene from '{path}': {ex.Message}", ex );
		}
	}

	[McpEditorTool]
	public static void SaveAllEditorSessions()
	{
		EditorScene.SaveAllSessions();
	}

	[McpEditorTool]
	public static void SaveActiveEditorSession()
	{
		EditorScene.SaveSession();
	}

	[McpEditorTool]
	public static JsonObject GetAllEditorSessions()
	{
		var sessions = new JsonArray();
		
		foreach ( var session in SceneEditorSession.All )
		{
			try
			{
				sessions.Add( new JsonObject
				{
					["id"] = session.Scene?.Id.ToString(),
					["name"] = session.Scene?.Name ?? "Unnamed Scene",
					["isActive"] = session == SceneEditorSession.Active
					// Note: IsDirty property not available on SceneEditorSession
				} );
			}
			catch ( Exception ex )
			{
				Log.Warning( $"Failed to serialize editor session: {ex.Message}" );
			}
		}
		
		return new JsonObject
		{
			["sessions"] = sessions,
			["count"] = sessions.Count
		};
	}

	[McpEditorTool]
	public static JsonObject GetActiveEditorSession()
	{
		var activeSession = SceneEditorSession.Active;
		if ( activeSession?.Scene == null )
		{
			return new JsonObject { ["error"] = "No active editor session found" };
		}
		
		return new JsonObject
		{
			["id"] = activeSession.Scene.Id.ToString(),
			["name"] = activeSession.Scene.Name ?? "Unnamed Scene",
			["isActive"] = true,
			// Note: IsDirty property not available on SceneEditorSession
			["scene"] = activeSession.Scene.Serialize()
		};
	}
}
