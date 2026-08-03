using Avalonia.Markup.Xaml;
using BirthReminde.Models;
using ClassIsland.Core.Abstractions.Controls;

namespace BirthReminde.Views.Components;

public partial class BirthdayTodayComponentSettingsControl : ComponentBase<BirthRemindeComponentSettings>
{
    public BirthdayTodayComponentSettingsControl()
    {
        InitializeComponent();
        Loaded += (_, _) => DataContext = Settings;
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}