using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Enums.SettingsWindow;

namespace BirthReminde.Views;

[SettingsPageInfo("plug.plugSettingsPage","生日设置页面","\ue4c4", "\ue4c3",SettingsPageCategory.External)]
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

    private void ButtonSyncTimeNow_OnClick(object? sender, RoutedEventArgs e)
    {
        
    }
    private void ButtonAdjustTime_OnClick(object? sender, RoutedEventArgs e)
    {
        
    }
}