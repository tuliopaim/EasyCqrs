namespace EasyCqrs.Notifications;

public interface INotification
{
    Exception? Exception { get; }
    string Message { get; }
}