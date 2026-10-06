using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using NotificationManagement.Core.Enums;

namespace NotificationManagement.Mvc.ViewModels;

public class CreateNotificationViewModel
{
    [Required(ErrorMessage = "Please select a notification type.")]
    [Display(Name = "Notification type")]
    public NotificationType? Type { get; set; }

    [Required(ErrorMessage = "Receiver is required.")]
    [StringLength(256, ErrorMessage = "Receiver cannot exceed 256 characters.")]
    public string Receiver { get; set; } = string.Empty;

    [Required(ErrorMessage = "Message cannot be empty.")]
    [StringLength(1000, ErrorMessage = "Message cannot exceed 1000 characters.")]
    public string Message { get; set; } = string.Empty;

    // Read-only; not model-bound.
    public IEnumerable<SelectListItem> TypeOptions =>
        Enum.GetValues<NotificationType>()
            .Select(t => new SelectListItem(t.Label(), t.ToString()));
}
