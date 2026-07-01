using OpenMediator;

namespace ArticleService.Contracts;

public record UpdateArticleCommand(
    Guid Id,
    string Title,
    string Content,
    string ContentFormat,
    bool CommentsEnabled,
    bool IsExplicitContent,
    bool IsAdultContent,
    List<string> Tags
) : ICommand<ApiResponse<ArticleDto>>;