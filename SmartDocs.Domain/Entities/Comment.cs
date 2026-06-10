using SmartDocs.Domain.Enums;

namespace SmartDocs.Domain.Entities;

public class Comment
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public CommentTag Tag { get; set; }
    public DateTime CreatedAt { get; set; }
    public Document Document { get; set; } = null!;
    public User User { get; set; } = null!;
}