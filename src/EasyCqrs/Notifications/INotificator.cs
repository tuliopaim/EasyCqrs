namespace EasyCqrs.Notifications;

public interface INotificator
{
    IReadOnlyList<Notification> Notifications { get; }
    bool IsValid { get; }
    void AddNotification(string notification);
    void AddNotifications(IEnumerable<string> notification);
}