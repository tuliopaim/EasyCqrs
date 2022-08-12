namespace EasyCqrs.Notifications;

public class Notification : INotification
{
    public Notification(string message)
    {
        ArgumentNullException.ThrowIfNull(message);
        Message = message;
    }

    public Notification(Exception exception, string message)
    {
        ArgumentNullException.ThrowIfNull(message);

        Exception = exception;
        Message = message;
    }

    public Exception? Exception { get; }
    public string Message { get; }
}