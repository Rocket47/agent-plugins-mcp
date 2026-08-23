---
name: mcp-smoke-testing
description: Verify that the developer-utilities MCP server is connected and its basic tools are callable.
---

# MCP Smoke Testing

Use this skill when the user asks to check, verify, test, or diagnose whether the `developer-utilities` MCP server is available from the agent client.

## Expected Checks

Run one or more lightweight tool calls that prove the MCP endpoint is reachable:

- Call `get_utc_time` to verify a no-argument tool.
- Call `echo` with a short test message to verify argument passing.
- Call `calculate_sha256` with a simple known input when hashing behavior needs to be checked.

## Reporting

- Report which MCP tools were called and whether each call succeeded.
- Include returned values only when they help confirm the check.
- If a tool call fails, explain the failing tool name and the likely setup area to inspect, such as the MCP server URL, running process, or plugin `mcp.json`.
