using Microsoft.EntityFrameworkCore;
using NotificationManagement.Core.Entities;
using NotificationManagement.Core.Enums;
using NotificationManagement.Core.Interfaces;
using NotificationManagement.Core.Models;
using NotificationManagement.Data.Context;

namespace NotificationManagement.Data.Repositories;

public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
{
    public NotificationRepository(ApplicationDbContext context) : base(context) { }


    public async Task<(IReadOnlyList<Notification> Items, int TotalCount)>
        GetFilteredAsync(
            NotificationFilter filter,
            CancellationToken cancellationToken = default)
    {
        IQueryable<Notification> query = DbSet.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var term = filter.Search.Trim();

            query = query.Where(n =>
                n.Receiver.Contains(term));
        }

        if (filter.Type.HasValue)
        {
            query = query.Where(n =>
                n.NotificationType == filter.Type.Value);
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(n =>
                n.Status == filter.Status.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        query = filter.NewestFirst
            ? query.OrderByDescending(n => n.CreatedAt)
            : query.OrderBy(n => n.CreatedAt);

        var pageNumber = Math.Max(1, filter.PageNumber);
        var pageSize = Math.Clamp(filter.PageSize, 1, 100);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<Notification>> GetRecentAsync(
        int count, CancellationToken cancellationToken = default)
        => await DbSet.AsNoTracking()
            .OrderByDescending(n => n.CreatedAt)
            .Take(count)
            .ToListAsync(cancellationToken);

    public async Task<NotificationStats> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        var counts = await DbSet.AsNoTracking()
            .GroupBy(n => n.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        int Get(NotificationStatus s) => counts.FirstOrDefault(c => c.Status == s)?.Count ?? 0;

        var sent = Get(NotificationStatus.Sent);
        var failed = Get(NotificationStatus.Failed);
        var pending = Get(NotificationStatus.Pending);

        return new NotificationStats(sent + failed + pending, sent, failed, pending);
    }
}
