using OpenMediator;

namespace ArticleService.Contracts;

public record CreateArticleCommand(
    string Title,
    string Content,
    string ContentFormat,
    bool CommentsEnabled,
    bool IsExplicitContent,
    bool IsAdultContent,
    List<string> Tags,
    Guid AuthorId
) : ICommand<ApiResponse<ArticleDto>>;