using ArticleService.Models;

namespace ArticleService.Extensions;

public static class TagExtensions
{
    /// <summary>
    /// Преобразует список тегов в список связей ArticleTag для указанной статьи.
    /// </summary>
    public static List<ArticleTag> ToArticleTags(this IEnumerable<Tag> tags, Guid articleId)
        => tags.Select(tag => new ArticleTag
        {
            ArticleId = articleId,
            TagId = tag.Id
        }).ToList();

    /// <summary>
    /// Нормализует строку в slug для использования в URL.
    /// "C# Programming" → "csharp-programming"
    /// </summary>
    public static string ToSlug(this string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return string.Empty;

        return name.Trim()
                   .ToLowerInvariant()
                   .Replace(" ", "-")
                   .Replace("#", "sharp")
                   .Replace(".", "-")
                   .Replace("+", "-plus")
                   .Replace("--", "-"); // Убираем двойные дефисы
    }
}