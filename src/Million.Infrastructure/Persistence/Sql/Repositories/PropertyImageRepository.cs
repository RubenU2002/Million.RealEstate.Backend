using Microsoft.EntityFrameworkCore;
using Million.Application.Common.Interfaces;
using Million.Domain.Entities;
using Million.Infrastructure.Persistence.Sql;

namespace Million.Infrastructure.Persistence.Sql.Repositories;

public class PropertyImageRepository : IPropertyImageRepository
{
    private readonly MillionDbContext _context;

    public PropertyImageRepository(MillionDbContext context)
    {
        _context = context;
    }

    public async Task<PropertyImage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.PropertyImages.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<PropertyImage>> GetByPropertyIdAsync(Guid propertyId, CancellationToken cancellationToken = default)
    {
        return await _context.PropertyImages
            .Where(i => i.PropertyId == propertyId)
            .ToListAsync(cancellationToken);
    }

    public async Task<PropertyImage?> GetFirstEnabledByPropertyIdAsync(Guid propertyId, CancellationToken cancellationToken = default)
    {
        return await _context.PropertyImages
            .Where(i => i.PropertyId == propertyId && i.Enabled)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Guid> AddAsync(PropertyImage propertyImage, CancellationToken cancellationToken = default)
    {
        await _context.PropertyImages.AddAsync(propertyImage, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return propertyImage.Id;
    }

    public async Task<bool> UpdateAsync(PropertyImage propertyImage, CancellationToken cancellationToken = default)
    {
        _context.PropertyImages.Update(propertyImage);
        var result = await _context.SaveChangesAsync(cancellationToken);
        return result > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var image = await GetByIdAsync(id, cancellationToken);
        if (image == null) return false;

        _context.PropertyImages.Remove(image);
        var result = await _context.SaveChangesAsync(cancellationToken);
        return result > 0;
    }
}
