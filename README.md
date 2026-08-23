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

`mcp.json` объявляет stdio-сервер. Совместимый клиент подставляет `${PLUGIN_ROOT}` и запускает проект через `dotnet`. Для готового к распространению пакета рекомендуется опубликовать self-contained executable и заменить `command` на путь вида `./bin/developer-utilities` — поле `command` не является shell-командой и не поддерживает подстановку `${PLUGIN_ROOT}`.

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
dotnet build src/AgentPluginsMcp.Server/AgentPluginsMcp.Server.csproj --configuration Release
dotnet run --project tests/AgentPluginsMcp.SmokeTests/AgentPluginsMcp.SmokeTests.csproj
```

После запуска HTTP-сервера:

```bash
curl http://localhost:5000/health
```

Для полного диалога MCP используйте клиент или [MCP Inspector](https://github.com/modelcontextprotocol/inspector).

## Структура и принятые решения

Сервер намеренно не хранит сессионное состояние. В MCP `2026-07-28` удалены обязательные `initialize`/`initialized` и `Mcp-Session-Id`; каждый запрос самодостаточен, а предварительное обнаружение возможностей выполняется через `server/discover`. Это позволяет масштабировать HTTP-сервер без sticky sessions.

Agent Plugins не заменяет MCP: `plugin.json` описывает пакет, `mcp.json` — способ подключения, а официальный MCP SDK реализует сам протокол.

## Источники

- [Agent Plugins 1.0.0](https://agent-plugins.org/)
- [Формат `plugin.json`](https://agent-plugins.org/plugin-authors/manifest)
- [Формат `mcp.json`](https://agent-plugins.org/plugin-authors/mcp-servers)
- [MCP 2026-07-28](https://blog.modelcontextprotocol.io/posts/2026-07-28/)
- [Официальный C# SDK](https://github.com/modelcontextprotocol/csharp-sdk)
