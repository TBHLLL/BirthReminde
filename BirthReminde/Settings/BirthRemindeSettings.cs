using BirthReminde.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Avalonia.Media;

namespace BirthReminde.Settings;

public partial class BirthRemindeSettings : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<BirthdayInfo> _birthdays = new();

    [ObservableProperty]
    private int _fontSize = 14;

    [ObservableProperty]
    private Color _fontColor = Colors.Black;

    [ObservableProperty]
    private bool _isCompactModeEnabled = false;
}