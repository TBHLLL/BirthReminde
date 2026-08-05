using BirthReminde.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace BirthReminde.Settings;

public partial class BirthRemindeSettings : ObservableObject
{
    public BirthRemindeSettings()
    {
        PropertyChanged += OnSelfPropertyChanged;
    }

    [ObservableProperty]
    private ObservableCollection<BirthdayInfo> _birthdays = new();
    // 生日集合数据保存
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

    [ObservableProperty] 
    private int _remideRange = 7;
}