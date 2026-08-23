$ErrorActionPreference = "Stop"

$PluginRoot = Split-Path -Parent $PSScriptRoot
$Project = Join-Path $PluginRoot "src/AgentPluginsMcp.Server/AgentPluginsMcp.Server.csproj"
$Output = Join-Path $PluginRoot "bin"

dotnet publish $Project --configuration Release --output $Output

if ($LASTEXITCODE -ne 0) {
    throw "dotnet publish failed with exit code $LASTEXITCODE"
}

Write-Host "HTTP MCP server published to $Output/AgentPluginsMcp.Server.dll"
