using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BirthReminde.Models;

namespace BirthReminde.Settings;

/// <summary>
/// 文件导入分析结果（CSV / Excel 通用）。
/// </summary>
public class ImportResult
{
    /// <summary>文件中解析出的全部生日（内部同名已去重，最后一条生效）</summary>
    public List<BirthdayInfo> AllBirthdays { get; } = new();

    /// <summary>与现有列表不重名、可以新增的条目</summary>
    public List<BirthdayInfo> NewBirthdays { get; } = new();

    /// <summary>与现有列表重名（按名字忽略大小写、去首尾空格比对）的条目</summary>
    public List<BirthdayInfo> Duplicates { get; } = new();
}

/// <summary>
/// 文件导入的公共分析逻辑：日期解析、内部同名去重、与现有列表比对。
/// 供 CSV 与 Excel 导入共用，避免逻辑分叉。
/// </summary>
public static class ImportAnalyzer
{
    private static readonly string[] DateFormats =
    {
        "yyyy/M/d",
        "yyyy/M/d H:mm:ss",
        "yyyy-MM-dd",
        "yyyy-M-d",
        "yyyy-MM-dd HH:mm:ss",
        "yyyy.M.d",
        "M/d/yyyy",
        "M-d-yyyy",
        "yyyy年M月d日",
        "yyyy年M月d日 HH:mm:ss"
    };

    /// <summary>
    /// 对解析出的原始生日做内部同名去重，并与现有列表比对，返回可新增与重名条目。
    /// </summary>
    public static ImportResult Analyze(IEnumerable<BirthdayInfo> raw, IEnumerable<BirthdayInfo> existingBirthdays)
    {
        var result = new ImportResult();

        // 内部同名去重：同名多行时最后一条内容生效，位置保留首次出现处
        var ordered = new List<BirthdayInfo>();
        var byName = new Dictionary<string, BirthdayInfo>(StringComparer.OrdinalIgnoreCase);
        foreach (var birthday in raw)
        {
            var key = birthday.Name.Trim();
            if (byName.TryGetValue(key, out var previous))
            {
                previous.Date = birthday.Date;
                previous.Notes = birthday.Notes;
            }
            else
            {
                byName[key] = birthday;
                ordered.Add(birthday);
            }
        }

        result.AllBirthdays.AddRange(ordered);

        // 与现有列表按名字比对（忽略大小写、去首尾空格）
        var existingNames = new HashSet<string>(
            existingBirthdays
                .Where(x => !string.IsNullOrWhiteSpace(x.Name))
                .Select(x => x.Name.Trim()),
            StringComparer.OrdinalIgnoreCase);
        foreach (var birthday in ordered)
        {
            if (existingNames.Contains(birthday.Name.Trim()))
                result.Duplicates.Add(birthday);
            else
                result.NewBirthdays.Add(birthday);
        }

        return result;
    }

    /// <summary>
    /// 宽松解析文本日期，兼容带不带前导零、斜杠/横杠/点分隔、中文日期等多种写法。
    /// </summary>
    public static bool TryParseDate(string text, out DateTime date)
    {
        date = default;
        if (string.IsNullOrWhiteSpace(text))
            return false;

        text = text.Trim();

        if (DateTime.TryParseExact(text, DateFormats, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out date))
            return true;

        if (DateTime.TryParse(text, CultureInfo.GetCultureInfo("zh-CN"), DateTimeStyles.None, out date))
            return true;

        return DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
    }

    /// <summary>
    /// 解析 Excel 单元格值为日期：兼容真实日期（DateTime）、文本（string）与
    /// Excel 日期序列号（数字，OADate）。
    /// </summary>
    public static bool TryParseCellDate(object? value, out DateTime date)
    {
        date = default;
        if (value == null)
            return false;

        switch (value)
        {
            case DateTime dt:
                date = dt;
                return true;
            case string s:
                return TryParseDate(s, out date);
            case double d:
                return TryFromOADate(d, out date);
            case float f:
                return TryFromOADate(f, out date);
            case decimal m:
                return TryFromOADate((double)m, out date);
            case long l:
                return TryFromOADate(l, out date);
            case int i:
                return TryFromOADate(i, out date);
            default:
                return TryParseDate(value.ToString() ?? string.Empty, out date);
        }
    }

    private static bool TryFromOADate(double value, out DateTime date)
    {
        date = default;
        // Excel 日期序列号有效范围（1900-01-01 到 9999-12-31）
        if (value < 1 || value > 2958465)
            return false;
        try
        {
            date = DateTime.FromOADate(value);
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }
}
