namespace NotificationManagement.Core.Exceptions;

/// <summary>Thrown by providers when sending fails. Message is safe to store and display.</summary>
public class NotificationSendException : Exception
{
    public NotificationSendException(string message) : base(message) { }
    public NotificationSendException(string message, Exception inner) : base(message, inner) { }
}
