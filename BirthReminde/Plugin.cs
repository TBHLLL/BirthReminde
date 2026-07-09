using ClassIsland.Core;
using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BirthReminde.Views;
using ClassIsland.Core.Extensions.Registry;
using ClassIsland.Shared.Helpers;
// using BirthReminde.Models;
// using BirthReminde.Services;

namespace BirthReminde;

[PluginEntrance]
public class Plugin : PluginBase
{
    public override void Initialize(HostBuilderContext context, IServiceCollection services)
    {
        services.AddComponent<BirthReminde.Views.birthreminder>();
        services.AddSettingsPage<BirthReminde.Views.SettingsPage>();
        AppBase.Current.AppStarted += async (_, _) =>
            await CommonTaskDialogs.ShowDialog("Hello world!", "Hello from BirthReminde!");
    }
}