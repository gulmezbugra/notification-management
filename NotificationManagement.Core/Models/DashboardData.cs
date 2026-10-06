using NotificationManagement.Core.Entities;

namespace NotificationManagement.Core.Models;

public record DashboardData(NotificationStats Stats, IReadOnlyList<Notification> Recent);
