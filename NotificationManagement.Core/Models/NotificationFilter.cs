using NotificationManagement.Core.Enums;

namespace NotificationManagement.Core.Models;

public class NotificationFilter
{
    public string? Search { get; set; }
    public NotificationType? Type { get; set; }
    public NotificationStatus? Status { get; set; }

    /// <summary>True = newest first (default).</summary>
    public bool NewestFirst { get; set; } = true;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
