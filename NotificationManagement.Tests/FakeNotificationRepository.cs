using NotificationManagement.Core.Entities;
using NotificationManagement.Core.Interfaces;
using NotificationManagement.Core.Models;

namespace NotificationManagement.Tests;

public class FakeNotificationRepository : INotificationRepository
{
    public List<Notification> Items { get; } = new();
    public int SaveCount { get; private set; }

    public Task<IReadOnlyList<Notification>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Notification>>(Items.ToList());

    public Task<Notification?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => Task.FromResult(Items.FirstOrDefault(n => n.Id == id));

    public Task AddAsync(Notification entity, CancellationToken cancellationToken = default)
    {
        entity.Id = Items.Count + 1;
        Items.Add(entity);
        return Task.CompletedTask;
    }

    public void Update(Notification entity) { }
    public void Delete(Notification entity) => Items.Remove(entity);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SaveCount++;
        return Task.FromResult(1);
    }


    public Task<(IReadOnlyList<Notification> Items, int TotalCount)>
        GetFilteredAsync(
            NotificationFilter filter,
            CancellationToken cancellationToken = default)
    {
        IEnumerable<Notification> query = Items;

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            query = query.Where(n =>
                n.Receiver.Contains(
                    filter.Search,
                    StringComparison.OrdinalIgnoreCase));
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

        var totalCount = query.Count();

        query = filter.NewestFirst
            ? query.OrderByDescending(n => n.CreatedAt)
            : query.OrderBy(n => n.CreatedAt);

        var pageNumber = Math.Max(1, filter.PageNumber);
        var pageSize = Math.Clamp(filter.PageSize, 1, 100);

        var items = query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Task.FromResult<
            (IReadOnlyList<Notification> Items, int TotalCount)>(
                (items, totalCount));
    }

    public Task<IReadOnlyList<Notification>> GetRecentAsync(int count, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Notification>>(Items.Take(count).ToList());

    public Task<NotificationStats> GetStatsAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(new NotificationStats(Items.Count, 0, 0, 0));
}
