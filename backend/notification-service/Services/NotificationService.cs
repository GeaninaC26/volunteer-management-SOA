using Microsoft.AspNetCore.SignalR;
using notification_service.Hubs;

namespace notification_service.Services;

public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(IHubContext<NotificationHub> hubContext, ILogger<NotificationService> logger){
        _hubContext = hubContext;
        _logger = logger;
    }
    public async Task SendToPageAsync(string pageUrl, string eventName, object data){
        try{
            await _hubContext.Clients.Group(pageUrl).SendAsync(eventName, data);
            _logger.LogInformation("Sent event {EventName} to page group {GroupName}", eventName, pageUrl);
        }catch (Exception ex){
            _logger.LogError(ex, "Error sending event {EventName} to page {PageUrl}", eventName, pageUrl);
            throw;
        }
    }
    public async Task SendToAllAsync(string eventName, object data){
        try{
            await _hubContext.Clients.All.SendAsync(eventName, data);
            _logger.LogInformation("Sent event {EventName} to all clients", eventName);
        }catch (Exception ex){
            _logger.LogError(ex, "Error sending event {EventName} to all clients", eventName);
            throw;
        }
    }

    public async Task SendToClientAsync(string connectionId, string eventName, object data){
        try{
            await _hubContext.Clients.Client(connectionId).SendAsync(eventName, data);
            _logger.LogInformation("Sent event {EventName} to client {ConnectionId}", eventName, connectionId);
        }catch (Exception ex){
            _logger.LogError(ex, "Error sending event {EventName} to client {ConnectionId}", eventName, connectionId);
            throw;
        }
    }
}