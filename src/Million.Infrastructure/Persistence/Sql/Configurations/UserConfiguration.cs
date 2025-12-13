using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Million.Domain.Entities;

namespace Million.Infrastructure.Persistence.Sql.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(150);
        
        builder.HasIndex(e => e.Email)
            .IsUnique();

        builder.Property(e => e.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);
            
        builder.Property(e => e.Role)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.CreatedAt)
            .IsRequired();
    }
}
