
using NotificationManagement.Core.Entities;
using NotificationManagement.Core.Models;

namespace NotificationManagement.Core.Interfaces;

public interface INotificationRepository
    : IGenericRepository<Notification>
{
    Task<(IReadOnlyList<Notification> Items, int TotalCount)>
        GetFilteredAsync(
            NotificationFilter filter,
            CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Notification>> GetRecentAsync(
        int count,
        CancellationToken cancellationToken = default);

    Task<NotificationStats> GetStatsAsync(
        CancellationToken cancellationToken = default);
}