using System.Text.Json.Serialization;

namespace ArticleService.Models;

public class ArticleTag
{
    public Guid ArticleId { get; set; }
    [JsonIgnore]
    public Article Article { get; set; } = null!;

    public Guid TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}