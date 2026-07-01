using OpenMediator;

namespace ArticleService.Contracts;

public record DeleteArticleCommand(Guid Id, bool Permanent = false) : ICommand<ApiResponse>;