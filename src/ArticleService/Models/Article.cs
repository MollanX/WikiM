namespace ArticleService.Models;

public class Article
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public ContentFormat ContentFormat { get; set; } = ContentFormat.Markdown;
    public Guid AuthorId { get; set; }
    public bool CommentsEnabled { get; set; } = true;
    public bool IsExplicitContent { get; set; } = false;
    public bool IsAdultContent { get; set; } = false;
    public List<ArticleTag> ArticleTags { get; set; } = new();
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public DateTime? PermanentDeleteAt { get; set; }
}

public enum ContentFormat
{
    Markdown,
    Html,
    Json
}