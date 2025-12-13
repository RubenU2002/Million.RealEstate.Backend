using Microsoft.EntityFrameworkCore;
using Million.Application.Common.Interfaces;
using Million.Domain.Entities;
using Million.Infrastructure.Persistence.Sql;

namespace Million.Infrastructure.Persistence.Sql.Repositories;

public class OwnerRepository : IOwnerRepository
{
    private readonly MillionDbContext _context;

    public OwnerRepository(MillionDbContext context)
    {
        _context = context;
    }

    public async Task<Owner?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Owners
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Owner>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Owners.ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Owners.AnyAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<Guid> AddAsync(Owner owner, CancellationToken cancellationToken = default)
    {
        await _context.Owners.AddAsync(owner, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return owner.Id;
    }

    public async Task<bool> UpdateAsync(Owner owner, CancellationToken cancellationToken = default)
    {
        _context.Owners.Update(owner);
        var result = await _context.SaveChangesAsync(cancellationToken);
        return result > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var owner = await GetByIdAsync(id, cancellationToken);
        if (owner == null) return false;

        _context.Owners.Remove(owner);
        var result = await _context.SaveChangesAsync(cancellationToken);
        return result > 0;
    }
}
