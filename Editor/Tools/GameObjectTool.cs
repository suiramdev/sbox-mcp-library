using System;
using System.Text.Json.Nodes;
using SandboxModelContextProtocol.Editor.Commands.Attributes;

namespace SandboxModelContextProtocol.Editor.Tools;

[McpEditorToolType]
public class GameObjectTool
{
	[McpEditorTool]
	public static JsonObject GetGameObjectByName( string name, string? sceneId = null )
	{
		var scene = GetSceneOrActive( sceneId );

		var gameObject = scene.GetAllObjects( false ).FirstOrDefault( go => go.Name == name );
		if ( gameObject == null )
		{
			return new JsonObject( null );
		}

		return gameObject.Serialize();
	}

	[McpEditorTool]
	public static JsonObject GetGameObjectById( string id, string? sceneId = null )
	{
		var scene = GetSceneOrActive( sceneId );

		var gameObject = scene.GetAllObjects( false ).FirstOrDefault( go => go.Id == new Guid( id ) );
		if ( gameObject == null )
		{
			return new JsonObject( null );
		}

		return gameObject.Serialize();
	}

	[McpEditorTool]
	public static JsonArray GetAllGameObjects( string? sceneId = null )
	{
		var scene = GetSceneOrActive( sceneId );
		var gameObjects = scene.GetAllObjects( false );

		return new JsonArray( gameObjects.Select( go => go.Serialize() ).ToArray() );
	}

	[McpEditorTool]
	public static JsonObject CreateGameObject( string name, string sceneId, string? parentId = null )
	{
		var scene = GetSceneOrActive( sceneId );

		GameObject? parent = null;
		if ( parentId != null )
		{
			if ( !Guid.TryParse( parentId, out Guid parsedParentId ) )
			{
				throw new InvalidOperationException( $"Invalid parent ID format: {parentId}" );
			}
			parent = GetGameObjectById( parsedParentId, scene );
		}

		var gameObject = scene.CreateObject();
		gameObject.Name = name;
		gameObject.SetParent( parent );

		return gameObject.Serialize();
	}

	[McpEditorTool]
	public static JsonObject DuplicateGameObject( string id, string? sceneId = null, string? parentId = null )
	{
		var scene = GetSceneOrActive( sceneId );

		var gameObject = GetGameObjectById( new Guid( id ), scene );

		GameObject? parent = null;
		if ( parentId != null )
		{
			// Check if parent exists
			parent = GetGameObjectById( new Guid( parentId ), scene );
		}

		var duplicate = gameObject.Clone();
		duplicate.Name = gameObject.Name + " (Copy)";
		duplicate.SetParent( parent );

		return duplicate.Serialize();
	}

	[McpEditorTool]
	public static bool DestroyGameObject( string id, string? sceneId = null )
	{
		var scene = GetSceneOrActive( sceneId );

		var gameObject = GetGameObjectById( new Guid( id ), scene );

		gameObject.Destroy();

		return true;
	}

	// Transform Commands
	[McpEditorTool]
	public static JsonObject SetGameObjectWorldPosition( string id, float x, float y, float z, string? sceneId = null )
	{
		var scene = GetSceneOrActive( sceneId );

		var gameObject = GetGameObjectById( new Guid( id ), scene );

		gameObject.WorldPosition = new Vector3( x, y, z );

		return gameObject.Serialize();
	}

	[McpEditorTool]
	public static JsonObject SetGameObjectWorldRotation( string id, float x, float y, float z, float w, string? sceneId = null )
	{
		var scene = GetSceneOrActive( sceneId );

		var gameObject = GetGameObjectById( new Guid( id ), scene );

		gameObject.WorldRotation = new Rotation( x, y, z, w );

		return gameObject.Serialize();
	}

	[McpEditorTool]
	public static JsonObject SetGameObjectWorldScale( string id, float x, float y, float z, string? sceneId = null )
	{
		var scene = GetSceneOrActive( sceneId );

		var gameObject = GetGameObjectById( new Guid( id ), scene );

		gameObject.WorldScale = new Vector3( x, y, z );

		return gameObject.Serialize();
	}

	[McpEditorTool]
	public static JsonObject SetGameObjectLocalPosition( string id, float x, float y, float z, string? sceneId = null )
	{
		var scene = GetSceneOrActive( sceneId );

		var gameObject = GetGameObjectById( new Guid( id ), scene );

		gameObject.LocalPosition = new Vector3( x, y, z );

		return gameObject.Serialize();
	}

	[McpEditorTool]
	public static JsonObject SetGameObjectLocalRotation( string id, float x, float y, float z, float w, string? sceneId = null )
	{
		var scene = GetSceneOrActive( sceneId );

		var gameObject = GetGameObjectById( new Guid( id ), scene );

		gameObject.LocalRotation = new Rotation( x, y, z, w );

		return gameObject.Serialize();
	}

	[McpEditorTool]
	public static JsonObject SetGameObjectLocalScale( string id, float x, float y, float z, string? sceneId = null )
	{
		var scene = GetSceneOrActive( sceneId );

		var gameObject = GetGameObjectById( new Guid( id ), scene );

		gameObject.LocalScale = new Vector3( x, y, z );

		return gameObject.Serialize();
	}

	// Hierarchy Commands
	[McpEditorTool]
	public static JsonObject SetGameObjectParent( string id, string? parentId, string? sceneId = null )
	{
		var scene = GetSceneOrActive( sceneId );

		var gameObject = GetGameObjectById( new Guid( id ), scene );

		GameObject? parent = null;
		if ( parentId != null )
		{
			parent = GetGameObjectById( new Guid( parentId ), scene );
		}

		gameObject.SetParent( parent );

		return gameObject.Serialize();
	}

	[McpEditorTool]
	public static JsonArray GetGameObjectChildren( string id, string? sceneId = null )
	{
		var scene = GetSceneOrActive( sceneId );
		var gameObject = GetGameObjectById( new Guid( id ), scene );

		return new JsonArray( gameObject.Children.Select( c => c.Serialize() ).ToArray() );
	}

	[McpEditorTool]
	public static JsonObject GetGameObjectParent( string id, string? sceneId = null )
	{
		var scene = GetSceneOrActive( sceneId );

		if ( !Guid.TryParse( id, out Guid parsedId ) )
		{
			throw new InvalidOperationException( $"Invalid ID format: {id}" );
		}

		var gameObject = GetGameObjectById( parsedId, scene );
		if ( gameObject.Parent == null )
		{
			return new JsonObject( null );
		}

		return gameObject.Parent.Serialize();
	}

	// Property Commands
	[McpEditorTool]
	public static JsonObject SetGameObjectName( string id, string name, string? sceneId = null )
	{
		var scene = GetSceneOrActive( sceneId );

		var gameObject = GetGameObjectById( new Guid( id ), scene );

		gameObject.Name = name;

		return gameObject.Serialize();
	}

	[McpEditorTool]
	public static JsonObject SetGameObjectEnabled( string id, bool enabled, string? sceneId = null )
	{
		var scene = GetSceneOrActive( sceneId );

		var gameObject = GetGameObjectById( new Guid( id ), scene );

		gameObject.Enabled = enabled;

		return gameObject.Serialize();
	}

	// Component Commands
	[McpEditorTool]
	public static JsonObject AddGameObjectComponent( string id, string componentType, string? sceneId = null )
	{
		var scene = GetSceneOrActive( sceneId );
		var gameObject = GetGameObjectById( new Guid( id ), scene );

		// Try multiple component type resolution strategies
		TypeDescription? typeDescription = null;

		// Strategy 1: Exact name match
		typeDescription = TypeLibrary.GetTypes()
			.Where( t => t.TargetType.IsAssignableTo( typeof( Component ) ) )
			.FirstOrDefault( t => t.Name == componentType );

		// Strategy 2: Try without namespace prefix
		if ( typeDescription == null && componentType.Contains( '.' ) )
		{
			string shortName = componentType.Split( '.' ).Last();
			typeDescription = TypeLibrary.GetTypes()
				.Where( t => t.TargetType.IsAssignableTo( typeof( Component ) ) )
				.FirstOrDefault( t => t.Name == shortName || t.TargetType.Name == shortName );
		}

		// Strategy 3: Try with Sandbox prefix
		if ( typeDescription == null && !componentType.StartsWith( "Sandbox." ) )
		{
			typeDescription = TypeLibrary.GetTypes()
				.Where( t => t.TargetType.IsAssignableTo( typeof( Component ) ) )
				.FirstOrDefault( t => t.Name == $"Sandbox.{componentType}" );
		}

		if ( typeDescription == null )
		{
			var availableTypes = TypeLibrary.GetTypes()
				.Where( t => t.TargetType.IsAssignableTo( typeof( Component ) ) )
				.Select( t => t.Name )
				.ToList();

			throw new InvalidOperationException(
				$"Component type '{componentType}' not found. Available types include: {string.Join( ", ", availableTypes )}" );
		}

		var type = typeDescription.TargetType;
		var addComponentMethod = typeof( GameObject ).GetMethod( "AddComponent", [typeof( bool )] );
		if ( addComponentMethod == null )
		{
			throw new InvalidOperationException( "AddComponent method not found" );
		}

		var genericMethod = addComponentMethod.MakeGenericMethod( type );
		genericMethod.Invoke( gameObject, [true] );

		return gameObject.Serialize();
	}

	[McpEditorTool]
	public static JsonObject RemoveGameObjectComponent( string id, string componentType, string? sceneId = null )
	{
		var scene = GetSceneOrActive( sceneId );

		var gameObject = GetGameObjectById( new Guid( id ), scene );

		// Try multiple component type matching strategies
		var component = gameObject.Components.GetAll().FirstOrDefault( c =>
			c.GetType().Name == componentType ||
			c.GetType().FullName == componentType ||
			c.GetType().Name == componentType.Split( '.' ).Last() ||
			c.GetType().FullName?.EndsWith( $".{componentType}" ) == true
		);

		if ( component == null )
		{
			throw new InvalidOperationException( $"Component '{componentType}' not found on GameObject '{gameObject.Name}'" );
		}

		component.Destroy();

		return gameObject.Serialize();
	}

	[McpEditorTool]
	public static JsonArray GetGameObjectComponents( string id, string? sceneId = null )
	{
		var scene = GetSceneOrActive( sceneId );
		var gameObject = GetGameObjectById( new Guid( id ), scene );

		return new JsonArray( gameObject.Components.GetAll().Select( c => c.Serialize() ).ToArray() );
	}

	[McpEditorTool]
	public static JsonObject GetGameObjectComponent( string id, string componentType, string? sceneId = null )
	{
		var scene = GetSceneOrActive( sceneId );

		var gameObject = GetGameObjectById( new Guid( id ), scene );

		var component = gameObject.Components.GetAll().FirstOrDefault( c => c.GetType().Name == componentType );
		if ( component == null )
		{
			return new JsonObject( null );
		}

		return (JsonObject)component.Serialize();
	}

	[McpEditorTool]
	public static JsonObject SetGameObjectComponentProperty( string id, string componentType, string propertyName, JsonNode value, string? sceneId = null )
	{
		var scene = GetSceneOrActive( sceneId );

		var gameObject = GetGameObjectById( new Guid( id ), scene );

		var component = gameObject.Components.GetAll().FirstOrDefault( c => c.GetType().Name == componentType );
		if ( component == null )
		{
			return new JsonObject( null );
		}

		// Use the type library to set the property, similar to Component.Reset()
		SerializedObject serializedObject = Game.TypeLibrary.GetSerializedObject( component );
		SerializedProperty property = serializedObject.GetProperty( propertyName );

		// Convert JsonNode to appropriate type and set the value
		property?.SetValue( value );

		return (JsonObject)component.Serialize();
	}

	private static Scene GetSceneOrActive( string? sceneId )
	{
		if ( sceneId == null )
			return SceneEditorSession.Active.Scene ?? throw new InvalidOperationException( "No active scene found" );

		// Try to parse as GUID first
		if ( Guid.TryParse( sceneId, out Guid parsedGuid ) )
		{
			var scene = SceneEditorSession.All.FirstOrDefault( s => s.Scene?.Id == parsedGuid )?.Scene;
			if ( scene != null )
				return scene;
		}

		// If not a GUID, try to match by scene name
		var sceneByName = SceneEditorSession.All.FirstOrDefault( s =>
			s.Scene?.Name?.Equals( sceneId, StringComparison.OrdinalIgnoreCase ) == true )?.Scene;

		if ( sceneByName != null )
			return sceneByName;

		// If still not found, return active scene as fallback
		return SceneEditorSession.Active.Scene ?? throw new InvalidOperationException( $"Scene '{sceneId}' not found and no active scene available" );
	}

	private static GameObject GetGameObjectById( Guid guid, Scene scene )
	{
		var gameObject = scene.GetAllObjects( false ).FirstOrDefault( go => go.Id == guid );
		if ( gameObject == null )
		{
			throw new InvalidOperationException( $"GameObject with id {guid} not found" );
		}

		return gameObject;
	}
}
