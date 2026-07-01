using ArticleService.Contracts;
using OpenMediator;

namespace ArticleService.Services;

public class GetArticleByIdHandler(IArticleRepository repository) : ICommandHandler<GetArticleByIdQuery, ApiResponse<ArticleDto>>
{
    public async Task<ApiResponse<ArticleDto>> HandleAsync(GetArticleByIdQuery query, CancellationToken cancellationToken)
    {
        var article = await repository.GetByIdAsync(query.Id, query.IncludeDeleted);

        if (article == null)
        {
            return ApiResponse<ArticleDto>.Fail($"Статья с ID {query.Id} не найдена");
        }

        return ApiResponse<ArticleDto>.Ok(article.ToDto());
    }
}