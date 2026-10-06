using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotificationManagement.Core.Entities;

namespace NotificationManagement.Data.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Receiver).IsRequired().HasMaxLength(256);
        builder.Property(n => n.Message).IsRequired().HasMaxLength(1000);

        // Store enums as readable strings instead of magic numbers.
        builder.Property(n => n.NotificationType).IsRequired().HasConversion<string>().HasMaxLength(20);
        builder.Property(n => n.Status).IsRequired().HasConversion<string>().HasMaxLength(20);

        builder.Property(n => n.CreatedAt).IsRequired();
        builder.Property(n => n.ErrorMessage).HasMaxLength(500);

        builder.HasIndex(n => n.Status);
        builder.HasIndex(n => n.CreatedAt);
    }
}
