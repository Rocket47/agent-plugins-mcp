using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using ModelContextProtocol.Server;

namespace AgentPluginsMcp.Server.Tools;

[McpServerToolType]
public sealed class DeveloperTools
{
    [McpServerTool(Name = "echo")]
    public static string Echo(
        [Description("Text to return.")] string message) => message;

    [McpServerTool(Name = "get_utc_time")]
    public static string GetUtcTime() => DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

    [McpServerTool(Name = "calculate_sha256")]
    public static string CalculateSha256(
        [Description("UTF-8 text to hash.")] string text)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(text));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
