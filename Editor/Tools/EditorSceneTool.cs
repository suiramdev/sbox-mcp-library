using System;
using System.Linq;
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
				if ( ResourceLibrary.TryGet( pathVariant, out sceneFile ) )
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
	public static JsonArray GetAllEditorSessions()
	{
		return new JsonArray( SceneEditorSession.All.Select( s => s.Scene.Serialize() ).ToArray() );
	}

	[McpEditorTool]
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
