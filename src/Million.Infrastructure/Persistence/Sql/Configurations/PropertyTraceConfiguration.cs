using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Million.Domain.Entities;

namespace Million.Infrastructure.Persistence.Sql.Configurations;

public class PropertyTraceConfiguration : IEntityTypeConfiguration<PropertyTrace>
{
    public void Configure(EntityTypeBuilder<PropertyTrace> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.DateSale)
            .IsRequired();

        builder.Property(e => e.Value)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(e => e.Tax)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
    }
}
