using NotificationManagement.Core.Enums;

namespace NotificationManagement.Mvc.ViewModels;

/// <summary>Presentation-only helpers (labels, icons, CSS classes). No business logic.</summary>
public static class NotificationDisplayExtensions
{
    public static string Label(this NotificationType type) => type switch
    {
        NotificationType.Email => "Email",
        NotificationType.Sms => "SMS",
        NotificationType.WhatsApp => "WhatsApp",
        NotificationType.Push => "Push",
        _ => type.ToString()
    };

    public static string Icon(this NotificationType type) => type switch
    {
        NotificationType.Email => "bi-envelope-fill",
        NotificationType.Sms => "bi-chat-dots-fill",
        NotificationType.WhatsApp => "bi-whatsapp",
        NotificationType.Push => "bi-phone-vibrate-fill",
        _ => "bi-bell-fill"
    };

    public static string IconCss(this NotificationType type) => type switch
    {
        NotificationType.Email => "type-email",
        NotificationType.Sms => "type-sms",
        NotificationType.WhatsApp => "type-whatsapp",
        NotificationType.Push => "type-push",
        _ => "type-email"
    };

    public static string BadgeClass(this NotificationStatus status) => status switch
    {
        NotificationStatus.Sent => "badge-sent",
        NotificationStatus.Pending => "badge-pending",
        NotificationStatus.Failed => "badge-failed",
        _ => "bg-secondary"
    };

    public static string StatusIcon(this NotificationStatus status) => status switch
    {
        NotificationStatus.Sent => "bi-check-circle-fill",
        NotificationStatus.Pending => "bi-hourglass-split",
        NotificationStatus.Failed => "bi-x-circle-fill",
        _ => "bi-circle"
    };
}
