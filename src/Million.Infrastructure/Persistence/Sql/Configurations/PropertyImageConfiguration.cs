using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Million.Domain.Entities;

namespace Million.Infrastructure.Persistence.Sql.Configurations;

public class PropertyImageConfiguration : IEntityTypeConfiguration<PropertyImage>
{
    public void Configure(EntityTypeBuilder<PropertyImage> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.File)
            .IsRequired()
            .HasMaxLength(500); // URL or Path length

        builder.Property(e => e.Enabled)
            .IsRequired();
    }
}
