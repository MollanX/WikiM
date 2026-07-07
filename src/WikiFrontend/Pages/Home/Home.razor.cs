using WikiFrontend.Models;

namespace WikiFrontend.Pages;

public partial class Home
{
    private List<ArticleDto>? articles;
    private List<TagDto>? tags;

    protected override async Task OnInitializedAsync()
    {
        articles = await Api.GetArticlesAsync();
        tags = await Api.GetTagsAsync();
    }

    private static string StripMarkdown(string content)
    {
        if (string.IsNullOrWhiteSpace(content)) return "";
        // Просто убираем базовые markdown-символы для превью
        var text = System.Text.RegularExpressions.Regex.Replace(content, @"[#*`>\[\]()!_~]", "");
        return text.Length > 150 ? text[..150] + "..." : text;
    }
}