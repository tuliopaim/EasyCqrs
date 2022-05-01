namespace EasyCqrs.Notifications;

public interface INotifier
{
    IEnumerable<string> GetErrorList();
    IReadOnlyList<Notification> Notifications { get; }
    bool IsValid { get; }
    void AddNotification(string notification);
    void AddNotifications(IEnumerable<string> notification);
}