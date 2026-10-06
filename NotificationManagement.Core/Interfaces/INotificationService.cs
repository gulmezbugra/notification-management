
using NotificationManagement.Core.Entities;
using NotificationManagement.Core.Models;

namespace NotificationManagement.Core.Interfaces;

public interface INotificationService
{
    Task<Notification> SendNotificationAsync(
        CreateNotificationDto dto,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Notification> Items, int TotalCount)>
        GetNotificationsAsync(
            NotificationFilter filter,
            CancellationToken cancellationToken = default);

    Task<Notification> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<DashboardData> GetDashboardAsync(
        int recentCount = 8,
        CancellationToken cancellationToken = default);
}