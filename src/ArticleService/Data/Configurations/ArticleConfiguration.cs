using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ArticleService.Models;

namespace ArticleService.Data.Configurations;

public class ArticleConfiguration : IEntityTypeConfiguration<Article>
{
       public void Configure(EntityTypeBuilder<Article> builder)
       {
              builder.HasKey(a => a.Id);
              builder.Property(a => a.Id).ValueGeneratedNever();

              builder.Property(a => a.Title)
                     .IsRequired()
                     .HasMaxLength(200);

              builder.Property(a => a.Content)
                     .IsRequired()
                     .HasColumnType("text");

              builder.Property(a => a.ContentFormat)
                     .HasConversion<string>()
                     .HasMaxLength(20);

              builder.Property(a => a.AuthorId)
                     .IsRequired();

              builder.Property(a => a.CommentsEnabled)
                     .HasDefaultValue(true);

              builder.Property(a => a.IsDeleted)
                     .HasDefaultValue(false);

              builder.Property(a => a.DeletedAt)
                     .IsRequired(false);

              builder.Property(a => a.PermanentDeleteAt)
                     .IsRequired(false);

              builder.HasIndex(a => a.AuthorId);
              builder.HasIndex(a => a.Title);
              builder.HasIndex(a => a.CreatedAt);
              builder.HasIndex(a => a.IsAdultContent);
              builder.HasIndex(a => a.IsExplicitContent);
              builder.HasIndex(a => a.IsDeleted);
              builder.HasIndex(a => a.PermanentDeleteAt);

              builder.HasQueryFilter(a => !a.IsDeleted);

              builder.ToTable("articles");
       }
}