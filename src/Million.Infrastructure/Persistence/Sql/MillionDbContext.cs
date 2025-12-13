using Microsoft.EntityFrameworkCore;
using Million.Domain.Entities;
using System.Reflection;

namespace Million.Infrastructure.Persistence.Sql;

public class MillionDbContext : DbContext
{
    public MillionDbContext(DbContextOptions<MillionDbContext> options) : base(options)
    {
    }

    public DbSet<Owner> Owners { get; set; } = null!;
    public DbSet<Property> Properties { get; set; } = null!;
    public DbSet<PropertyImage> PropertyImages { get; set; } = null!;
    public DbSet<PropertyTrace> PropertyTraces { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
