using Million.Domain.Entities;

namespace Million.Application.Common.Interfaces;

public interface IPropertyTraceRepository
{
    Task<PropertyTrace?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<PropertyTrace>> GetByPropertyIdAsync(Guid propertyId, CancellationToken cancellationToken = default);
    Task<PropertyTrace?> GetLatestByPropertyIdAsync(Guid propertyId, CancellationToken cancellationToken = default);
    Task<Guid> AddAsync(PropertyTrace propertyTrace, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(PropertyTrace propertyTrace, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
