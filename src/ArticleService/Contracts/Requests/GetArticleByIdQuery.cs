using OpenMediator;

namespace ArticleService.Contracts;

public record GetArticleByIdQuery(Guid Id, bool IncludeDeleted = false) : ICommand<ApiResponse<ArticleDto>>;