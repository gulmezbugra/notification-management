using Microsoft.Extensions.DependencyInjection;
using NotificationManagement.Business.Factories;
using NotificationManagement.Business.Services;
using NotificationManagement.Core.Interfaces;

namespace NotificationManagement.Business;

public static class DependencyInjection
{
    public static IServiceCollection AddBusinessLayer(this IServiceCollection services)
    {
        // Auto-register every concrete INotification in this assembly.
        // A new channel (e.g. TelegramNotification) is picked up with no change here.
        var notificationTypes = typeof(DependencyInjection).Assembly
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(INotification).IsAssignableFrom(t));

        foreach (var type in notificationTypes)
            services.AddScoped(typeof(INotification), type);

        services.AddScoped<INotificationFactory, NotificationFactory>();
        services.AddScoped<INotificationService, NotificationService>();

        return services;
    }
}
