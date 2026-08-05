using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using BirthReminde.Models;
using BirthReminde.Settings;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Enums.SettingsWindow;
using ClassIsland.Shared;
using System;
using System.Linq;

namespace BirthReminde.Views;

[SettingsPageInfo("plug.plugSettingsPage", "生日设置", "\uE8AD", "\uE8AC", SettingsPageCategory.External)]
public partial class SettingsPage : SettingsPageBase
{
    public SettingsPage()
    {
        InitializeComponent();
        DataContext = this;
    }

    public BirthRemindeSettings Settings => IAppHost.GetService<BirthRemindeSettings>();

    private void AddBirthday_Click(object? sender, RoutedEventArgs e)
    {
        var name = NameTextBox.Text?.Trim();
        var date = DatePicker.SelectedDate;

        if (!string.IsNullOrEmpty(name) && date.HasValue)
        {
            Settings.Birthdays.Add(new BirthdayInfo
            {
                Name = name,
                Date = date.Value.LocalDateTime
            });

            NameTextBox.Text = string.Empty;
            DatePicker.SelectedDate = null;
        }
    }

    private void RemoveBirthday_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is BirthdayInfo birthday)
        {
            Settings.Birthdays.Remove(birthday);
        }
    }

    private async void SelectCsvFile_Click(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null)
            return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "选择CSV文件",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("CSV文件")
                {
                    Patterns = new[] { "*.csv" }
                }
            }
        });

        var file = files?.FirstOrDefault();
        if (file != null)
        {
            CsvFilePathTextBox.Text = file.Path.LocalPath;
        }
    }

    private void ImportCsv_Click(object? sender, RoutedEventArgs e)
    {
        var filePath = CsvFilePathTextBox.Text?.Trim();
        if (string.IsNullOrEmpty(filePath))
            return;

        var imported = ImortCSV.ImportFromFile(filePath);
        if (imported.Count == 0)
            return;

        foreach (var birthday in imported)
        {
            Settings.Birthdays.Add(birthday);
        }
    }
}