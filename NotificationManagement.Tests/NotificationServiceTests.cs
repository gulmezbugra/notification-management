using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NotificationManagement.Business;
using NotificationManagement.Business.Services;
using NotificationManagement.Core.Enums;
using NotificationManagement.Core.Exceptions;
using NotificationManagement.Core.Interfaces;
using NotificationManagement.Core.Models;

namespace NotificationManagement.Tests;

public class NotificationServiceTests
{
    private static (NotificationService Service, FakeNotificationRepository Repo) Build()
    {
        var services = new ServiceCollection();
        services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
        services.AddBusinessLayer();
        var factory = services.BuildServiceProvider().GetRequiredService<INotificationFactory>();

        var repo = new FakeNotificationRepository();
        return (new NotificationService(repo, factory, NullLogger<NotificationService>.Instance), repo);
    }

    [Fact]
    public async Task SendNotificationAsync_Success_MarksSent()
    {
        var (service, repo) = Build();

        var result = await service.SendNotificationAsync(new CreateNotificationDto
        {
            Receiver = "user@example.com",
            Message = "Hello",
            Type = NotificationType.Email
        });

        Assert.Equal(NotificationStatus.Sent, result.Status);
        Assert.NotNull(result.SentAt);
        Assert.Null(result.ErrorMessage);
        Assert.Single(repo.Items);
    }

    [Fact]
    public async Task SendNotificationAsync_ProviderFailure_MarksFailedWithMessage()
    {
        var (service, _) = Build();

        var result = await service.SendNotificationAsync(new CreateNotificationDto
        {
            Receiver = "+905551234567",
            Message = "Hello [fail]",
            Type = NotificationType.Sms
        });

        Assert.Equal(NotificationStatus.Failed, result.Status);
        Assert.Null(result.SentAt);
        Assert.False(string.IsNullOrWhiteSpace(result.ErrorMessage));
    }

    [Fact]
    public async Task SendNotificationAsync_InvalidReceiver_ThrowsAndSavesNothing()
    {
        var (service, repo) = Build();

        await Assert.ThrowsAsync<NotificationValidationException>(() =>
            service.SendNotificationAsync(new CreateNotificationDto
            {
                Receiver = "not-an-email",
                Message = "Hello",
                Type = NotificationType.Email
            }));

        Assert.Empty(repo.Items);
    }

    [Fact]
    public async Task SendNotificationAsync_EmptyMessage_Throws()
    {
        var (service, _) = Build();

        await Assert.ThrowsAsync<NotificationValidationException>(() =>
            service.SendNotificationAsync(new CreateNotificationDto
            {
                Receiver = "user@example.com",
                Message = "   ",
                Type = NotificationType.Email
            }));
    }

    [Fact]
    public async Task GetByIdAsync_Missing_ThrowsNotFound()
    {
        var (service, _) = Build();

        await Assert.ThrowsAsync<NotFoundException>(() => service.GetByIdAsync(999));
    }
}
