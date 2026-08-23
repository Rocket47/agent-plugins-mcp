using AgentPluginsMcp.Server.Tools;
using ModelContextProtocol.Server;

const string TransportOption = "--transport";

var transport = GetOption(args, TransportOption)
    ?? Environment.GetEnvironmentVariable("MCP_TRANSPORT")
    ?? "http";

if (transport.Equals("stdio", StringComparison.OrdinalIgnoreCase))
{
    var builder = Host.CreateApplicationBuilder(args);

    builder.Logging.AddConsole(options =>
    {
        options.LogToStandardErrorThreshold = LogLevel.Trace;
    });

    builder.Services
        .AddMcpServer()
        .WithStdioServerTransport()
        .WithTools<DeveloperTools>();

    await builder.Build().RunAsync();
    return;
}

if (!transport.Equals("http", StringComparison.OrdinalIgnoreCase))
{
    throw new ArgumentException(
        $"Unsupported transport '{transport}'. Use 'http' or 'stdio'.",
        TransportOption);
}

var webBuilder = WebApplication.CreateBuilder(args);

webBuilder.Services
    .AddMcpServer()
    .WithHttpTransport(options =>
    {
        options.Stateless = true;
    })
    .WithTools<DeveloperTools>();

var app = webBuilder.Build();

app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    service = "developer-utilities-mcp",
    protocol = "2026-07-28"
}));
app.MapMcp("/mcp");

await app.RunAsync();

static string? GetOption(string[] arguments, string option)
{
    for (var index = 0; index < arguments.Length - 1; index++)
    {
        if (arguments[index].Equals(option, StringComparison.OrdinalIgnoreCase))
        {
            return arguments[index + 1];
        }
    }

    return null;
}

