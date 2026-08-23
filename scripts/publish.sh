#!/usr/bin/env bash
set -euo pipefail

script_dir="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
plugin_root="$(cd -- "${script_dir}/.." && pwd)"

dotnet publish \
  "${plugin_root}/src/AgentPluginsMcp.Server/AgentPluginsMcp.Server.csproj" \
  --configuration Release \
  --output "${plugin_root}/bin"

printf 'HTTP MCP server published to %s\n' "${plugin_root}/bin/AgentPluginsMcp.Server.dll"
