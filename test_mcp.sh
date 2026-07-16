#!/bin/bash
# Test MCP server by sending JSON-RPC messages

echo '{"jsonrpc":"2.0","id":1,"method":"initialize","params":{"protocolVersion":"2024-11-05","capabilities":{},"clientInfo":{"name":"test","version":"1.0"}}}' | \
dotnet run --project MultiAgentCodeReview.McpServer --no-build 2>/dev/null

echo '{"jsonrpc":"2.0","method":"notifications/initialized"}' | \
dotnet run --project MultiAgentCodeReview.McpServer --no-build 2>/dev/null

echo '{"jsonrpc":"2.0","id":2,"method":"tools/list","params":{}}' | \
dotnet run --project MultiAgentCodeReview.McpServer --no-build 2>/dev/null
