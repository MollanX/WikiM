using ArticleService.Contracts;
using ArticleService.Extensions;
using ArticleService.Models;
using OpenMediator;

namespace ArticleService.Services.Handlers;

public class CreateArticleHandler(IArticleRepository articleRepository, ITagRepository tagRepository) : ICommandHandler<CreateArticleCommand, ApiResponse<ArticleDto>>
{
    public async Task<ApiResponse<ArticleDto>> HandleAsync(CreateArticleCommand command, CancellationToken cancellationToken)
    {
        var tags = await tagRepository.GetOrCreateAsync(command.Tags);

        var articleId = Guid.CreateVersion7();
        var now = DateTime.UtcNow;

        var article = new Article
        {
            Id = articleId,
            Title = command.Title,
            Content = command.Content,
            ContentFormat = Enum.Parse<ContentFormat>(command.ContentFormat),
            AuthorId = command.AuthorId,
            CommentsEnabled = command.CommentsEnabled,
            IsExplicitContent = command.IsExplicitContent,
            IsAdultContent = command.IsAdultContent,
            CreatedAt = now,
            UpdatedAt = now,
            ArticleTags = tags.ToArticleTags(articleId)
        };

        await articleRepository.AddAsync(article);
        await articleRepository.SaveChangesAsync();

        await articleRepository.CacheArticleAsync(article);
        await articleRepository.InvalidateListCacheAsync();

        return ApiResponse<ArticleDto>.Ok(article.ToDto(), "Статья успешно создана");
    }
}