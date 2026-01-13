namespace notification_service.Services;

public interface INotificationService
{
    Task SendToPageAsync(string pageUrl, string eventName, object data);

    Task SendToAllAsync(string eventName, object data);

    Task SendToClientAsync(string connectionId, string eventName, object data);
}