using ArticleService.Models;

namespace ArticleService.Services;

public interface IArticleRepository
{
    Task<Article?> GetByIdAsync(Guid id, bool includeDeleted = false);
    Task<IEnumerable<Article>> GetAllAsync(int page, int pageSize, bool excludeExplicit, bool excludeAdult);
    Task AddAsync(Article article);
    Task SaveChangesAsync();
    Task<bool> SoftDeleteAsync(Guid id);
    Task<bool> PermanentDeleteAsync(Guid id);
    Task<IEnumerable<Article>> GetByTagAsync(string tagSlug, int page, int pageSize);
    Task<IEnumerable<Article>> GetExpiredForDeletionAsync(DateTime threshold);
    Task<int> PermanentDeleteRangeAsync(IEnumerable<Article> articles);

    // Кэш
    Task CacheArticleAsync(Article article);
    Task InvalidateArticleCacheAsync(Guid id);
    Task InvalidateListCacheAsync();
    Task<T?> GetFromCacheAsync<T>(string key) where T : class;
    Task SetListCacheAsync<T>(string key, T data) where T : class;
}