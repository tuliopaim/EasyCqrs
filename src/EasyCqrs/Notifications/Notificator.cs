namespace EasyCqrs.Notifications;

public class Notificator : INotificator
{
    private readonly List<Notification> _notifications = new();

    public IReadOnlyList<Notification> Notifications => _notifications;

    public bool IsValid => Notifications.Count == 0;

    public void AddNotification(string notification)
    {
        ArgumentNullException.ThrowIfNull(notification);

        _notifications.Add(new Notification(notification));
    }

    public void AddNotifications(IEnumerable<string> notifications)
    {
        ArgumentNullException.ThrowIfNull(notifications);
        
        _notifications.AddRange(notifications.Select(x => new Notification(x)));
    }
}