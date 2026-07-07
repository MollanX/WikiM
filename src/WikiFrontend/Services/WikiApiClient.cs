using System.Net.Http.Json;
using WikiFrontend.Models;

namespace WikiFrontend.Services;

public class WikiApiClient(HttpClient http)
{
    public async Task<List<ArticleDto>> GetArticlesAsync(int page = 1, int pageSize = 20)
    {
        var response = await http.GetFromJsonAsync<ApiResponse<List<ArticleDto>>>(
            $"api/articles?page={page}&pageSize={pageSize}");
        return response?.Data ?? new();
    }

    public async Task<ArticleDto?> GetArticleAsync(Guid id)
    {
        var response = await http.GetFromJsonAsync<ApiResponse<ArticleDto>>($"api/articles/{id}");
        return response?.Data;
    }

    public async Task<List<TagDto>> GetTagsAsync(int page = 1, int pageSize = 50)
    {
        var response = await http.GetFromJsonAsync<ApiResponse<List<TagDto>>>(
            $"api/tags?page={page}&pageSize={pageSize}");
        return response?.Data ?? new();
    }
}