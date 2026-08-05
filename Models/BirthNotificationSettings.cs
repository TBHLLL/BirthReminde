using CommunityToolkit.Mvvm.ComponentModel;

namespace BirthReminde.Models;

public partial class BirthNotificationSettings : ObservableObject
{
    [ObservableProperty] private DateTime _notifiedTime;
}