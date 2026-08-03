using CommunityToolkit.Mvvm.ComponentModel;

namespace BirthReminde.Models;

public partial class BirthRemindeComponentSettings : ObservableObject
{
    [ObservableProperty]
    private bool _showNames = true;

    [ObservableProperty]
    private int _maxNameCount = 5;
}