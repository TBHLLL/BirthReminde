using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BirthReminde.Models;
using MiniExcelLibs;

namespace BirthReminde.Settings;

/// <summary>
/// 生日数据导出：CSV 与 Excel（.xlsx）。
/// </summary>
public static class ExportFile
{
    public static void ExportCsv(string path, IEnumerable<BirthdayInfo> birthdays)
    {
        var sb = new StringBuilder();
        sb.AppendLine("姓名,日期,备注");
        foreach (var b in birthdays)
        {
            sb.AppendLine($"{EscapeCsv(b.Name)},{b.Date:yyyy-MM-dd},{EscapeCsv(b.Notes ?? string.Empty)}");
        }

        // UTF-8 带 BOM，保证 Excel/WPS 打开中文不乱码（与导入端编码识别配套）
        File.WriteAllText(path, sb.ToString(), new UTF8Encoding(true));
    }

    public static void ExportExcel(string path, IEnumerable<BirthdayInfo> birthdays)
    {
        var rows = birthdays
            .Select(b => new
            {
                姓名 = b.Name,
                日期 = b.Date.ToString("yyyy-MM-dd"),
                备注 = b.Notes ?? string.Empty
            })
            .ToList();

        MiniExcel.SaveAs(path, rows, printHeader: true, sheetName: "生日列表", overwriteFile: true);
    }

    /// <summary>CSV 字段转义：含逗号/引号/换行时用双引号包裹并转义内部引号。</summary>
    private static string EscapeCsv(string field)
    {
        if (field.Contains(',') || field.Contains('"') || field.Contains('\n') || field.Contains('\r'))
            return "\"" + field.Replace("\"", "\"\"") + "\"";
        return field;
    }
}
