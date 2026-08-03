using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using BirthReminde.Models;

namespace BirthReminde.Settings;

public class ImortCSV
{
    public static List<BirthdayInfo> ImportFromFile(string filePath)
    {
        var birthdays = new List<BirthdayInfo>();

        if (!File.Exists(filePath))
            return birthdays;

        var lines = File.ReadAllLines(filePath);
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var parts = line.Split(',');
            if (parts.Length < 2)
                continue;

            var name = parts[0].Trim();
            if (string.IsNullOrEmpty(name))
                continue;

            if (!DateTime.TryParseExact(parts[1].Trim(), "yyyy/MM/dd",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                continue;

            var notes = parts.Length > 2 ? parts[2].Trim() : null;

            birthdays.Add(new BirthdayInfo
            {
                Name = name,
                Date = date,
                Notes = notes
            });
        }

        return birthdays;
    }
}