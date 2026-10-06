using Microsoft.Extensions.Logging;
using NotificationManagement.Core.Entities;
using NotificationManagement.Core.Enums;
using NotificationManagement.Core.Exceptions;
using NotificationManagement.Core.Interfaces;
using NotificationManagement.Core.Models;

namespace NotificationManagement.Business.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _repository;
    private readonly INotificationFactory _factory;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        INotificationRepository repository,
        INotificationFactory factory,
        ILogger<NotificationService> logger)
    {
        _repository = repository;
        _factory = factory;
        _logger = logger;
    }

    public async Task<Notification> SendNotificationAsync(CreateNotificationDto dto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Receiver))
            throw new NotificationValidationException("Receiver is required.");
        if (string.IsNullOrWhiteSpace(dto.Message))
            throw new NotificationValidationException("Message cannot be empty.");

        var receiver = dto.Receiver.Trim();

        // 1) Ask the factory for the right implementation (throws if the type is unsupported).
        INotification notification = _factory.Create(dto.Type);

        if (!notification.IsValidReceiver(receiver))
            throw new NotificationValidationException($"'{receiver}' is not a valid receiver for a {dto.Type} notification.");

        // 2) Persist as Pending first, so a record exists even if sending crashes.
        var entity = new Notification
        {
            Receiver = receiver,
            Message = dto.Message.Trim(),
            NotificationType = dto.Type,
            Status = NotificationStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(entity, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        // 3) Send and record the outcome.
        try
        {
            await notification.SendAsync(entity.Receiver,entity.Message,cancellationToken);

            entity.Status = NotificationStatus.Sent;
            entity.SentAt = DateTime.UtcNow;
            entity.ErrorMessage = null;
        }
        catch (NotificationSendException ex)
        {
            _logger.LogWarning(ex, "Provider failed for notification {Id}", entity.Id);
            entity.Status = NotificationStatus.Failed;
            entity.ErrorMessage = ex.Message;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            // Never leak technical details into the stored/displayed message.
            _logger.LogError(ex, "Unexpected error while sending notification {Id}", entity.Id);
            entity.Status = NotificationStatus.Failed;
            entity.ErrorMessage = "An unexpected error occurred while sending the notification.";
        }

        _repository.Update(entity);
        await _repository.SaveChangesAsync(cancellationToken);

        return entity;
    }


    public Task<(IReadOnlyList<Notification> Items, int TotalCount)>
        GetNotificationsAsync(
            NotificationFilter filter,
            CancellationToken cancellationToken = default)
    {
        return _repository.GetFilteredAsync(
            filter,
            cancellationToken);
    }

    public async Task<Notification> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _repository.GetByIdAsync(id, cancellationToken)
           ?? throw new NotFoundException($"Notification with id {id} was not found.");

    public async Task<DashboardData> GetDashboardAsync(int recentCount = 8, CancellationToken cancellationToken = default)
    {
        var stats = await _repository.GetStatsAsync(cancellationToken);
        var recent = await _repository.GetRecentAsync(recentCount, cancellationToken);
        return new DashboardData(stats, recent);
    }
}
