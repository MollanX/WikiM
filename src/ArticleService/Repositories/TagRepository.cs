using Microsoft.EntityFrameworkCore;
using ArticleService.Data;
using ArticleService.Models;
using ArticleService.Extensions;

namespace ArticleService.Services;

public class TagRepository(ArticleDbContext context, ILogger<TagRepository> logger) : ITagRepository
{
    public async Task<IEnumerable<Tag>> GetAllAsync(int page = 1, int pageSize = 50)
    {
        return await context.Tags
            .AsNoTracking()
            .OrderBy(t => t.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Tag?> GetByIdAsync(Guid id)
    {
        return await context.Tags.FindAsync(id);
    }

    public async Task<Tag?> GetBySlugAsync(string slug)
    {
        return await context.Tags
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Slug == slug.ToLowerInvariant());
    }

    public async Task<Tag> CreateAsync(Tag tag)
    {
        // Проверяем уникальность slug
        if (await ExistsBySlugAsync(tag.Slug))
            throw new InvalidOperationException($"Тег с slug '{tag.Slug}' уже существует");

        tag.Id = Guid.CreateVersion7();
        tag.CreatedAt = DateTime.UtcNow;

        context.Tags.Add(tag);
        await context.SaveChangesAsync();

        logger.LogInformation("Created tag: {TagName} ({TagSlug})", tag.Name, tag.Slug);
        return tag;
    }

    public async Task<Tag?> UpdateAsync(Tag tag)
    {
        var existing = await context.Tags.FindAsync(tag.Id);
        if (existing == null) return null;

        // Если slug меняется — проверяем уникальность
        if (existing.Slug != tag.Slug && await ExistsBySlugAsync(tag.Slug))
            throw new InvalidOperationException($"Тег с slug '{tag.Slug}' уже существует");

        existing.Name = tag.Name;
        existing.Slug = tag.Slug;

        await context.SaveChangesAsync();

        logger.LogInformation("Updated tag: {TagId}", tag.Id);
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var tag = await context.Tags
            .Include(t => t.ArticleTags)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (tag == null) return false;

        // Проверяем, используется ли тег
        if (tag.ArticleTags.Count > 0)
            throw new InvalidOperationException(
                $"Нельзя удалить тег '{tag.Name}': он используется в {tag.ArticleTags.Count} статьях");

        context.Tags.Remove(tag);
        await context.SaveChangesAsync();

        logger.LogInformation("Deleted tag: {TagName}", tag.Name);
        return true;
    }

    public async Task<bool> ExistsBySlugAsync(string slug)
    {
        return await context.Tags.AnyAsync(t => t.Slug == slug.ToLowerInvariant());
    }

    public async Task<int> GetArticleCountAsync(Guid tagId)
    {
        return await context.ArticleTags.CountAsync(at => at.TagId == tagId);
    }

    public async Task<IEnumerable<Tag>> SearchAsync(string searchTerm, int limit = 10)
    {
        var term = searchTerm.ToLowerInvariant();

        return await context.Tags
            .AsNoTracking()
            .Where(t => t.Name.ToLower().Contains(term) || t.Slug.Contains(term))
            .OrderBy(t => t.Name)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tag>> GetOrCreateAsync(List<string> tagNames)
    {
        var tags = new List<Tag>();

        foreach (var name in tagNames.Select(n => n.Trim()).Where(n => !string.IsNullOrWhiteSpace(n)))
        {
            var slug = name.ToSlug();
            var tag = await context.Tags.FirstOrDefaultAsync(t => t.Slug == slug);

            if (tag == null)
            {
                tag = new Tag
                {
                    Id = Guid.CreateVersion7(),
                    Name = name,
                    Slug = slug,
                    CreatedAt = DateTime.UtcNow
                };
                context.Tags.Add(tag);
                logger.LogDebug("Auto-created tag: {TagName}", name);
            }

            tags.Add(tag);
        }

        await context.SaveChangesAsync();
        return tags.DistinctBy(t => t.Id);
    }
}