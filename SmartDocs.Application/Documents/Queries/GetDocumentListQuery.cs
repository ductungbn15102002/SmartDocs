namespace SmartDocs.Application.Documents.Queries;

public record GetDocumentListQuery(DocumentStatus? Status, Guid? UserId) : IRequest<IEnumerable<DocumentDto>>;

public record DocumentDto(Guid Id, string Title, string FileType, string Status, DateTime CreatedAt, string SubmittedBy);

public class GetDocumentListQueryHandler : IRequestHandler<GetDocumentListQuery, IEnumerable<DocumentDto>>
{
    private readonly IDocumentRepository _documentRepo;

    public GetDocumentListQueryHandler(IDocumentRepository documentRepo)
    {
        _documentRepo = documentRepo;
    }

    public async Task<IEnumerable<DocumentDto>> Handle(GetDocumentListQuery request, CancellationToken cancellationToken)
    {
        var documents = await _documentRepo.GetAllAsync();

        if (request.Status.HasValue)
            documents = documents.Where(d => d.Status == request.Status);

        if (request.UserId.HasValue)
            documents = documents.Where(d => d.SubmittedById == request.UserId);

        return documents.Select(d => new DocumentDto(
            d.Id, d.Title, d.FileType,
            d.Status.ToString(), d.CreatedAt,
            d.SubmittedBy?.FullName ?? "Unknown"
        ));
    }
}