using SmartDocs.Domain.Enums;
using System.Xml.Linq;

namespace SmartDocs.Domain.Entities;

public class Document
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public DocumentStatus Status { get; set; }
    public Guid SubmittedById { get; set; }
    public Guid? ReviewedById { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public User SubmittedBy { get; set; } = null!;
    public User? ReviewedBy { get; set; }
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}