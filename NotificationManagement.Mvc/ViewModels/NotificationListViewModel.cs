
using NotificationManagement.Core.Enums;

namespace NotificationManagement.Mvc.ViewModels;

public class NotificationListViewModel
{
    public List<NotificationItemViewModel> Items { get; set; } = new();

    public string? Search { get; set; }
    public NotificationType? Type { get; set; }
    public NotificationStatus? Status { get; set; }

    public string Sort { get; set; } = "date_desc";

    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalCount { get; set; }

    public int TotalPages =>
        (int)Math.Ceiling((double)TotalCount / PageSize);

    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
}