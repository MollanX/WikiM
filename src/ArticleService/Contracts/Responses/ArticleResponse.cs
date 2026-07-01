namespace ArticleService.Contracts;

public class ArticleResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string ContentFormat { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public bool CommentsEnabled { get; set; }
    public bool IsExplicitContent { get; set; }
    public bool IsAdultContent { get; set; }
    public bool IsDeleted { get; set; }
    public List<string> Tags { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}