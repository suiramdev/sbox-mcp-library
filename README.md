# Model Context Protocol for s&box

[![.NET](https://img.shields.io/badge/.NET-9.0-blue)](https://dotnet.microsoft.com/)
[![s&box](https://img.shields.io/badge/s%26box-Compatible-orange)](https://sbox.game/)

> [!IMPORTANT]
> This project is currently under active development.

A s&box library that enables AI assistants to interact with the s&box editor through the Model Context Protocol (MCP) via real-time WebSocket communication.

This adapter library works in conjunction with the separate [MCP Server](https://github.com/suiramdev/sbox-mcp-server) to provide seamless integration between AI assistants and your s&box projects.

![](./Assets/example_1.gif)

## Prerequisites

- [s&box](https://sbox.game/) (latest version)
- [MCP Server](https://github.com/suiramdev/sbox-mcp-server) (must be installed and running separately)
- An MCP-compatible AI assistant (Claude Desktop, Cursor, etc.)

## Quick Start

### Step 1: Install and Run the MCP Server

Before using this adapter library, you must first install and run the MCP Server:

1. **Install the MCP Server** from the [sbox-mcp-server repository](https://github.com/suiramdev/sbox-mcp-server)
2. **Follow the setup instructions** in the MCP Server repository to:
   - Build and run the server
   - Configure your AI assistant
3. **Ensure the server is running** before proceeding to the next step

> [!IMPORTANT]
> The MCP Server must be running for this adapter library to function. Please refer to the [MCP Server documentation](https://github.com/suiramdev/sbox-mcp-server) for detailed installation and configuration instructions.

### Step 2: Install the Adapter Library in s&box

Once the MCP Server is running:

1. **Install this library** in your s&box project through the **Asset Library**. [Find it on asset.party](https://sbox.game/sdv/modelcontextprotocol)

2. **Navigate to MCP → Connect to MCP Server** in the s&box editor menu bar.

3. **Listen for the connection confirmation**: You'll hear a success sound and see a success message in the console when connected.

## Usage

Once both the MCP Server and this adapter library are installed and connected, you can interact with your s&box project using natural language through your AI assistant:

```
"Create a ModelRenderer component on the Cube object"
"Find all game objects named 'Player'"
"Set the Scale property of the Transform component on MainCamera to 2,2,2"
"Remove the Rigidbody component from the Ball object"
"Show me all components attached to the Ground object"
```

## Troubleshooting

### Connection Issues

If the **Connect to MCP Server** button fails:

1. **Verify the MCP Server is running** - Check the [MCP Server repository](https://github.com/suiramdev/sbox-mcp-server) for troubleshooting steps
2. **Review the s&box console** for error messages

### Testing the Connection

You can manually test if the MCP Server is accessible by:
- Using [Postman](https://www.postman.com/downloads/) or similar API clients to send requests to the server
- Checking the MCP Server console output for connection attempts

## Architecture

This adapter library acts as a bridge between:
- **s&box Editor** ↔ **This Adapter Library** ↔ **MCP Server** ↔ **AI Assistant**

The adapter library:
- Connects to the external MCP Server via WebSocket
- Translates MCP commands into s&box editor operations
- Provides real-time feedback to AI assistants

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request. For major changes, please open an issue first to discuss what you would like to change.

### Related Repositories

- **MCP Server**: [sbox-mcp-server](https://github.com/suiramdev/sbox-mcp-server) - The main server component
- **This Library**: Adapter library for s&box integration

## Support

- **Issues**: [GitHub Issues](https://github.com/suiramdev/sbox-mcp-library/issues)
- **Discussions**: [GitHub Discussions](https://github.com/suiramdev/sbox-mcp-library/discussions)
- **MCP Server Support**: [MCP Server Issues](https://github.com/suiramdev/sbox-mcp-server/issues)
