namespace SmartDocs.Application.Documents.Commands;

public record SubmitDocumentCommand(Guid DocumentId, Guid ReviewerId) : IRequest;

public class SubmitDocumentCommandHandler : IRequestHandler<SubmitDocumentCommand>
{
    private readonly IDocumentRepository _documentRepo;

    public SubmitDocumentCommandHandler(IDocumentRepository documentRepo)
    {
        _documentRepo = documentRepo;
    }

    public async Task Handle(SubmitDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = await _documentRepo.GetByIdAsync(request.DocumentId)
            ?? throw new Exception("Document not found");

        if (document.Status != DocumentStatus.Draft)
            throw new Exception("Only Draft documents can be submitted");

        document.Status = DocumentStatus.Pending;
        document.ReviewedById = request.ReviewerId;
        document.UpdatedAt = DateTime.UtcNow;

        await _documentRepo.UpdateAsync(document);
    }
}