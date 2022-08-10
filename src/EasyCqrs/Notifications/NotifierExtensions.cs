namespace EasyCqrs.Notifications;

public static class NotifierExtensions
{
    public static IEnumerable<string> Errors(this INotifier notifier)
    {
        foreach (var notification in notifier.Notifications)
        {
            yield return notification.Message;
        }
    }

    public static bool ContainsException(this INotifier notifier)
    {
        return notifier.Notifications.Any(n => n.Exception is not null);
    }

    public static void Notify(this INotifier notifier, IEnumerable<string> notifications)
    {
        ArgumentNullException.ThrowIfNull(notifications);

        foreach (var notification in notifications)
        {
            notifier.Notify(new Notification(notification));
        }
    }

    public static void Notify(this INotifier notifier, string notification)
    {
        ArgumentNullException.ThrowIfNull(notification);
        notifier.Notify(new Notification(notification));
    }
}
