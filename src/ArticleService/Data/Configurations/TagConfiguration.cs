using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ArticleService.Models;

namespace ArticleService.Data.Configurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedNever();

        builder.Property(t => t.Name)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(t => t.Slug)
               .IsRequired()
               .HasMaxLength(50);

        builder.HasIndex(t => t.Slug).IsUnique();
        builder.HasIndex(t => t.Name);

        builder.ToTable("tags");
    }
}