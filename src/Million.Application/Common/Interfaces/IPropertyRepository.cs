using Million.Application.Common.Models;
using Million.Domain.Entities;

namespace Million.Application.Common.Interfaces;

public interface IPropertyRepository
{
    Task<Property?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Property>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Property>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Property>> SearchAsync(
        string? name = null, 
        string? address = null, 
        decimal? minPrice = null, 
        decimal? maxPrice = null,
        CancellationToken cancellationToken = default);
    Task<(IEnumerable<Property> Properties, int TotalCount)> SearchPagedAsync(
        PaginationParameters pagination,
        string? name = null, 
        string? address = null, 
        decimal? minPrice = null, 
        decimal? maxPrice = null,
        CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> CodeInternalExistsAsync(string codeInternal, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<Guid> AddAsync(Property property, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Property property, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
