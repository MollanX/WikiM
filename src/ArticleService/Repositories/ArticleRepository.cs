using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Text.Json;
using ArticleService.Data;
using ArticleService.Models;
using System.Text.Json.Serialization;

namespace ArticleService.Services;

public class ArticleRepository(ArticleDbContext context, IConnectionMultiplexer redis, ILogger<ArticleRepository> logger) : IArticleRepository
{
    private readonly IDatabase _cache = redis.GetDatabase();
    private const int CacheExpiryMinutes = 10;
    private const string ListKeysSet = "cache:article-list-keys";

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        WriteIndented = false
    };

    // === Чтение ===

    public async Task<Article?> GetByIdAsync(Guid id, bool includeDeleted = false)
    {
        var cacheKey = $"article:{id}";

        if (!includeDeleted)
        {
            var cached = await _cache.StringGetAsync(cacheKey);
            if (!cached.IsNullOrEmpty)
            {
                logger.LogDebug("Cache HIT: {Key}", cacheKey);
                return JsonSerializer.Deserialize<Article>(cached.ToString(), _jsonOptions);
            }
        }

        IQueryable<Article> query = context.Articles
            .Include(a => a.ArticleTags)
            .ThenInclude(at => at.Tag);

        if (includeDeleted)
            query = query.IgnoreQueryFilters();

        var article = await query.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);

        if (article != null && !article.IsDeleted)
            await CacheArticleAsync(article);

        return article;
    }

    public async Task<IEnumerable<Article>> GetAllAsync(int page, int pageSize, bool excludeExplicit, bool excludeAdult)
    {
        var cacheKey = $"articles:all:p{page}:s{pageSize}:e{excludeExplicit}:a{excludeAdult}";

        var cached = await GetFromCacheAsync<List<Article>>(cacheKey);
        if (cached != null) return cached;

        IQueryable<Article> query = context.Articles
            .Include(a => a.ArticleTags)
            .ThenInclude(at => at.Tag)
            .AsNoTracking();

        if (excludeExplicit)
            query = query.Where(a => !a.IsExplicitContent);
        if (excludeAdult)
            query = query.Where(a => !a.IsAdultContent);

        var articles = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        await SetListCacheAsync(cacheKey, articles);
        return articles;
    }

    public async Task<IEnumerable<Article>> GetByTagAsync(string tagSlug, int page, int pageSize)
    {
        var cacheKey = $"articles:tag:{tagSlug}:p{page}:s{pageSize}";

        var cached = await GetFromCacheAsync<List<Article>>(cacheKey);
        if (cached != null) return cached;

        var articles = await context.Articles
            .Include(a => a.ArticleTags)
            .ThenInclude(at => at.Tag)
            .Where(a => a.ArticleTags.Any(at => at.Tag.Slug == tagSlug))
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        await SetListCacheAsync(cacheKey, articles);
        return articles;
    }

    // === Запись ===

    public async Task AddAsync(Article article)
    {
        context.Articles.Add(article);
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }

    public async Task<bool> SoftDeleteAsync(Guid id)
    {
        var article = await context.Articles
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(a => a.Id == id);

        if (article == null) return false;

        article.IsDeleted = true;
        article.DeletedAt = DateTime.UtcNow;
        article.PermanentDeleteAt = DateTime.UtcNow.AddDays(90);
        article.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> PermanentDeleteAsync(Guid id)
    {
        var article = await context.Articles
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(a => a.Id == id);

        if (article == null) return false;

        context.Articles.Remove(article);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Article>> GetExpiredForDeletionAsync(DateTime threshold)
    {
        return await context.Articles
            .IgnoreQueryFilters()
            .Where(a => a.IsDeleted && a.PermanentDeleteAt <= threshold)
            .ToListAsync();
    }

    public async Task<int> PermanentDeleteRangeAsync(IEnumerable<Article> articles)
    {
        context.Articles.RemoveRange(articles);
        var count = await context.SaveChangesAsync();
        logger.LogInformation("Permanently deleted {Count} expired articles", count);
        return count;
    }

    // === Теги ===

    public async Task<IEnumerable<Tag>> GetOrCreateTagsAsync(List<string> tagNames)
    {
        var tags = new List<Tag>();

        foreach (var name in tagNames.Select(n => n.Trim()).Where(n => !string.IsNullOrWhiteSpace(n)))
        {
            var slug = NormalizeSlug(name);
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
            }

            tags.Add(tag);
        }

        await context.SaveChangesAsync();
        return tags.DistinctBy(t => t.Id);
    }

    // === Кэш (публичные методы) ===

    public async Task CacheArticleAsync(Article article)
    {
        var key = $"article:{article.Id}";
        await _cache.StringSetAsync(key, JsonSerializer.Serialize(article, _jsonOptions),
            TimeSpan.FromMinutes(CacheExpiryMinutes));
        logger.LogDebug("Cache SET: {Key}", key);
    }

    public async Task InvalidateArticleCacheAsync(Guid id)
    {
        await _cache.KeyDeleteAsync($"article:{id}");
        logger.LogDebug("Cache INVALIDATE: article:{Id}", id);
    }

    public async Task InvalidateListCacheAsync()
    {
        var keys = await _cache.SetMembersAsync(ListKeysSet);
        if (keys.Length > 0)
        {
            var redisKeys = keys.Select(k => (RedisKey)k.ToString()).ToArray();
            await _cache.KeyDeleteAsync(redisKeys);
            await _cache.KeyDeleteAsync(ListKeysSet);
            logger.LogDebug("Invalidated {Count} list cache keys", keys.Length);
        }
    }

    public async Task<T?> GetFromCacheAsync<T>(string key) where T : class
    {
        var cached = await _cache.StringGetAsync(key);
        if (cached.IsNullOrEmpty) return null;

        logger.LogDebug("Cache HIT: {Key}", key);
        return JsonSerializer.Deserialize<T>(cached.ToString(), _jsonOptions);
    }

    public async Task SetListCacheAsync<T>(string key, T data) where T : class
    {
        await _cache.StringSetAsync(key, JsonSerializer.Serialize(data, _jsonOptions),
            TimeSpan.FromMinutes(CacheExpiryMinutes));
        await _cache.SetAddAsync(ListKeysSet, key);
        logger.LogDebug("Cache SET: {Key} (list)", key);
    }

    // === Приватные ===

    private static string NormalizeSlug(string name)
    {
        return name.ToLowerInvariant()
                   .Replace(" ", "-")
                   .Replace("#", "sharp")
                   .Replace(".", "-")
                   .Replace("+", "-plus");
    }
}