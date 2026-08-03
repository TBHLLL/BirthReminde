using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia.Media;
namespace BirthReminde.Settings;

public partial class BirthRemideComponentSettings : ObservableObject
{
    [ObservableProperty]
    private bool _showNames = true;

    [ObservableProperty]
    private int _maxNameCount = 5;

    // 从 BirthRemindeSettings 迁移过来的显示设置
    [ObservableProperty]
    private int _fontSize = 20;

    [ObservableProperty]
    private Color _fontColor = Colors.White;

    [ObservableProperty]
    private bool _isCompactModeEnabled = true;

    [ObservableProperty]
    private bool _isCycleEnabled = false;

    [ObservableProperty]
    private int _cycleIntervalSeconds = 3;

    [ObservableProperty]
    private bool _isAnimationEnabled = true;
}