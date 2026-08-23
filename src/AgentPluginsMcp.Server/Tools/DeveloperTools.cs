using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using ModelContextProtocol.Server;

namespace AgentPluginsMcp.Server.Tools;

[McpServerToolType]
public sealed class DeveloperTools
{
    [McpServerTool(Name = "echo")]
    [Description("Returns the supplied text unchanged. Useful for checking MCP connectivity.")]
    public static string Echo(
        [Description("Text to return.")] string message) => message;

    [McpServerTool(Name = "get_utc_time")]
    [Description("Returns the current UTC time in ISO 8601 format.")]
    public static string GetUtcTime() => DateTimeOffset.UtcNow.ToString("O");

    [McpServerTool(Name = "calculate_sha256")]
    [Description("Calculates the SHA-256 digest of UTF-8 text and returns lowercase hexadecimal.")]
    public static string CalculateSha256(
        [Description("UTF-8 text to hash.")] string text)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(text));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    [McpServerTool(Name = "analyze_text")]
    [Description("Counts characters, Unicode runes, words, and lines in text.")]
    public static TextStatistics AnalyzeText(
        [Description("Text to analyze.")] string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        var wordCount = 0;
        var insideWord = false;

        foreach (var rune in text.EnumerateRunes())
        {
            if (Rune.IsWhiteSpace(rune))
            {
                insideWord = false;
            }
            else if (!insideWord)
            {
                wordCount++;
                insideWord = true;
            }
        }

        var lineCount = text.Length == 0 ? 0 : 1;
        for (var index = 0; index < text.Length; index++)
        {
            if (text[index] == '\n')
            {
                lineCount++;
            }
        }

        return new TextStatistics(
            Characters: text.Length,
            UnicodeScalars: text.EnumerateRunes().Count(),
            Words: wordCount,
            Lines: lineCount);
    }
}

public sealed record TextStatistics(
    int Characters,
    int UnicodeScalars,
    int Words,
    int Lines);
