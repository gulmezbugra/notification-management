using Microsoft.Extensions.Logging;
using NotificationManagement.Core.Enums;

namespace NotificationManagement.Business.Notifications;

public class PushNotification : NotificationBase
{
    public PushNotification(ILogger<PushNotification> logger) : base(logger) { }

    public override NotificationType Type => NotificationType.Push;
    protected override string ChannelName => "Push";

    // Receiver is a device token: at least 8 characters, no whitespace.
    public override bool IsValidReceiver(string receiver)
        => receiver.Length >= 8 && !receiver.Any(char.IsWhiteSpace);
}
