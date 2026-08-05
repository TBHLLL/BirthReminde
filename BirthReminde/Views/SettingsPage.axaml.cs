using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using BirthReminde.Models;
using BirthReminde.Settings;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Enums.SettingsWindow;
using ClassIsland.Shared;
using ClassIsland.Core.Helpers.UI;
using FluentAvalonia.UI.Controls;
using Avalonia.Media;
using System;
using System.Collections.Generic;
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

    private void BirthdaysListBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var count = BirthdaysListBox.SelectedItems?.Count ?? 0;
        DeleteSelectedButton.Content = count > 0 ? $"删除选中({count})" : "删除选中";
    }

    private void RemoveSelected_Click(object? sender, RoutedEventArgs e)
    {
        var selected = BirthdaysListBox.SelectedItems?.Cast<BirthdayInfo>().ToList() ?? new List<BirthdayInfo>();
        if (selected.Count == 0)
        {
            this.ShowWarningToast("请先选择要删除的生日");
            return;
        }

        foreach (var birthday in selected)
        {
            Settings.Birthdays.Remove(birthday);
        }
        this.ShowSuccessToast($"已删除 {selected.Count} 条生日");
    }

    private void EditBirthday_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { DataContext: BirthdayInfo birthday })
        {
            _ = ShowEditDialog(birthday);
        }
    }

    private void BirthdaysListBox_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        var source = e.Source as Control;
        while (source != null)
        {
            if (source.DataContext is BirthdayInfo birthday)
            {
                _ = ShowEditDialog(birthday);
                return;
            }
            source = source.Parent as Control;
        }
    }

    private async Task ShowEditDialog(BirthdayInfo birthday)
    {
        var nameBox = new TextBox { Text = birthday.Name, Watermark = "姓名", MinWidth = 220 };
        var datePicker = new DatePicker { SelectedDate = birthday.Date };
        var notesBox = new TextBox { Text = birthday.Notes, Watermark = "备注（可选）", MinWidth = 220 };

        var panel = new StackPanel
        {
            Spacing = 8,
            Children =
            {
                new TextBlock { Text = "姓名" },
                nameBox,
                new TextBlock { Text = "生日" },
                datePicker,
                new TextBlock { Text = "备注（可选）" },
                notesBox
            }
        };

        var dialog = new ContentDialog
        {
            Title = "编辑生日",
            Content = panel,
            PrimaryButtonText = "保存",
            CloseButtonText = "取消",
            DefaultButton = ContentDialogButton.Primary
        };

        var dialogResult = await dialog.ShowAsync(TopLevel.GetTopLevel(this));
        if (dialogResult != ContentDialogResult.Primary)
            return;

        var newName = nameBox.Text?.Trim();
        if (string.IsNullOrEmpty(newName))
        {
            this.ShowWarningToast("姓名不能为空");
            return;
        }
        if (!datePicker.SelectedDate.HasValue)
        {
            this.ShowWarningToast("请选择生日日期");
            return;
        }

        // 名字 = 唯一键：不允许改成与其他条目同名
        var hasConflict = Settings.Birthdays.Any(x =>
            !ReferenceEquals(x, birthday) &&
            string.Equals(x.Name.Trim(), newName, StringComparison.OrdinalIgnoreCase));
        if (hasConflict)
        {
            this.ShowWarningToast($"已存在名为“{newName}”的生日，请使用其他名字");
            return;
        }

        birthday.Name = newName;
        birthday.Date = datePicker.SelectedDate.Value.LocalDateTime;
        birthday.Notes = string.IsNullOrWhiteSpace(notesBox.Text) ? null : notesBox.Text.Trim();
        Settings.Save();
        this.ShowSuccessToast("已保存修改");
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

    private async void ImportCsv_Click(object? sender, RoutedEventArgs e)
    {
        var filePath = CsvFilePathTextBox.Text?.Trim();
        if (string.IsNullOrEmpty(filePath))
            return;

        var result = ImortCSV.AnalyzeImport(filePath, Settings.Birthdays);
        if (result.AllBirthdays.Count == 0)
        {
            this.ShowWarningToast("没有解析到有效的生日数据，请检查 CSV 格式");
            return;
        }

        if (result.Duplicates.Count == 0)
        {
            foreach (var birthday in result.AllBirthdays)
            {
                Settings.Birthdays.Add(birthday);
            }
            this.ShowSuccessToast($"已新增 {result.AllBirthdays.Count} 条生日");
            return;
        }

        var duplicateNames = result.Duplicates
            .Select(x => x.Name.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(10)
            .ToList();
        var nameText = string.Join("、", duplicateNames) + (result.Duplicates.Count > 10 ? " 等" : "");

        var panel = new StackPanel
        {
            Spacing = 6,
            Children =
            {
                new TextBlock
                {
                    Text = $"文件中有 {result.Duplicates.Count} 条与现有列表重名（{nameText}），请选择处理方式：",
                    TextWrapping = TextWrapping.Wrap
                },
                new TextBlock
                {
                    Text = "覆盖已有：更新同名条目的日期和备注（名字保持不变）",
                    Opacity = 0.7,
                    TextWrapping = TextWrapping.Wrap
                },
                new TextBlock
                {
                    Text = "跳过重复：只导入文件中不重名的条目",
                    Opacity = 0.7,
                    TextWrapping = TextWrapping.Wrap
                },
                new TextBlock
                {
                    Text = "全部新增：保留文件中的全部条目，允许重名",
                    Opacity = 0.7,
                    TextWrapping = TextWrapping.Wrap
                }
            }
        };

        var dialog = new ContentDialog
        {
            Title = "发现重复名字",
            Content = panel,
            PrimaryButtonText = "覆盖已有",
            SecondaryButtonText = "跳过重复",
            CloseButtonText = "全部新增",
            DefaultButton = ContentDialogButton.Primary
        };

        var choice = await dialog.ShowAsync(TopLevel.GetTopLevel(this));
        switch (choice)
        {
            case ContentDialogResult.Primary: // 覆盖已有
                foreach (var duplicate in result.Duplicates)
                {
                    var target = Settings.Birthdays.FirstOrDefault(x =>
                        string.Equals(x.Name.Trim(), duplicate.Name.Trim(), StringComparison.OrdinalIgnoreCase));
                    if (target != null)
                    {
                        target.Date = duplicate.Date;
                        target.Notes = duplicate.Notes;
                    }
                }
                foreach (var birthday in result.NewBirthdays)
                {
                    Settings.Birthdays.Add(birthday);
                }
                Settings.Save();
                this.ShowSuccessToast($"已覆盖 {result.Duplicates.Count} 条，新增 {result.NewBirthdays.Count} 条");
                break;
            case ContentDialogResult.Secondary: // 跳过重复
                foreach (var birthday in result.NewBirthdays)
                {
                    Settings.Birthdays.Add(birthday);
                }
                this.ShowSuccessToast($"已新增 {result.NewBirthdays.Count} 条，跳过 {result.Duplicates.Count} 条重复");
                break;
            default: // 全部新增
                foreach (var birthday in result.AllBirthdays)
                {
                    Settings.Birthdays.Add(birthday);
                }
                this.ShowSuccessToast($"已新增 {result.AllBirthdays.Count} 条（含重复）");
                break;
        }
    }
}
