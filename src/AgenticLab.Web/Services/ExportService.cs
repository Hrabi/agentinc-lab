using System.Text;
using System.Text.Json;

namespace AgenticLab.Web.Services;

/// <summary>
/// Converts and exports data in different formats based on templates.
/// </summary>
public class ExportService
{
    /// <summary>
    /// Exports data to the specified format.
    /// </summary>
    public string Export(object data, ExportFormat format) => format switch
    {
        ExportFormat.Json => ExportJson(data),
        ExportFormat.Csv => ExportCsv(data),
        ExportFormat.Markdown => ExportMarkdown(data),
        ExportFormat.PlainText => ExportPlainText(data),
        _ => throw new ArgumentOutOfRangeException(nameof(format))
    };

    private static string ExportJson(object data)
    {
        return JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }

    private static string ExportCsv(object data)
    {
        if (data is not IEnumerable<IDictionary<string, object>> rows)
        {
            // Convert single object to a one-row table
            var json = JsonSerializer.Serialize(data);
            var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
            if (dict is null) return "";

            var sb = new StringBuilder();
            sb.AppendLine(string.Join(",", dict.Keys.Select(EscapeCsv)));
            sb.AppendLine(string.Join(",", dict.Values.Select(v => EscapeCsv(v.ToString()))));
            return sb.ToString();
        }

        var allKeys = rows.SelectMany(r => r.Keys).Distinct().ToList();
        var csvBuilder = new StringBuilder();
        csvBuilder.AppendLine(string.Join(",", allKeys.Select(EscapeCsv)));

        foreach (var row in rows)
        {
            var values = allKeys.Select(k => row.TryGetValue(k, out var v) ? EscapeCsv(v?.ToString() ?? "") : "");
            csvBuilder.AppendLine(string.Join(",", values));
        }

        return csvBuilder.ToString();
    }

    private static string ExportMarkdown(object data)
    {
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        return $"```json\n{json}\n```";
    }

    private static string ExportPlainText(object data)
    {
        return data.ToString() ?? "";
    }

    private static string EscapeCsv(string? value)
    {
        if (value is null) return "";
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }
}

/// <summary>
/// Supported export formats.
/// </summary>
public enum ExportFormat
{
    Json,
    Csv,
    Markdown,
    PlainText
}
