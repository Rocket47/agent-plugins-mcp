using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

var serverAssembly = Path.GetFullPath(Path.Combine(
    Directory.GetCurrentDirectory(),
    "bin",
    "AgentPluginsMcp.Server.dll"));

if (!File.Exists(serverAssembly))
{
    throw new FileNotFoundException(
        "Run scripts/publish.sh or scripts/publish.ps1 before the smoke test.",
        serverAssembly);
}

var transport = new StdioClientTransport(new StdioClientTransportOptions
{
    Name = "developer-utilities-smoke-test",
    Command = "dotnet",
    Arguments = [serverAssembly, "--transport", "stdio"],
    StandardErrorLines = line => Console.Error.WriteLine($"server: {line}")
});

using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(15));
await using var client = await McpClient.CreateAsync(
    transport,
    new McpClientOptions
    {
        ProtocolVersion = "2026-07-28",
        InitializationTimeout = TimeSpan.FromSeconds(10)
    },
    cancellationToken: timeout.Token);

var tools = await client.ListToolsAsync(cancellationToken: timeout.Token);
var expectedTools = new[]
{
    "analyze_text",
    "calculate_sha256",
    "echo",
    "get_utc_time"
};
var actualTools = tools.Select(tool => tool.Name).Order().ToArray();

if (!actualTools.SequenceEqual(expectedTools))
{
    throw new InvalidOperationException(
        $"Unexpected tools: {string.Join(", ", actualTools)}");
}

var result = await client.CallToolAsync(
    "echo",
    new Dictionary<string, object?> { ["message"] = "MCP 2026 works" },
    cancellationToken: timeout.Token);
var echoedText = result.Content
    .OfType<TextContentBlock>()
    .Single()
    .Text;

if (echoedText != "MCP 2026 works")
{
    throw new InvalidOperationException($"Unexpected echo result: {echoedText}");
}

Console.WriteLine(
    $"stdio smoke test passed; protocol={client.NegotiatedProtocolVersion}; tools={string.Join(",", actualTools)}");
