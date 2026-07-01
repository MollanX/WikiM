using OpenMediator;

namespace ArticleService.Contracts;

public record GetArticlesQuery(int Page = 1, int PageSize = 20, bool ExcludeExplicit = false, bool ExcludeAdult = false
) : ICommand<ApiResponse<List<ArticleDto>>>;