using Microsoft.EntityFrameworkCore;
using Million.Application.Common.Interfaces;
using Million.Application.Common.Models;
using Million.Domain.Entities;
using Million.Infrastructure.Persistence.Sql;

namespace Million.Infrastructure.Persistence.Sql.Repositories;

public class PropertyRepository : IPropertyRepository
{
    private readonly MillionDbContext _context;

    public PropertyRepository(MillionDbContext context)
    {
        _context = context;
    }

    public async Task<Property?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Properties
            .Include(p => p.Images)
            .Include(p => p.Traces)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Property>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Properties
            .Include(p => p.Images)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Property>> SearchAsync(
        string? name = null,
        string? address = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Properties.AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(p => p.Name.Contains(name));

        if (!string.IsNullOrWhiteSpace(address))
            query = query.Where(p => p.Address.Contains(address));

        if (minPrice.HasValue)
            query = query.Where(p => p.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => p.Price <= maxPrice.Value);

        return await query
            .Include(p => p.Images)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IEnumerable<Property> Properties, int TotalCount)> SearchPagedAsync(
        PaginationParameters pagination,
        string? name = null,
        string? address = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Properties.AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(p => p.Name.Contains(name));

        if (!string.IsNullOrWhiteSpace(address))
            query = query.Where(p => p.Address.Contains(address));

        if (minPrice.HasValue)
            query = query.Where(p => p.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => p.Price <= maxPrice.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Include(p => p.Images)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Properties.AnyAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<bool> CodeInternalExistsAsync(string codeInternal, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Properties.AsQueryable();

        if (excludeId.HasValue)
            query = query.Where(p => p.Id != excludeId.Value);

        return await query.AnyAsync(p => p.CodeInternal == codeInternal, cancellationToken);
    }

    public async Task<IEnumerable<Property>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        return await _context.Properties
            .Where(p => p.OwnerId == ownerId)
            .Include(p => p.Images)
            .ToListAsync(cancellationToken);
    }

    public async Task<Guid> AddAsync(Property property, CancellationToken cancellationToken = default)
    {
        await _context.Properties.AddAsync(property, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return property.Id;
    }

    public async Task<bool> UpdateAsync(Property property, CancellationToken cancellationToken = default)
    {
        _context.Properties.Update(property);
        var result = await _context.SaveChangesAsync(cancellationToken);
        return result > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var property = await _context.Properties.FindAsync(new object[] { id }, cancellationToken);
        if (property == null) return false;

        _context.Properties.Remove(property);
        var result = await _context.SaveChangesAsync(cancellationToken);
        return result > 0;
    }
}
