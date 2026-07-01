namespace ArticleService.Contracts;

public class UpdateArticleRequest
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string ContentFormat { get; set; } = "Markdown";
    public bool CommentsEnabled { get; set; } = true;
    public List<string> Tags { get; set; } = [];
}