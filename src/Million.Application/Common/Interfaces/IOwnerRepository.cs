using Million.Domain.Entities;

namespace Million.Application.Common.Interfaces;

public interface IOwnerRepository
{
    Task<Owner?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Owner>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Guid> AddAsync(Owner owner, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Owner owner, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
