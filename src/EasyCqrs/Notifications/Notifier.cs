namespace EasyCqrs.Notifications;

public class Notifier : INotifier
{
    private readonly List<INotification> _notifications = new();

    public IReadOnlyList<INotification> Notifications => _notifications;

    public bool IsValid => Notifications.Count == 0;
    
    public void Notify(INotification notification)
    {
        _notifications.Add(notification);
    }
}