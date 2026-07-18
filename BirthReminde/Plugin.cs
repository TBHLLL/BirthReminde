using ClassIsland.Core;
using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BirthReminde.Views;
using BirthReminde.Settings;
using ClassIsland.Core.Extensions.Registry;

namespace BirthReminde;

[PluginEntrance]
public class Plugin : PluginBase
{
    public override void Initialize(HostBuilderContext context, IServiceCollection services)
    {
        services.AddSingleton<BirthRemindeSettings>();
        services.AddComponent<BirthReminde.Views.birthreminder>();
        services.AddSettingsPage<BirthReminde.Views.SettingsPage>();
    }
}