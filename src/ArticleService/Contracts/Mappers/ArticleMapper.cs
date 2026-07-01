using ArticleService.Models;

namespace ArticleService.Contracts;

public static class ArticleMapper
{
    public static ArticleDto ToDto(this Article article) => new()
    {
        Id = article.Id,
        Title = article.Title,
        Content = article.Content,
        ContentFormat = article.ContentFormat.ToString(),
        AuthorId = article.AuthorId,
        CommentsEnabled = article.CommentsEnabled,
        IsExplicitContent = article.IsExplicitContent,
        IsAdultContent = article.IsAdultContent,
        IsDeleted = article.IsDeleted,
        Tags = article.ArticleTags.Select(at => at.Tag.Name).ToList(),
        CreatedAt = article.CreatedAt,
        UpdatedAt = article.UpdatedAt
    };

    public static Article ToEntity(this CreateArticleCommand request) => new()
    {
        Title = request.Title,
        Content = request.Content,
        ContentFormat = Enum.Parse<ContentFormat>(request.ContentFormat),
        CommentsEnabled = request.CommentsEnabled,
        IsExplicitContent = request.IsExplicitContent,
        IsAdultContent = request.IsAdultContent
    };
}