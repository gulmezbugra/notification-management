using NotificationManagement.Core.Enums;
using NotificationManagement.Core.Exceptions;
using NotificationManagement.Core.Interfaces;

namespace NotificationManagement.Business.Factories;

/// <summary>
/// The single place that decides which INotification implementation handles a given type.
/// There is no if/else or switch: every INotification registered in DI declares its own Type,
/// and the factory simply looks it up. Adding a new channel = add one class (+ one enum value).
/// </summary>
public class NotificationFactory : INotificationFactory
{
    private readonly IReadOnlyDictionary<NotificationType, INotification> _notifications;

    public NotificationFactory(IEnumerable<INotification> notifications)
    {
        _notifications = notifications.ToDictionary(n => n.Type);
    }

    public INotification Create(NotificationType type)
        => _notifications.TryGetValue(type, out var notification)
            ? notification
            : throw new UnsupportedNotificationTypeException(type);
}
