namespace SmartDocs.Application.Documents.Commands;

public record ApproveRejectDocumentCommand(
    Guid DocumentId,
    bool IsApproved,
    string Comment,
    CommentTag Tag,
    Guid ReviewerId
) : IRequest;

public class ApproveRejectDocumentCommandHandler : IRequestHandler<ApproveRejectDocumentCommand>
{
    private readonly IDocumentRepository _documentRepo;
    private readonly IAuditLogRepository _auditRepo;

    public ApproveRejectDocumentCommandHandler(IDocumentRepository documentRepo, IAuditLogRepository auditRepo)
    {
        _documentRepo = documentRepo;
        _auditRepo = auditRepo;
    }

    public async Task Handle(ApproveRejectDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = await _documentRepo.GetByIdAsync(request.DocumentId)
            ?? throw new Exception("Document not found");

        if (document.Status != DocumentStatus.Pending)
            throw new Exception("Only Pending documents can be approved/rejected");

        document.Status = request.IsApproved ? DocumentStatus.Approved : DocumentStatus.Rejected;
        document.UpdatedAt = DateTime.UtcNow;

        document.Comments.Add(new Comment
        {
            Id = Guid.NewGuid(),
            DocumentId = document.Id,
            UserId = request.ReviewerId,
            Content = request.Comment,
            Tag = request.Tag,
            CreatedAt = DateTime.UtcNow
        });

        await _documentRepo.UpdateAsync(document);

        await _auditRepo.AddAsync(new AuditLog
        {
            Id = Guid.NewGuid(),
            DocumentId = document.Id,
            UserId = request.ReviewerId,
            Action = request.IsApproved ? "APPROVED" : "REJECTED",
            Timestamp = DateTime.UtcNow
        });
    }
}