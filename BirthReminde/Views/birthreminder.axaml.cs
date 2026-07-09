using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;

namespace BirthReminde.Views;

[ComponentInfo(
    "A1B2C3D4-E5F6-7890-ABCD-EF1234567890",
    "生日提醒",
    "\uE837",
    "显示当天过生日的人，支持通过文件添加生日信息"
)]
public partial class birthreminder : ComponentBase
{
    public birthreminder()
    {
        InitializeComponent();
    }
}