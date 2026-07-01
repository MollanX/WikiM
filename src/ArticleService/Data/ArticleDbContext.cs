using ArticleService.Models;
using Microsoft.EntityFrameworkCore;

namespace ArticleService.Data;

public class ArticleDbContext : DbContext
{
    public ArticleDbContext(DbContextOptions<ArticleDbContext> options) : base(options)
    {

    }

    public DbSet<Article> Articles => Set<Article>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<ArticleTag> ArticleTags => Set<ArticleTag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ArticleDbContext).Assembly);
    }
}