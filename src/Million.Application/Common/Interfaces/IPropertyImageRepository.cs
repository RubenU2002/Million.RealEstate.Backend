using Million.Domain.Entities;

namespace Million.Application.Common.Interfaces;

public interface IPropertyImageRepository
{
    Task<PropertyImage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<PropertyImage>> GetByPropertyIdAsync(Guid propertyId, CancellationToken cancellationToken = default);
    Task<PropertyImage?> GetFirstEnabledByPropertyIdAsync(Guid propertyId, CancellationToken cancellationToken = default);
    Task<Guid> AddAsync(PropertyImage propertyImage, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(PropertyImage propertyImage, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
