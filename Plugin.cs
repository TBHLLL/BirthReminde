using ClassIsland.Core;
using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BirthReminde.Views;
using BirthReminde.Settings;
using ClassIsland.Core.Extensions.Registry;
using ClassIsland.Shared.Helpers;
using System.Text;
using birthreminder = BirthReminde.Views.Components.birthreminder;

namespace BirthReminde;

[PluginEntrance]
public class Plugin : PluginBase
{
    public static Plugin? Instance { get; private set; }

    public override void Initialize(HostBuilderContext context, IServiceCollection services)
    {
        Instance = this;

        // 注册 GB18030/GBK 等代码页支持，供 CSV 导入识别中文编码
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        var settings = LoadSettings();
        services.AddSingleton(settings);

        services.AddComponent<birthreminder, BirthReminde.Views.Components.BirthdayTodayComponentSettingsControl>();
        services.AddSettingsPage<BirthReminde.Views.SettingsPage>();
    }

    public static BirthRemindeSettings LoadSettings()
    {
        if (Instance == null)
            return new BirthRemindeSettings();

        var path = System.IO.Path.Combine(Instance.PluginConfigFolder, "Settings.json");
        return ConfigureFileHelper.LoadConfig<BirthRemindeSettings>(path);
    }

    public static void SaveSettings(BirthRemindeSettings settings)
    {
        if (Instance == null)
            return;

        var path = System.IO.Path.Combine(Instance.PluginConfigFolder, "Settings.json");
        ConfigureFileHelper.SaveConfig(path, settings);
    }
}
