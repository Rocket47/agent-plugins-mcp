using System.Diagnostics;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

var serverAssembly = Path.GetFullPath(Path.Combine(
    Directory.GetCurrentDirectory(),
    "bin",
    "AgentPluginsMcp.Server.dll"));

using var server = Process.Start(new ProcessStartInfo
{
    FileName = "dotnet",
    UseShellExecute = false,
    Environment = { ["ASPNETCORE_URLS"] = "http://127.0.0.1:5050" },
    ArgumentList = { serverAssembly }
}) ?? throw new InvalidOperationException("Unable to start the MCP server.");

try
{
    using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(20));
    using var httpClient = new HttpClient();

    while (true)
    {
        try
        {
            using var healthResponse = await httpClient.GetAsync(
                "http://127.0.0.1:5050/health",
                timeout.Token);

            if (healthResponse.IsSuccessStatusCode)
            {
                break;
            }
        }
        catch (HttpRequestException) when (!timeout.IsCancellationRequested)
        {
        }

        await Task.Delay(100, timeout.Token);
    }

    var transport = new HttpClientTransport(new HttpClientTransportOptions
    {
        Name = "developer-utilities-smoke-test",
        Endpoint = new Uri("http://127.0.0.1:5050/mcp"),
        TransportMode = HttpTransportMode.StreamableHttp
    });

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
        $"HTTP smoke test passed; protocol={client.NegotiatedProtocolVersion}; tools={string.Join(",", actualTools)}");
}
finally
{
    if (!server.HasExited)
    {
        server.Kill(entireProcessTree: true);
        await server.WaitForExitAsync();
    }
}
