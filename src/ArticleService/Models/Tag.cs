using System.Text.Json.Serialization;

namespace ArticleService.Models;

public class Tag
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public List<ArticleTag> ArticleTags { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}