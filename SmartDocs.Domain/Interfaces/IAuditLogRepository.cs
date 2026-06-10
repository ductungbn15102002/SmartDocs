using SmartDocs.Domain.Entities;

namespace SmartDocs.Domain.Interfaces;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog log);
    Task<IEnumerable<AuditLog>> GetByDocumentIdAsync(Guid documentId);
}