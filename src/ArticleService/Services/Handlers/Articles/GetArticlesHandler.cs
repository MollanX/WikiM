using ArticleService.Contracts;
using ArticleService.Services;
using OpenMediator;

namespace ArticleService.Services.Handlers;

public class GetArticlesHandler(IArticleRepository repository) : ICommandHandler<GetArticlesQuery, ApiResponse<List<ArticleDto>>>
{
    public async Task<ApiResponse<List<ArticleDto>>> HandleAsync(GetArticlesQuery query, CancellationToken cancellationToken)
    {
        var articles = await repository.GetAllAsync(query.Page, query.PageSize, query.ExcludeExplicit, query.ExcludeAdult);

        var dtos = articles.Select(a => a.ToDto()).ToList();
        return ApiResponse<List<ArticleDto>>.Ok(dtos);
    }
}