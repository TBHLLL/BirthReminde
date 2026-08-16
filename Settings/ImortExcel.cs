using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BirthReminde.Models;
using MiniExcelLibs;

namespace BirthReminde.Settings;

/// <summary>
/// Excel（.xlsx）文件导入：读取生日数据并复用公共的重名分析与日期解析。
/// </summary>
public static class ImortExcel
{
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

        // 统一按无表头读取（列名 A/B/C...），再根据首行是否像表头决定是否跳过
        var rows = MiniExcel.Query(filePath, useHeaderRow: false)
            .Cast<IDictionary<string, object>>()
            .ToList();
        if (rows.Count == 0)
            return birthdays;

        var startIndex = LooksLikeHeader(rows[0]) ? 1 : 0;

        for (var i = startIndex; i < rows.Count; i++)
        {
            var row = rows[i];
            var name = GetCell(row, "A")?.ToString()?.Trim();
            if (string.IsNullOrWhiteSpace(name))
                continue;

            if (!ImportAnalyzer.TryParseCellDate(GetCell(row, "B"), out var date))
                continue;

            var notes = GetCell(row, "C")?.ToString()?.Trim();

            birthdays.Add(new BirthdayInfo
            {
                Name = name,
                Date = date,
                Notes = string.IsNullOrEmpty(notes) ? null : notes
            });
        }

        return birthdays;
    }

    /// <summary>首行是否像表头：第一列是姓名、第二列是日期/生日。</summary>
    private static bool LooksLikeHeader(IDictionary<string, object> firstRow)
    {
        var a = GetCell(firstRow, "A")?.ToString()?.Trim();
        var b = GetCell(firstRow, "B")?.ToString()?.Trim();
        return IsNameColumn(a) && IsDateColumn(b);
    }

    private static bool IsNameColumn(string? text)
        => string.Equals(text, "姓名", StringComparison.OrdinalIgnoreCase)
           || string.Equals(text, "name", StringComparison.OrdinalIgnoreCase);

    private static bool IsDateColumn(string? text)
        => string.Equals(text, "日期", StringComparison.OrdinalIgnoreCase)
           || string.Equals(text, "生日", StringComparison.OrdinalIgnoreCase)
           || string.Equals(text, "date", StringComparison.OrdinalIgnoreCase)
           || string.Equals(text, "birthday", StringComparison.OrdinalIgnoreCase);

    private static object? GetCell(IDictionary<string, object> row, string column)
        => row.TryGetValue(column, out var value) ? value : null;
}
