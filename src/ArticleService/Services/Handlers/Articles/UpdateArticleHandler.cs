using ArticleService.Contracts;
using ArticleService.Extensions;
using ArticleService.Models;
using OpenMediator;

namespace ArticleService.Services;

public class UpdateArticleHandler(IArticleRepository repository, ITagRepository tagRepository) : ICommandHandler<UpdateArticleCommand, ApiResponse<ArticleDto>>
{
    public async Task<ApiResponse<ArticleDto>> HandleAsync(UpdateArticleCommand command, CancellationToken cancellationToken)
    {
        var article = await repository.GetByIdAsync(command.Id);
        if (article == null)
        {
            return ApiResponse<ArticleDto>.Fail($"Статья с ID {command.Id} не найдена");
        }

        article.Title = command.Title;
        article.Content = command.Content;
        article.ContentFormat = Enum.Parse<ContentFormat>(command.ContentFormat);
        article.CommentsEnabled = command.CommentsEnabled;
        article.IsExplicitContent = command.IsExplicitContent;
        article.IsAdultContent = command.IsAdultContent;
        article.UpdatedAt = DateTime.UtcNow;

        var tags = await tagRepository.GetOrCreateAsync(command.Tags);
        article.ArticleTags.Clear();
        article.ArticleTags = tags.ToArticleTags(article.Id);

        await repository.SaveChangesAsync();

        await repository.CacheArticleAsync(article);
        await repository.InvalidateListCacheAsync();

        return ApiResponse<ArticleDto>.Ok(article.ToDto(), "Статья успешно обновлена");
    }
}