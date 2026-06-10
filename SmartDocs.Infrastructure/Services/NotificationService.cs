namespace SmartDocs.Infrastructure.Services;

using Microsoft.AspNetCore.SignalR;

public class NotificationService : INotificationService
{
    private readonly IHubContext<Hub> _hubContext;

    public NotificationService(IHubContext<Hub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyDocumentStatusChanged(Guid documentId, string status)
    {
        await _hubContext.Clients.Group(documentId.ToString())
            .SendAsync("DocumentStatusChanged", new { documentId, status });
    }
}