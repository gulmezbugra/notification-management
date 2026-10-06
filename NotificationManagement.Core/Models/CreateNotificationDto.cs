using NotificationManagement.Core.Enums;

namespace NotificationManagement.Core.Models;

public class CreateNotificationDto
{
    public string Receiver { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
}
