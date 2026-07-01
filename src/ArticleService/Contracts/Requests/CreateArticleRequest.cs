namespace ArticleService.Contracts;

public class CreateArticleRequest
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string ContentFormat { get; set; } = "Markdown";
    public bool CommentsEnabled { get; set; } = true;
    public bool IsExplicitContent { get; set; } = false;
    public bool IsAdultContent { get; set; } = false;
    public List<string> Tags { get; set; } = new();
}