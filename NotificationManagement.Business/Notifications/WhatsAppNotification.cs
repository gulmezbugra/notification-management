using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using NotificationManagement.Core.Enums;

namespace NotificationManagement.Business.Notifications;

public partial class WhatsAppNotification : NotificationBase
{
    public WhatsAppNotification(ILogger<WhatsAppNotification> logger) : base(logger) { }

    public override NotificationType Type => NotificationType.WhatsApp;
    protected override string ChannelName => "WhatsApp";

    public override bool IsValidReceiver(string receiver)
        => PhoneRegex().IsMatch(receiver.Replace(" ", "").Replace("-", ""));

    [GeneratedRegex(@"^\+?[0-9]{7,15}$")]
    private static partial Regex PhoneRegex();
}
