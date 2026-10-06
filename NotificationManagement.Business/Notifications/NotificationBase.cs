using Microsoft.Extensions.Logging;
using NotificationManagement.Core.Enums;
using NotificationManagement.Core.Exceptions;
using NotificationManagement.Core.Interfaces;

namespace NotificationManagement.Business.Notifications;

/// <summary>
/// Shared mock behaviour for the initial version (no real providers yet).
/// Include the text "[fail]" in a message to simulate a provider error.
/// When a real provider is integrated, only the concrete class's SendAsync changes.
/// </summary>
public abstract class NotificationBase : INotification
{
    private const string SimulatedFailureTag = "[fail]";
    private readonly ILogger _logger;

    protected NotificationBase(ILogger logger) => _logger = logger;

    public abstract NotificationType Type { get; }
    protected abstract string ChannelName { get; }
    public abstract bool IsValidReceiver(string receiver);

    public virtual async Task SendAsync(
     string receiver,
     string message,
     CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "[{Channel}] Notification sending started.",
            ChannelName);

        await Task.Delay(250, cancellationToken);

        if (message.Contains(
            SimulatedFailureTag,
            StringComparison.OrdinalIgnoreCase))
        {
            throw new NotificationSendException(
                $"The {ChannelName} provider rejected the message.");
        }

        _logger.LogInformation(
            "[{Channel}] Notification delivered successfully.",
            ChannelName);
    }
}
