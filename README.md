# Developer Utilities MCP

Пример переносимого Agent Plugin 1.0.0 с MCP-сервером на .NET. Сервер использует официальный C# SDK 2.0 и поддерживает MCP `2026-07-28`.

## Что реализовано

- stateless Streamable HTTP endpoint `POST /mcp` — основной режим для MCP `2026-07-28`;
- stdio transport для запуска сервером-совместимым клиентом из `mcp.json`;
- `server/discover` и обратная совместимость обеспечиваются официальным SDK;
- четыре stateless-инструмента: `echo`, `get_utc_time`, `calculate_sha256` и `analyze_text`;
- стандартные файлы Agent Plugins 1.0.0: `plugin.json` и `mcp.json`.

## Требования

- .NET SDK 10.0 или новее;
- клиент с поддержкой MCP `2026-07-28` либо совместимый клиент предыдущей версии.

## Сборка исполняемого приложения

Репозиторий содержит обычное исполняемое ASP.NET Core-приложение и solution `AgentPluginsMcp.slnx`:

```bash
dotnet build AgentPluginsMcp.slnx --configuration Release
./scripts/publish.sh
```

Windows PowerShell:

```powershell
dotnet build AgentPluginsMcp.slnx --configuration Release
./scripts/publish.ps1
```

Публикация создаёт в `bin/` готовое framework-dependent приложение:

```text
bin/
├── AgentPluginsMcp.Server       # нативный launcher на macOS/Linux
├── AgentPluginsMcp.Server.exe   # launcher при публикации на Windows
├── AgentPluginsMcp.Server.dll   # переносимая .NET-сборка
└── *.deps.json / *.runtimeconfig.json / зависимости
```

Именно опубликованную `bin/AgentPluginsMcp.Server.dll`, а не исходный проект, запускает `mcp.json`. Каталог `bin/` не коммитится: он создаётся при сборке и включается в распространяемый архив Agent Plugin.

## Быстрый старт: HTTP

```bash
dotnet restore src/AgentPluginsMcp.Server/AgentPluginsMcp.Server.csproj
dotnet run --project src/AgentPluginsMcp.Server/AgentPluginsMcp.Server.csproj
```

По умолчанию сервер слушает `http://localhost:5000`, MCP endpoint доступен по адресу `http://localhost:5000/mcp`, а health check — по адресу `http://localhost:5000/health`.

Порт можно изменить стандартным способом ASP.NET Core:

```bash
ASPNETCORE_URLS=http://127.0.0.1:5050 \
  dotnet run --project src/AgentPluginsMcp.Server/AgentPluginsMcp.Server.csproj
```

## Быстрый старт: stdio

```bash
dotnet run \
  --project src/AgentPluginsMcp.Server/AgentPluginsMcp.Server.csproj \
  --configuration Release \
  --no-launch-profile \
  -- \
  --transport stdio
```

В stdio-режиме логи направляются в `stderr`, чтобы не повреждать JSON-RPC сообщения в `stdout`.

## Agent Plugin

Корень репозитория уже является пакетом Agent Plugin:

```text
.
├── plugin.json
├── mcp.json
└── src/
    └── AgentPluginsMcp.Server/
```

`mcp.json` объявляет stdio-сервер. Совместимый клиент подставляет `${PLUGIN_ROOT}` в путь к опубликованной сборке и выполняет эквивалент команды:

```bash
dotnet "${PLUGIN_ROOT}/bin/AgentPluginsMcp.Server.dll" --transport stdio
```

Подстановка применяется в `args`, но не в `command`, поэтому исполняемой командой указан доступный через `PATH` хост `dotnet`.

Пример удалённой конфигурации после публикации HTTP-сервера:

```json
{
  "$schema": "https://agent-plugins.org/schemas/1.0.0/mcp.schema.json",
  "mcpServers": {
    "developer-utilities": {
      "type": "streamable-http",
      "url": "https://example.com/mcp"
    }
  }
}
```

Секреты нельзя помещать в `mcp.json`: стандарт Agent Plugins 1.0.0 оставляет аутентификацию клиенту.

## Проверка

```bash
dotnet build AgentPluginsMcp.slnx --configuration Release
./scripts/publish.sh
dotnet run --project tests/AgentPluginsMcp.SmokeTests/AgentPluginsMcp.SmokeTests.csproj
```

После запуска HTTP-сервера:

```bash
curl http://localhost:5000/health
```

Для полного диалога MCP используйте клиент или [MCP Inspector](https://github.com/modelcontextprotocol/inspector).
## Источники

- [Agent Plugins 1.0.0](https://agent-plugins.org/)
- [Формат `plugin.json`](https://agent-plugins.org/plugin-authors/manifest)
- [Формат `mcp.json`](https://agent-plugins.org/plugin-authors/mcp-servers)
- [MCP 2026-07-28](https://blog.modelcontextprotocol.io/posts/2026-07-28/)
