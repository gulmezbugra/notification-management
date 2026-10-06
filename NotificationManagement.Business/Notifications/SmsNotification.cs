using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NotificationManagement.Business.Settings;
using NotificationManagement.Core.Enums;
using NotificationManagement.Core.Exceptions;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace NotificationManagement.Business.Notifications;

public class SmsNotification : NotificationBase
{
    private readonly SmsSettings _settings;
    private readonly ILogger<SmsNotification> _logger;

    public SmsNotification(
        ILogger<SmsNotification> logger,
        IOptions<SmsSettings> settings)
        : base(logger)
    {
        _logger = logger;
        _settings = settings.Value;
    }

    public override NotificationType Type => NotificationType.Sms;

    protected override string ChannelName => "SMS";

    public override bool IsValidReceiver(string receiver)
    {
        return receiver.StartsWith("+")
               && receiver.Length >= 10
               && receiver.Length <= 16
               && receiver.Skip(1).All(char.IsDigit);
    }

    public override async Task SendAsync(
        string receiver,
        string message,
        CancellationToken cancellationToken = default)
    {
        try
        {
            TwilioClient.Init(
                _settings.AccountSid,
                _settings.AuthToken);

            var sms = await MessageResource.CreateAsync(
                body: message,
                from: new PhoneNumber(_settings.FromNumber),
                to: new PhoneNumber(receiver));

            _logger.LogInformation(
                "SMS successfully sent to {Receiver}. SID: {Sid}",
                receiver,
                sms.Sid);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send SMS to {Receiver}",
                receiver);

            throw new NotificationSendException(
                $"SMS could not be sent to {receiver}.");
        }
    }
}