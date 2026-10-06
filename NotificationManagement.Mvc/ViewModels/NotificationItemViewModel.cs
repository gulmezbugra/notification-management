using NotificationManagement.Core.Entities;
using NotificationManagement.Core.Enums;

namespace NotificationManagement.Mvc.ViewModels;

public class NotificationItemViewModel
{
    public int Id { get; set; }
    public string Receiver { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public NotificationStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? SentAt { get; set; }
    public string? ErrorMessage { get; set; }
}

public static class NotificationMappings
{
    // Dates are stored in UTC; convert to the server's local time for display.
    public static NotificationItemViewModel ToViewModel(this Notification n) => new()
    {
        Id = n.Id,
        Receiver = n.Receiver,
        Message = n.Message,
        Type = n.NotificationType,
        Status = n.Status,
        CreatedAt = n.CreatedAt.ToLocalTime(),
        SentAt = n.SentAt?.ToLocalTime(),
        ErrorMessage = n.ErrorMessage
    };
}
