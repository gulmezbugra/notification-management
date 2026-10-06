using NotificationManagement.Core.Enums;

namespace NotificationManagement.Core.Exceptions;

public class UnsupportedNotificationTypeException : Exception
{
    public UnsupportedNotificationTypeException(NotificationType type)
        : base($"Notification type '{type}' is not supported.") { }
}
