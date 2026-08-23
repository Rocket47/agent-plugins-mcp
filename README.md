# Developer Utilities MCP

Stateless Streamable HTTP MCP-сервер на .NET 10. Использует официальный C# SDK 2.0 и поддерживает MCP `2026-07-28`.

## Возможности

Сервер предоставляет четыре инструмента:

- `echo` — возвращает переданный текст;
- `get_utc_time` — возвращает текущее UTC-время;
- `calculate_sha256` — вычисляет SHA-256;
- `analyze_text` — считает символы, Unicode-кодовые точки, слова и строки.

Доступны два HTTP endpoint:

- `GET /health` — проверка состояния;
- `POST /mcp` — stateless Streamable HTTP MCP.

STDIO transport намеренно не поддерживается.

## Требования

- .NET SDK 10.0 или новее;
- MCP-клиент с поддержкой Streamable HTTP.

## Запуск из исходников

```bash
dotnet run --project src/AgentPluginsMcp.Server/AgentPluginsMcp.Server.csproj
```

По умолчанию сервер слушает `http://localhost:5000`:

```text
http://localhost:5000/health
http://localhost:5000/mcp
```

Другой адрес можно задать через стандартную переменную ASP.NET Core:

```bash
ASPNETCORE_URLS=http://127.0.0.1:5050 \
  dotnet run --project src/AgentPluginsMcp.Server/AgentPluginsMcp.Server.csproj
```

## Сборка приложения

```bash
dotnet build AgentPluginsMcp.slnx --configuration Release
./scripts/publish.sh
```

Windows PowerShell:

```powershell
dotnet build AgentPluginsMcp.slnx --configuration Release
./scripts/publish.ps1
```

Готовое framework-dependent приложение появится в `bin/`. Запуск опубликованной версии:

```bash
ASPNETCORE_URLS=http://127.0.0.1:5000 \
  dotnet bin/AgentPluginsMcp.Server.dll
```

Каталог `bin/` создаётся при публикации и не коммитится.

## Agent Plugin

`mcp.json` объявляет HTTP-подключение:

```json
{
  "$schema": "https://agent-plugins.org/schemas/1.0.0/mcp.schema.json",
  "mcpServers": {
    "developer-utilities": {
      "type": "streamable-http",
      "url": "http://127.0.0.1:5000/mcp"
    }
  }
}
```

Перед использованием локального плагина сервер необходимо запустить отдельно. Для удалённого развёртывания замените URL в `mcp.json` на публичный HTTPS endpoint.

## Подключение к Codex

Запустите сервер, затем зарегистрируйте его:

```bash
codex mcp add developer-utilities --url http://127.0.0.1:5000/mcp
```

Проверка:

```bash
codex mcp list
```

После изменения конфигурации перезапустите Codex.

## Тестирование

Smoke-тест самостоятельно запускает опубликованный HTTP-сервер на порту `5050`, выполняет `server/discover`, `tools/list` и `tools/call`, затем останавливает процесс:

```bash
./scripts/publish.sh
dotnet run --project tests/AgentPluginsMcp.SmokeTests/AgentPluginsMcp.SmokeTests.csproj
```

Ручная проверка запущенного сервера:

```bash
curl http://127.0.0.1:5000/health
```

## Источники

- [Agent Plugins 1.0.0](https://agent-plugins.org/)
- [Формат `mcp.json`](https://agent-plugins.org/plugin-authors/mcp-servers)
- [MCP 2026-07-28](https://blog.modelcontextprotocol.io/posts/2026-07-28/)
- [Официальный C# SDK](https://github.com/modelcontextprotocol/csharp-sdk)
