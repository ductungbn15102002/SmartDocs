namespace SmartDocs.Application.Common.Interfaces;

public interface INotificationService
{
    Task NotifyDocumentStatusChanged(Guid documentId, string status);
}