using NotificationManagement.Core.Enums;

namespace NotificationManagement.Core.Interfaces;

public interface INotificationFactory
{
    INotification Create(NotificationType type);
}
