using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Enums.SettingsWindow;

namespace BirthReminde.Views;

[SettingsPageInfo("plug.plugSettingsPage","生日设置页面",SettingsPageCategory.External)]
public partial class SettingsPage : SettingsPageBase
{
    public SettingsPage()
    {
        InitializeComponent();
    }

    private void AddBirthday_Click(object? sender, RoutedEventArgs e)
    {
        
    }
    private void RemoveBirthday_Click(object? sender, RoutedEventArgs e)
    {
        
    }
}