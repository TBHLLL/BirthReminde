using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BirthReminde.Models;
using BirthReminde.Settings;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using ClassIsland.Shared;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using Avalonia.Media;

namespace BirthReminde.Views;

[ComponentInfo(
    "A1B2C3D4-E5F6-7890-ABCD-EF1234567890",
    "生日提醒",
    "\uE837",
    "显示当天或即将到来的生日提醒"
)]
public partial class birthreminder : ComponentBase
{
    public BirthRemindeSettings Settings => IAppHost.GetService<BirthRemindeSettings>();

    private System.Timers.Timer? _timer;
    private List<(BirthdayInfo Info, int DaysUntil)> _upcomingBirthdays = new();
    private int _cycleIndex;
    private int _tickCounter;
    private int _updateCounter;
    private bool _isAnimating;
    private string _lastDisplayText = "";

    public birthreminder()
    {
        InitializeComponent();
        Loaded += OnControlLoaded;
        Unloaded += OnControlUnloaded;
    }
    
    private void OnControlLoaded(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
    {
        Settings.Birthdays.CollectionChanged += OnBirthdaysChanged;
        Settings.PropertyChanged += OnSettingsPropertyChanged;
        RefreshUpcomingList();
        UpdateDisplay();
        StartTimer();
    }

    private void OnBirthdaysChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            RefreshUpcomingList();
            UpdateDisplay();
        });
    }

    private void OnSettingsPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(Settings.FontSize)
            or nameof(Settings.FontColor)
            or nameof(Settings.IsCompactModeEnabled))
        {
            Dispatcher.UIThread.InvokeAsync(ApplyStyle);
        }
    }

    private void StartTimer()
    {
        _timer = new System.Timers.Timer(1000);
        _timer.Elapsed += (_, _) =>
        {
            Dispatcher.UIThread.InvokeAsync(OnTick);
        };
        _timer.Start();
    }

    private void OnTick()
    {
        _tickCounter++;
        _updateCounter++;

        if (_updateCounter >= 60)
        {
            _updateCounter = 0;
            RefreshUpcomingList();
        }

        if (Settings.IsCycleEnabled && _upcomingBirthdays.Count > 1)
        {
            if (_tickCounter >= Settings.CycleIntervalSeconds)
            {
                _tickCounter = 0;
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
        var birthdays = Settings.Birthdays;
        if (birthdays == null || birthdays.Count == 0)
            return;

        var today = DateTime.Today;
        foreach (var b in birthdays)
        {
            var daysUntil = b.GetDaysUntilBirthday();
            if (daysUntil <= 7)
            {
                _upcomingBirthdays.Add((b, daysUntil));
            }
        }

        _upcomingBirthdays = _upcomingBirthdays.OrderBy(x => x.DaysUntil).ToList();

        if (_cycleIndex >= _upcomingBirthdays.Count)
            _cycleIndex = 0;
    }

    private async void UpdateDisplay()
    {
        if (Settings.IsAnimationEnabled)
        {
            await AnimateTextChange();
        }
        else
        {
            SetDisplayText();
        }
    }

    private string GetDisplayText()
    {
        if (_upcomingBirthdays.Count == 0)
            return " 今日无生日 ";

        var (info, daysUntil) = _upcomingBirthdays[_cycleIndex];

        if (daysUntil == 0)
            return $" {info.Name}今天生日！{info.GetAge()}岁 ";
        if (daysUntil == 1)
            return $" {info.Name}明天生日 ";
        return $" {info.Name}还有{daysUntil}天 ";
    }

    private void SetDisplayText()
    {
        var newText = GetDisplayText();
        if (newText == _lastDisplayText && DisplayTextBlock.Text == newText)
            return;
        _lastDisplayText = newText;
        DisplayTextBlock.Text = newText;
        ApplyStyle();
    }

    private async Task AnimateTextChange()
    {
        var newText = GetDisplayText();
        if (newText == _lastDisplayText)
            return;

        if (_isAnimating)
        {
            _lastDisplayText = newText;
            DisplayTextBlock.Text = newText;
            ApplyStyle();
            return;
        }

        _isAnimating = true;
        _lastDisplayText = newText;

        var targetOpacity = DisplayTextBlock.Opacity;

        for (var i = 0; i < 10; i++)
        {
            targetOpacity -= 0.1;
            if (targetOpacity < 0) targetOpacity = 0;
            DisplayTextBlock.Opacity = targetOpacity;
            await Task.Delay(15);
        }

        DisplayTextBlock.Text = newText;
        ApplyStyle();

        for (var i = 0; i < 10; i++)
        {
            targetOpacity += 0.1;
            if (targetOpacity > 1) targetOpacity = 1;
            DisplayTextBlock.Opacity = targetOpacity;
            await Task.Delay(15);
        }

        DisplayTextBlock.Opacity = 1;
        _isAnimating = false;
    }

    private void ApplyStyle()
    {
        if (Settings.IsCompactModeEnabled)
        {
            DisplayTextBlock.FontSize = Settings.FontSize - 2;
            DisplayTextBlock.Foreground = new SolidColorBrush(Settings.FontColor);
        }
        else
        {
            DisplayTextBlock.FontSize = Settings.FontSize;
            DisplayTextBlock.Foreground = new SolidColorBrush(Settings.FontColor);
        }
    }

    private void OnControlUnloaded(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
    {
        _timer?.Stop();
        _timer?.Dispose();
        _timer = null;
        Settings.Birthdays.CollectionChanged -= OnBirthdaysChanged;
        Settings.PropertyChanged -= OnSettingsPropertyChanged;
    }
}