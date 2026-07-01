using ArticleService.Contracts;
using ArticleService.Extensions;
using ArticleService.Models;
using ArticleService.Services;
using Microsoft.AspNetCore.Mvc;

namespace ArticleService.Controllers;

[ApiController]
[Route("api/tags")]
public class TagsController(ITagRepository tagRepository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<Tag>>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        var tags = await tagRepository.GetAllAsync(page, pageSize);
        return Ok(ApiResponse<List<Tag>>.Ok(tags.ToList()));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<Tag>>> GetById(Guid id)
    {
        var tag = await tagRepository.GetByIdAsync(id);
        if (tag == null)
            return NotFound(ApiResponse<Tag>.Fail($"Тег с ID {id} не найден"));

        return Ok(ApiResponse<Tag>.Ok(tag));
    }

    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<ApiResponse<Tag>>> GetBySlug(string slug)
    {
        var tag = await tagRepository.GetBySlugAsync(slug);
        if (tag == null)
            return NotFound(ApiResponse<Tag>.Fail($"Тег '{slug}' не найден"));

        return Ok(ApiResponse<Tag>.Ok(tag));
    }

    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<List<Tag>>>> Search([FromQuery] string q, [FromQuery] int limit = 10)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest(ApiResponse<List<Tag>>.Fail("Поисковый запрос обязателен"));

        var tags = await tagRepository.SearchAsync(q, limit);
        return Ok(ApiResponse<List<Tag>>.Ok(tags.ToList()));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<Tag>>> Create([FromBody] CreateTagRequest request)
    {
        try
        {
            var tag = new Tag
            {
                Name = request.Name,
                Slug = request.Slug ?? request.Name.ToSlug()
            };

            var created = await tagRepository.CreateAsync(tag);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<Tag>.Ok(created));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<Tag>.Fail(ex.Message));
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<Tag>>> Update(Guid id, [FromBody] UpdateTagRequest request)
    {
        try
        {
            var tag = new Tag
            {
                Id = id,
                Name = request.Name,
                Slug = request.Slug ?? request.Name.ToSlug()
            };

            var updated = await tagRepository.UpdateAsync(tag);
            if (updated == null)
                return NotFound(ApiResponse<Tag>.Fail($"Тег с ID {id} не найден"));

            return Ok(ApiResponse<Tag>.Ok(updated));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<Tag>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse>> Delete(Guid id)
    {
        try
        {
            var deleted = await tagRepository.DeleteAsync(id);
            if (!deleted)
                return NotFound(ApiResponse.Fail($"Тег с ID {id} не найден"));

            return Ok(ApiResponse.Ok("Тег удалён"));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse.Fail(ex.Message));
        }
    }
}

// DTO для запросов
public record CreateTagRequest(string Name, string? Slug = null);
public record UpdateTagRequest(string Name, string? Slug = null);