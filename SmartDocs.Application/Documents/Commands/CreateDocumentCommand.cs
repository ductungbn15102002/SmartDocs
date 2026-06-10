using SmartDocs.Application.Common.Interfaces;

namespace SmartDocs.Application.Documents.Commands;

public record CreateDocumentCommand(
    string Title,
    Stream FileStream,
    string FileName,
    string ContentType,
    Guid SubmittedById
) : IRequest<Guid>;

public class CreateDocumentCommandHandler : IRequestHandler<CreateDocumentCommand, Guid>
{
    private readonly IDocumentRepository _documentRepo;
    private readonly IFileStorageService _fileStorage;

    public CreateDocumentCommandHandler(IDocumentRepository documentRepo, IFileStorageService fileStorage)
    {
        _documentRepo = documentRepo;
        _fileStorage = fileStorage;
    }

    public async Task<Guid> Handle(CreateDocumentCommand request, CancellationToken cancellationToken)
    {
        var filePath = await _fileStorage.UploadAsync(request.FileStream, request.FileName, request.ContentType);

        var document = new Document
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            FilePath = filePath,
            FileType = Path.GetExtension(request.FileName).ToUpper(),
            Status = DocumentStatus.Draft,
            SubmittedById = request.SubmittedById,
            CreatedAt = DateTime.UtcNow
        };

        await _documentRepo.AddAsync(document);
        return document.Id;
    }
}