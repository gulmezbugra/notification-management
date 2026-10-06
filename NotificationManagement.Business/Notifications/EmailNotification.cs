using System.Net.Mail;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using NotificationManagement.Business.Settings;
using NotificationManagement.Core.Enums;
using NotificationManagement.Core.Exceptions;

namespace NotificationManagement.Business.Notifications;

public class EmailNotification : NotificationBase
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailNotification> _logger;

    public EmailNotification(
        ILogger<EmailNotification> logger,
        IOptions<EmailSettings> settings)
        : base(logger)
    {
        _logger = logger;
        _settings = settings.Value;
    }

    public override NotificationType Type => NotificationType.Email;

    protected override string ChannelName => "Email";

    public override bool IsValidReceiver(string receiver)
        => MailAddress.TryCreate(receiver, out var address)
           && address.Address == receiver.Trim();

    public override async Task SendAsync(
        string receiver,
        string message,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var email = new MimeMessage();

            email.From.Add(
                new MailboxAddress(
                    _settings.SenderName,
                    _settings.SenderEmail));

            email.To.Add(
                MailboxAddress.Parse(receiver));

            email.Subject = "NotifyHub Notification";

            email.Body = new TextPart("plain")
            {
                Text = message
            };

            using var smtp = new MailKit.Net.Smtp.SmtpClient();

            await smtp.ConnectAsync(
                _settings.SmtpServer,
                _settings.Port,
                SecureSocketOptions.StartTls,
                cancellationToken);

            await smtp.AuthenticateAsync(
                _settings.SenderEmail,
                _settings.Password,
                cancellationToken);

            await smtp.SendAsync(
                email,
                cancellationToken);

            await smtp.DisconnectAsync(
                true,
                cancellationToken);

            _logger.LogInformation(
                "Email successfully sent to {Receiver}",
                receiver);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send email to {Receiver}",
                receiver);

            throw new NotificationSendException(
                $"Email could not be sent to {receiver}.");
        }
    }
}