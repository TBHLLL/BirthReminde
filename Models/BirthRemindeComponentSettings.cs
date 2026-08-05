using CommunityToolkit.Mvvm.ComponentModel;

namespace BirthReminde.Models;

public partial class BirthRemindeComponentSettings : ObservableObject
{
    [ObservableProperty]
    private bool _showNames = true;

    [ObservableProperty]
    private int _maxNameCount = 5;

    // 字体大小与颜色由 ClassIsland 高级设置管理，此处不再维护
    [ObservableProperty]
    private bool _isCompactModeEnabled = true;

    [ObservableProperty]
    private bool _isCycleEnabled = false;

    [ObservableProperty]
    private double _cycleIntervalSeconds = 5.0;

    [ObservableProperty]
    private bool _isAnimationEnabled = true;

    [ObservableProperty] 
    private bool _isShowPersonAge = true;

    [ObservableProperty] private string _displayTextBlock = "";
    [ObservableProperty] private string _subDisplayTextBlock = "近期无生日";
    [ObservableProperty] private string _ageTextBlock = "";
}