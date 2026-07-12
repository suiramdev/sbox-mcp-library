using System;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using SandboxModelContextProtocol.Editor.Tools.Attributes;

namespace SandboxModelContextProtocol.Editor.Tools;

[McpToolType]
public class EditorSceneTool
{
	[McpTool]
	public static JsonObject GetActiveEditorScene()
	{
		Scene? scene = SceneEditorSession.Active.Scene;
		if ( scene == null )
		{
			return new JsonObject( null );
		}

		return scene.Serialize();
	}

	[McpTool]
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
}
