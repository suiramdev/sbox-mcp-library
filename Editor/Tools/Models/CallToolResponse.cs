using SandboxModelContextProtocol.Editor.Connection.Models;

namespace SandboxModelContextProtocol.Editor.Tools.Models;

public class CallToolResponse : CallResponse
{
	public override string? Type { get; init; } = "call";
}
