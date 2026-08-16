using System.Collections.Generic;
using System.IO;
using BirthReminde.Models;

namespace BirthReminde.Settings;

/// <summary>
/// 文件导入统一入口：按扩展名分发到 CSV 或 Excel 解析器。
/// </summary>
public static class ImortFile
{
    public static ImportResult AnalyzeImport(string filePath, IEnumerable<BirthdayInfo> existingBirthdays)
    {
        var ext = Path.GetExtension(filePath).ToLowerInvariant();
        return ext switch
        {
            ".xlsx" => ImortExcel.AnalyzeImport(filePath, existingBirthdays),
            _ => ImortCSV.AnalyzeImport(filePath, existingBirthdays)
        };
    }
}
