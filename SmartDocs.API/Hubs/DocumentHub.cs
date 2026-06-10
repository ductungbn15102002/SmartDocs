namespace SmartDocs.API.Hubs;

using Microsoft.AspNetCore.SignalR;

public class DocumentHub : Hub
{
    public async Task JoinDocument(string documentId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, documentId);
    }

    public async Task LeaveDocument(string documentId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, documentId);
    }
}