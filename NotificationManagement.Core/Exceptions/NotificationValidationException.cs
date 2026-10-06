namespace NotificationManagement.Core.Exceptions;

/// <summary>Thrown for invalid receiver / empty message. Safe to show to end users.</summary>
public class NotificationValidationException : Exception
{
    public NotificationValidationException(string message) : base(message) { }
}
