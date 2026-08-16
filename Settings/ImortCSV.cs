using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BirthReminde.Models;

namespace BirthReminde.Settings;

public class ImortCSV
{
    /// <summary>
    /// 解析 CSV 并与现有列表比对，返回可新增的条目和重名条目。
    /// </summary>
    public static ImportResult AnalyzeImport(string filePath, IEnumerable<BirthdayInfo> existingBirthdays)
    {
        var raw = ParseFile(filePath);
        return ImportAnalyzer.Analyze(raw, existingBirthdays);
    }

    private static List<BirthdayInfo> ParseFile(string filePath)
    {
        var birthdays = new List<BirthdayInfo>();
        if (!File.Exists(filePath))
            return birthdays;

        var text = ReadFileText(filePath);
        var lines = text.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var parts = SplitCsvLine(line);
            if (parts.Count < 2)
                continue;

            var name = parts[0].Trim();
            if (string.IsNullOrWhiteSpace(name))
                continue;

            if (!ImportAnalyzer.TryParseDate(parts[1], out var date))
                continue;

            var notes = parts.Count > 2 ? string.Join(",", parts.Skip(2)).Trim() : null;

            birthdays.Add(new BirthdayInfo
            {
                Name = name,
                Date = date,
                Notes = string.IsNullOrEmpty(notes) ? null : notes
            });
        }

        return birthdays;
    }

    /// <summary>
    /// 自动识别编码读取文件：支持 UTF-8（含 BOM）、UTF-16 和中文常用的 GBK/GB18030。
    /// </summary>
    private static string ReadFileText(string filePath)
    {
        var bytes = File.ReadAllBytes(filePath);

        // UTF-8 BOM
        if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
            return Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3);
        // UTF-16 LE BOM
        if (bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE)
            return Encoding.Unicode.GetString(bytes, 2, bytes.Length - 2);
        // UTF-16 BE BOM
        if (bytes.Length >= 2 && bytes[0] == 0xFE && bytes[1] == 0xFF)
            return Encoding.BigEndianUnicode.GetString(bytes, 2, bytes.Length - 2);

        // 无 BOM：先按 UTF-8 严格解码，失败则回退 GB18030（兼容旧版 Excel 导出的 ANSI/GBK 文件）
        try
        {
            return new UTF8Encoding(false, true).GetString(bytes);
        }
        catch (DecoderFallbackException)
        {
            try
            {
                return Encoding.GetEncoding("GB18030").GetString(bytes);
            }
            catch (Exception)
            {
                return Encoding.UTF8.GetString(bytes);
            }
        }
    }

    /// <summary>
    /// 支持引号包裹字段的 CSV 行拆分（备注中包含逗号时也能正确解析）。
    /// </summary>
    private static List<string> SplitCsvLine(string line)
    {
        var fields = new List<string>();
        var current = new StringBuilder();
        var inQuotes = false;

        for (var i = 0; i < line.Length; i++)
        {
            var c = line[i];
            if (inQuotes)
            {
                if (c == '"')
                {
                    if (i + 1 < line.Length && line[i + 1] == '"')
                    {
                        current.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = false;
                    }
                }
                else
                {
                    current.Append(c);
                }
            }
            else
            {
                switch (c)
                {
                    case '"':
                        inQuotes = true;
                        break;
                    case ',':
                        fields.Add(current.ToString());
                        current.Clear();
                        break;
                    default:
                        current.Append(c);
                        break;
                }
            }
        }

        fields.Add(current.ToString());
        return fields;
    }
}
