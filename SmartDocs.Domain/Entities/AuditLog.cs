namespace SmartDocs.Domain.Entities;

public class AuditLog
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public Guid UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public Document Document { get; set; } = null!;
    public User User { get; set; } = null!;
}