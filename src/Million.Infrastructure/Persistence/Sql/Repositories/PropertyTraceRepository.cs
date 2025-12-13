using Microsoft.EntityFrameworkCore;
using Million.Application.Common.Interfaces;
using Million.Domain.Entities;
using Million.Infrastructure.Persistence.Sql;

namespace Million.Infrastructure.Persistence.Sql.Repositories;

public class PropertyTraceRepository : IPropertyTraceRepository
{
    private readonly MillionDbContext _context;

    public PropertyTraceRepository(MillionDbContext context)
    {
        _context = context;
    }

    public async Task<PropertyTrace?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.PropertyTraces.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<PropertyTrace>> GetByPropertyIdAsync(Guid propertyId, CancellationToken cancellationToken = default)
    {
        return await _context.PropertyTraces
            .Where(t => t.PropertyId == propertyId)
            .OrderByDescending(t => t.DateSale)
            .ToListAsync(cancellationToken);
    }

    public async Task<PropertyTrace?> GetLatestByPropertyIdAsync(Guid propertyId, CancellationToken cancellationToken = default)
    {
        return await _context.PropertyTraces
            .Where(t => t.PropertyId == propertyId)
            .OrderByDescending(t => t.DateSale)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Guid> AddAsync(PropertyTrace propertyTrace, CancellationToken cancellationToken = default)
    {
        await _context.PropertyTraces.AddAsync(propertyTrace, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return propertyTrace.Id;
    }

    public async Task<bool> UpdateAsync(PropertyTrace propertyTrace, CancellationToken cancellationToken = default)
    {
        _context.PropertyTraces.Update(propertyTrace);
        var result = await _context.SaveChangesAsync(cancellationToken);
        return result > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var trace = await GetByIdAsync(id, cancellationToken);
        if (trace == null) return false;

        _context.PropertyTraces.Remove(trace);
        var result = await _context.SaveChangesAsync(cancellationToken);
        return result > 0;
    }
}
