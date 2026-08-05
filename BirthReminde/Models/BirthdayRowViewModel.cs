using CommunityToolkit.Mvvm.ComponentModel;

namespace BirthReminde.Models;

/// <summary>
/// 生日列表表格的行包装模型：持有生日数据与页面级勾选状态（勾选状态不持久化）。
/// </summary>
public partial class BirthdayRowViewModel : ObservableObject
{
    public BirthdayRowViewModel(BirthdayInfo item)
    {
        Item = item;
    }

    public BirthdayInfo Item { get; }

    [ObservableProperty]
    private bool _isChecked;
}
