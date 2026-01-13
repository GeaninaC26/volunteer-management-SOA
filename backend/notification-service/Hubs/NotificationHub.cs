using Microsoft.AspNetCore.SignalR;

namespace notification_service.Hubs;

public class NotificationHub : Hub
{
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(ILogger<NotificationHub> logger)
    {
        _logger = logger;
    }
    public async Task JoinPageGroup(string url)
    {
        var groupName = GetGroupNameFromUrl(url);
        _logger.LogInformation("Connection {ConnectionId} joining group {GroupName} for URL {Url}", 
            Context.ConnectionId, groupName, url);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        _logger.LogDebug("Connection {ConnectionId} successfully joined group {GroupName}", 
            Context.ConnectionId, groupName);
    }
    public async Task LeavePageGroup(string url)
    {
        var groupName = GetGroupNameFromUrl(url);
        _logger.LogInformation("Connection {ConnectionId} leaving group {GroupName} for URL {Url}", 
            Context.ConnectionId, groupName, url);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        _logger.LogDebug("Connection {ConnectionId} successfully left group {GroupName}", 
            Context.ConnectionId, groupName);
    }
    private string GetGroupNameFromUrl(string url)
    {
        try{
            var groupName = url.Split('?')[0].TrimEnd('/');
            _logger.LogDebug("Parsed URL {Url} to group name {GroupName}", url, groupName);
            return groupName;
        }catch(Exception ex){
            _logger.LogWarning(ex, "Failed to parse URL {Url}, using URL as group name", url);
            return url;
        }
    }
    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (exception != null)
        {
            _logger.LogWarning(exception, "Client disconnected with exception: {ConnectionId}", Context.ConnectionId);
        }
        else
        {
            _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
        }
        await base.OnDisconnectedAsync(exception);
    }
}