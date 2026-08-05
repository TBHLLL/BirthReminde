using ClassIsland.Core.Models.Ruleset;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BirthReminde.Models;

public partial class BirthNotificationSettings : ObservableObject
{
    [ObservableProperty] private DateTime _notifiedTime;
    [ObservableProperty]
    private Ruleset _triggerRuleset = new();
    [ObservableProperty] private bool _isShowAge = false;
}