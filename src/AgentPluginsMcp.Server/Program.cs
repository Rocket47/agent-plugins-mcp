using AgentPluginsMcp.Server.Tools;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMcpServer()
    .WithHttpTransport(options =>
    {
        options.Stateless = true;
    })
    .WithTools<DeveloperTools>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    service = "developer-utilities-mcp",
    protocol = "2026-07-28"
}));
app.MapMcp("/mcp");

await app.RunAsync();
