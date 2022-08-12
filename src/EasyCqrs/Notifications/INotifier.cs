namespace EasyCqrs.Notifications;

public interface INotifier
{
    IReadOnlyList<INotification> Notifications { get; }
    bool IsValid { get; }
    void Notify(INotification notification);
}
