using NotificationManagement.Core.Enums;

namespace NotificationManagement.Core.Interfaces;

public interface INotification
{
    /// <summary>The channel this implementation handles. Used by the factory to register it.</summary>
    NotificationType Type { get; }

    /// <summary>Channel-specific receiver validation (email address, phone number, device token...).</summary>
    bool IsValidReceiver(string receiver);

    Task SendAsync(
    string receiver,
    string message,
    CancellationToken cancellationToken = default);
}
