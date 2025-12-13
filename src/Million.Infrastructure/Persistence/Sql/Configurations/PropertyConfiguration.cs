using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Million.Domain.Entities;

namespace Million.Infrastructure.Persistence.Sql.Configurations;

public class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Description)
            .HasMaxLength(500);

        builder.Property(e => e.Address)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Price)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(e => e.CodeInternal)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Year)
            .IsRequired();

        builder.Property(e => e.Created)
            .IsRequired();

        // Relation with Owner
        builder.HasOne<Owner>()
            .WithMany()
            .HasForeignKey(p => p.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relation with Images
        builder.HasMany(p => p.Images)
            .WithOne()
            .HasForeignKey(i => i.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relation with Traces (assuming Traces belong to Property)
        // Note: The ER diagram shows Traces linked to Property, but typically traces are history.
        // Assuming cascade delete is okay for traces if property is deleted, or restricted.
        // Given Clean Code often prefers explicit deletes, we will stick to Cascade for composition strong ownership if appropriate.
        // Let's check logic: if property is deleted, traces should likely go? 
        // Or if traces are historical records, maybe we can't delete property?
        // Let's assume composition for now based on 'Traces' property in Property entity.
         builder.HasMany(p => p.Traces)
            .WithOne()
            .HasForeignKey(t => t.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
