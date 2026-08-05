using System.Collections.Specialized;
using System.ComponentModel;
using Avalonia.Interactivity;
using Avalonia.Threading;
using BirthReminde.Models;
using BirthReminde.Settings;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using ClassIsland.Shared;
using Timer = System.Timers.Timer;

namespace BirthReminde.Views.Components;

[ComponentInfo(
    "FB677B13-A657-F6B1-DAEB-7AA8FD6655BD",
    "生日提醒",
    "\uE8AD",
    "显示当天或即将到来的生日提醒"
)]
public partial class birthreminder : ComponentBase<BirthRemindeComponentSettings>
{
    private readonly HashSet<BirthdayInfo> _subscribedBirthdays = new();
    private int _cycleIndex;
    private bool _isAnimating;

    private DateTime _lastCycleTime;
    private string _lastDisplayKey = "";
    private DateTime _lastRefreshTime;
    private Timer? _timer;
    private List<(BirthdayInfo Info, int DaysUntil)> _upcomingBirthdays = new();

    public birthreminder()
    {
        InitializeComponent();
        Loaded += OnControlLoaded;
        Unloaded += OnControlUnloaded;
    }

    public BirthRemindeSettings GlobalSettings => IAppHost.GetService<BirthRemindeSettings>();

    private void OnControlLoaded(object? sender, RoutedEventArgs e)
    {
        GlobalSettings.Birthdays.CollectionChanged += OnBirthdaysChanged;
        GlobalSettings.PropertyChanged += OnGlobalSettingsPropertyChanged;
        Settings.PropertyChanged += OnSettingsPropertyChanged;
        ResyncBirthdaySubscriptions();
        RefreshUpcomingList();
        UpdateDisplay();
        _lastCycleTime = DateTime.Now;
        _lastRefreshTime = DateTime.Now;
        StartTimer();
    }

    private void OnBirthdaysChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            ResyncBirthdaySubscriptions();
            RefreshUpcomingList();
            UpdateDisplay();
        });
    }

    private void ResyncBirthdaySubscriptions()
    {
        foreach (var birthday in _subscribedBirthdays) birthday.PropertyChanged -= OnBirthdayPropertyChanged;
        _subscribedBirthdays.Clear();

        foreach (var birthday in GlobalSettings.Birthdays)
        {
            birthday.PropertyChanged += OnBirthdayPropertyChanged;
            _subscribedBirthdays.Add(birthday);
        }
    }

    private void OnBirthdayPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            RefreshUpcomingList();
            UpdateDisplay();
        });
    }

    private void OnGlobalSettingsPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // 提醒范围变更时刷新列表与显示
        if (e.PropertyName == nameof(GlobalSettings.RemideRange))
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                RefreshUpcomingList();
                UpdateDisplay();
            });
    }

    private void OnSettingsPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // 字体大小/颜色由 ClassIsland 高级设置管理，显示开关由 XAML 绑定处理
    }

    private void StartTimer()
    {
        _timer = new Timer(100);
        _timer.Elapsed += (_, _) => { Dispatcher.UIThread.InvokeAsync(OnTick); };
        _timer.Start();
    }

    private void OnTick()
    {
        var now = DateTime.Now;
        if ((now - _lastRefreshTime).TotalSeconds >= 60)
        {
            _lastRefreshTime = now;
            RefreshUpcomingList();
        }

        if (Settings.IsCycleEnabled && _upcomingBirthdays.Count > 1)
        {
            var intervalSeconds = Math.Max(0.1, Settings.CycleIntervalSeconds);
            if ((now - _lastCycleTime).TotalSeconds >= intervalSeconds)
            {
                _lastCycleTime = now;
                _cycleIndex = (_cycleIndex + 1) % _upcomingBirthdays.Count;
                UpdateDisplay();
            }
        }
        else
        {
            UpdateDisplay();
        }
    }

    private void RefreshUpcomingList()
    {
        _upcomingBirthdays.Clear();
        var birthdays = GlobalSettings.Birthdays;
        if (birthdays == null || birthdays.Count == 0)
            return;

        var range = Math.Max(0, GlobalSettings.RemideRange);
        foreach (var b in birthdays)
        {
            var daysUntil = b.GetDaysUntilBirthday();
            if (daysUntil <= range) _upcomingBirthdays.Add((b, daysUntil));
        }

        _upcomingBirthdays = _upcomingBirthdays.OrderBy(x => x.DaysUntil).ToList();

        if (_cycleIndex >= _upcomingBirthdays.Count)
            _cycleIndex = 0;
    }

    /// <summary>
    ///     生成当前显示状态的唯一标识，用于检测内容是否变化
    /// </summary>
    private string GetDisplayKey()
    {
        if (_upcomingBirthdays.Count == 0)
            return "empty";

        var (info, daysUntil) = _upcomingBirthdays[_cycleIndex];
        return $"{info.Name}|{daysUntil}|{info.GetAge()}";
    }

    /// <summary>
    ///     将姓名、提醒文本、年龄写入组件设置，由 XAML 绑定自动更新 UI
    /// </summary>
    private void UpdateDisplayContent()
    {
        if (_upcomingBirthdays.Count == 0)
        {
            Settings.DisplayTextBlock = "";
            Settings.SubDisplayTextBlock = "近期无生日";
            Settings.AgeTextBlock = "";
            return;
        }

        var (info, daysUntil) = _upcomingBirthdays[_cycleIndex];
        Settings.DisplayTextBlock = info.Name;

        if (daysUntil == 0)
        {
            Settings.SubDisplayTextBlock = "今天生日！🎂🎂🎂";
            Settings.AgeTextBlock = $"{info.GetAge()}岁";
        }
        else if (daysUntil == 1)
        {
            Settings.SubDisplayTextBlock = "明天生日";
            Settings.AgeTextBlock = $"{info.GetAge() + 1}岁";
        }
        else
        {
            Settings.SubDisplayTextBlock = $"还有{daysUntil}天";
            Settings.AgeTextBlock = $"{info.GetAge() + 1}岁";
        }
    }

    private async void UpdateDisplay()
    {
        var key = GetDisplayKey();
        if (key == _lastDisplayKey)
            return;

        if (Settings.IsAnimationEnabled)
        {
            await AnimateTextChange(key);
        }
        else
        {
            _lastDisplayKey = key;
            UpdateDisplayContent();
        }
    }

    private async Task AnimateTextChange(string newKey)
    {
        if (_isAnimating)
        {
            // 动画进行中，直接更新内容
            _lastDisplayKey = newKey;
            UpdateDisplayContent();
            return;
        }

        _isAnimating = true;
        _lastDisplayKey = newKey;

        // 淡出
        var opacity = CircleProgressRoot.Opacity;
        for (var i = 0; i < 10; i++)
        {
            opacity -= 0.1;
            if (opacity < 0) opacity = 0;
            CircleProgressRoot.Opacity = opacity;
            await Task.Delay(15);
        }

        UpdateDisplayContent();

        // 淡入
        for (var i = 0; i < 10; i++)
        {
            opacity += 0.1;
            if (opacity > 1) opacity = 1;
            CircleProgressRoot.Opacity = opacity;
            await Task.Delay(15);
        }

        CircleProgressRoot.Opacity = 1;
        _isAnimating = false;
    }

    private void OnControlUnloaded(object? sender, RoutedEventArgs e)
    {
        _timer?.Stop();
        _timer?.Dispose();
        _timer = null;
        foreach (var birthday in _subscribedBirthdays) birthday.PropertyChanged -= OnBirthdayPropertyChanged;
        _subscribedBirthdays.Clear();
        GlobalSettings.Birthdays.CollectionChanged -= OnBirthdaysChanged;
        GlobalSettings.PropertyChanged -= OnGlobalSettingsPropertyChanged;
        Settings.PropertyChanged -= OnSettingsPropertyChanged;
    }
}