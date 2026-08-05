using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using BirthReminde.Models;
using BirthReminde.Settings;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Core.Abstractions.Services.NotificationProviders;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Models.Notification;
using ClassIsland.Shared;
using Microsoft.Extensions.Hosting;

namespace BirthReminde.Services.Notifications;

[NotificationProviderInfo("29A0DCD2-2C43-AC21-62AF-42D976256DF7", "生日提醒", "\uE8AD", "规则集满足且当天有人过生日时发出提醒")]
public class BirthNotified : NotificationProviderBase<BirthNotificationSettings>, IHostedService
{
    private readonly BirthRemindeSettings _settings;
    private readonly IRulesetService _rulesetService;
    private readonly HashSet<string> _notifiedKeys = new();
    private DateOnly _currentDay;

    public BirthNotified(BirthRemindeSettings settings, IRulesetService rulesetService)
    {
        _settings = settings;
        _rulesetService = rulesetService;
        _rulesetService.StatusUpdated += RulesetServiceOnStatusUpdated;
    }

    public new async Task StartAsync(CancellationToken cancellationToken)
    {
        // 启动时立即检查一次，之后每分钟兜底（覆盖跨天、启动晚于当日等情况）
        Dispatcher.UIThread.Post(TrySendIfDue);
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
            Dispatcher.UIThread.Post(TrySendIfDue);
        }
    }

    public new Task StopAsync(CancellationToken cancellationToken)
    {
        _rulesetService.StatusUpdated -= RulesetServiceOnStatusUpdated;
        return Task.CompletedTask;
    }

    private void RulesetServiceOnStatusUpdated(object? sender, EventArgs e)
    {
        Dispatcher.UIThread.Post(TrySendIfDue);
    }

    private void TrySendIfDue()
    {
        var now = DateTime.Now;
        var today = DateOnly.FromDateTime(now);

        // 跨天时清空已提醒记录
        if (today != _currentDay)
        {
            _notifiedKeys.Clear();
            _currentDay = today;
        }

        var ruleset = Settings.TriggerRuleset;
        // 规则集未配置（没有任何启用且包含有效规则的组）时永不触发
        if (ruleset == null || !IsRulesetConfigured(ruleset) || !_rulesetService.IsRulesetSatisfied(ruleset))
            return;

        // 按“月/日”比较，天然规避 2 月 29 日在平年的构造异常
        var todayBirthdays = _settings.Birthdays
            .Where(b => b.Date.Month == now.Month && b.Date.Day == now.Day)
            .ToList();
        if (todayBirthdays.Count == 0)
            return;

        var keys = todayBirthdays
            .Select(b => $"{today:yyyy-MM-dd}|{b.Name}")
            .ToList();
        if (keys.All(_notifiedKeys.Contains))
            return;

        var names = string.Join("、", todayBirthdays.Select(b => b.Name));
        var detail = string.Join("、", todayBirthdays.Select(b => $"{b.Name}：{b.GetAge()} 岁"));
        var overstring = "";
        if (Settings.IsShowAge)
        {
            overstring = $"今天是 {names} 的生日🎂🎂🎂!   {detail}";
        }
        else
        {
            overstring = $"今天是 {names} 的生日🎂🎂🎂!";
        }
        
        ShowNotification(new NotificationRequest
        {
            MaskContent = NotificationContent.CreateTwoIconsMask("生日快乐！"),
            OverlayContent = NotificationContent.CreateSimpleTextContent(
                overstring,
                x => x.Duration = TimeSpan.FromSeconds(10))
        });

        foreach (var key in keys)
        {
            _notifiedKeys.Add(key);
        }
    }

    private bool IsRulesetConfigured(ClassIsland.Core.Models.Ruleset.Ruleset ruleset)
    {
        return ruleset.Groups.Any(g =>
            g.IsEnabled && g.Rules.Any(r => !string.IsNullOrEmpty(r.Id)));
    }
}
