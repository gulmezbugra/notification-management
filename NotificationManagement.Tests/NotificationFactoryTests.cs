using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NotificationManagement.Business;
using NotificationManagement.Business.Notifications;
using NotificationManagement.Core.Enums;
using NotificationManagement.Core.Exceptions;
using NotificationManagement.Core.Interfaces;

namespace NotificationManagement.Tests;

public class NotificationFactoryTests
{
    private static INotificationFactory BuildFactoryFromDi()
    {
        var services = new ServiceCollection();
        services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
        services.AddBusinessLayer();
        return services.BuildServiceProvider().GetRequiredService<INotificationFactory>();
    }

    [Theory]
    [InlineData(NotificationType.Email, typeof(EmailNotification))]
    [InlineData(NotificationType.Sms, typeof(SmsNotification))]
    [InlineData(NotificationType.WhatsApp, typeof(WhatsAppNotification))]
    [InlineData(NotificationType.Push, typeof(PushNotification))]
    public void Create_ReturnsCorrectImplementation(NotificationType type, Type expected)
    {
        var factory = BuildFactoryFromDi();

        var notification = factory.Create(type);

        Assert.IsType(expected, notification);
        Assert.Equal(type, notification.Type);
    }

    [Fact]
    public void Create_EveryEnumValueIsSupported()
    {
        var factory = BuildFactoryFromDi();

        foreach (var type in Enum.GetValues<NotificationType>())
            Assert.NotNull(factory.Create(type));
    }

    [Fact]
    public void Create_UnsupportedType_Throws()
    {
        var factory = BuildFactoryFromDi();

        Assert.Throws<UnsupportedNotificationTypeException>(() => factory.Create((NotificationType)99));
    }

    [Theory]
    [InlineData(NotificationType.Email, "user@example.com", true)]
    [InlineData(NotificationType.Email, "not-an-email", false)]
    [InlineData(NotificationType.Sms, "+905551234567", true)]
    [InlineData(NotificationType.Sms, "abc", false)]
    [InlineData(NotificationType.WhatsApp, "+90 555 123 45 67", true)]
    [InlineData(NotificationType.Push, "device-token-123", true)]
    [InlineData(NotificationType.Push, "short", false)]
    public void IsValidReceiver_ValidatesPerChannel(NotificationType type, string receiver, bool expected)
    {
        var notification = BuildFactoryFromDi().Create(type);

        Assert.Equal(expected, notification.IsValidReceiver(receiver));
    }
}
