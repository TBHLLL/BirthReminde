using BirthReminde.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Avalonia.Media;

namespace BirthReminde.Settings;

public partial class BirthRemindeSettings : ObservableObject
{
    public BirthRemindeSettings()
    {
        PropertyChanged += OnSelfPropertyChanged;
    }

    [ObservableProperty]
    private ObservableCollection<BirthdayInfo> _birthdays = new();

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

    partial void OnBirthdaysChanged(ObservableCollection<BirthdayInfo> value)
    {
        if (value != null)
            value.CollectionChanged += OnCollectionChanged;
    }

    private void OnSelfPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(Birthdays))
            Plugin.SaveSettings(this);
    }

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Plugin.SaveSettings(this);
    }

    public void Save()
    {
        Plugin.SaveSettings(this);
    }
}