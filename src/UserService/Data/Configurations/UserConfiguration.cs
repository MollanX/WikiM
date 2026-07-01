using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Models;

namespace UserService.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).ValueGeneratedNever();

        builder.Property(u => u.Username)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(u => u.Email)
               .IsRequired()
               .HasMaxLength(256);

        builder.Property(u => u.Phone)
               .HasMaxLength(20);

        builder.Property(u => u.Bio)
               .HasMaxLength(500);

        builder.Property(u => u.BirthDate)
               .IsRequired(false);

        builder.Property(u => u.Role)
               .HasConversion<string>()
               .HasMaxLength(20)
               .HasDefaultValue(UserRole.User);

        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.Username).IsUnique();
        builder.HasIndex(u => u.Phone).IsUnique().HasFilter(null); // null == не уникальный, но для будущего

        builder.ToTable("users");
    }
}