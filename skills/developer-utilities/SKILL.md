---
name: developer-utilities
description: Use developer utility MCP tools for UTC time, SHA-256 hashing.
---

# Developer Utilities

Use this skill when the user asks for a small developer utility operation that can be completed through the `developer-utilities` MCP server.

## MCP Tools

- Use `get_utc_time` when the user asks for the current UTC time.
- Use `calculate_sha256` when the user asks to calculate a SHA-256 hash for text.
- Use `echo` only when the user explicitly asks to test echo behavior or MCP connectivity.

## Rules

- Preserve the user's input text exactly when passing it to `calculate_sha256`.
- For SHA-256, treat the text as UTF-8 input and return the hex digest produced by the MCP tool.
- Do not use `get_utc_time` for local time unless the user explicitly asks for UTC or accepts UTC as the answer.
- If a request needs business context outside these utility tools, answer normally or ask for the missing context instead of forcing an MCP call.
