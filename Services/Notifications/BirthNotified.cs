using ClassIsland.Core.Abstractions.Services.NotificationProviders;
using ClassIsland.Core.Attributes;
using BirthReminde.Models;

namespace BirthReminde.Services.Notifications;

[NotificationProviderInfo("29A0DCD2-2C43-AC21-62AF-42D976256DF7","生日提醒","\uE8AD","提供当天过生的人在特定时间段的生日提醒")]
public class BirthNotified : NotificationProviderBase<BirthNotificationSettings>
{
    
}