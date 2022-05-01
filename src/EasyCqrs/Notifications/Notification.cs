namespace EasyCqrs.Notifications;

public class Notification
{
    public Notification(string message)
    {
        ArgumentNullException.ThrowIfNull(message);
        Message = message;
    }

    public string Message { get; }
}