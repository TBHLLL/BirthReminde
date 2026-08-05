using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BirthReminde.Models;
using ClassIsland.Core.Abstractions.Controls;

namespace BirthReminde.Views;

public partial class BirthNotificationProviderSettings : NotificationProviderControlBase<BirthNotificationSettings>
{
    public BirthNotificationProviderSettings()
    {
        InitializeComponent();
    }
}