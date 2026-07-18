using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using BirthReminde.Models;
using BirthReminde.Settings;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Enums.SettingsWindow;
using ClassIsland.Shared;

namespace BirthReminde.Views;

[SettingsPageInfo("plug.plugSettingsPage", "生日设置", "\ue4c4", "\ue4c3", SettingsPageCategory.External)]
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
}