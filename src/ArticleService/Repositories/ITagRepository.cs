using ArticleService.Models;

namespace ArticleService.Services;

public interface ITagRepository
{
    Task<IEnumerable<Tag>> GetAllAsync(int page = 1, int pageSize = 50);
    Task<Tag?> GetByIdAsync(Guid id);
    Task<Tag?> GetBySlugAsync(string slug);
    Task<Tag> CreateAsync(Tag tag);
    Task<Tag?> UpdateAsync(Tag tag);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsBySlugAsync(string slug);
    Task<int> GetArticleCountAsync(Guid tagId);
    Task<IEnumerable<Tag>> SearchAsync(string searchTerm, int limit = 10);
    Task<IEnumerable<Tag>> GetOrCreateAsync(List<string> tagNames);
}