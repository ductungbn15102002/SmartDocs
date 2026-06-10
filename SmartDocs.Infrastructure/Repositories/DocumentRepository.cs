using SmartDocs.Infrastructure.Persistence;

namespace SmartDocs.Infrastructure.Repositories;

public class DocumentRepository : IDocumentRepository
{
    private readonly AppDbContext _context;

    public DocumentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Document?> GetByIdAsync(Guid id) =>
        await _context.Documents
            .Include(d => d.SubmittedBy)
            .Include(d => d.ReviewedBy)
            .Include(d => d.Comments)
            .Include(d => d.AuditLogs)
            .FirstOrDefaultAsync(d => d.Id == id);

    public async Task<IEnumerable<Document>> GetAllAsync() =>
        await _context.Documents
            .Include(d => d.SubmittedBy)
            .ToListAsync();

    public async Task AddAsync(Document document)
    {
        await _context.Documents.AddAsync(document);
        await _context.SaveChangesAsync(CancellationToken.None);
    }

    public async Task UpdateAsync(Document document)
    {
        _context.Documents.Update(document);
        await _context.SaveChangesAsync(CancellationToken.None);
    }

    public async Task DeleteAsync(Guid id)
    {
        var doc = await GetByIdAsync(id);
        if (doc != null)
        {
            _context.Documents.Remove(doc);
            await _context.SaveChangesAsync(CancellationToken.None);
        }
    }
}