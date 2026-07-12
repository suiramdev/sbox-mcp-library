using SandboxModelContextProtocol.Editor.Connection.Models;

namespace SandboxModelContextProtocol.Editor.Tools.Models;

public class CallToolRequest : CallRequest
{
	public override string? Type { get; init; } = "call";
}
