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
using System.Linq;
using Avalonia.Threading;

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

    public birthreminder()
    {
        InitializeComponent();
    }
}