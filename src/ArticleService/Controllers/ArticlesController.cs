using ArticleService.Contracts;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OpenMediator.Buses;

namespace ArticleService.Controllers;

[ApiController]
[Route("api/articles")]
public class ArticlesController(IMediatorBus mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<ArticleDto>>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool excludeExplicit = false,
        [FromQuery] bool excludeAdult = false)
    {
        var query = new GetArticlesQuery(page, pageSize, excludeExplicit, excludeAdult);
        var result = await mediator.SendAsync<GetArticlesQuery, ApiResponse<List<ArticleDto>>>(query);

        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ArticleDto>>> GetById(Guid id, [FromQuery] bool includeDeleted = false)
    {
        var query = new GetArticleByIdQuery(id, includeDeleted);
        var result = await mediator.SendAsync<GetArticleByIdQuery, ApiResponse<ArticleDto>>(query);

        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ArticleDto>>> Create([FromBody] CreateArticleCommand command, [FromServices] IValidator<CreateArticleCommand> validator)
    {
        // Валидация
        var validationResult = await validator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse<ArticleDto>.Fail("Ошибка валидации", errors));
        }

        // AuthorId пока заглушка, потом из JWT
        var commandWithAuthor = command with { AuthorId = Guid.CreateVersion7() };
        var result = await mediator.SendAsync<CreateArticleCommand, ApiResponse<ArticleDto>>(commandWithAuthor);

        return result.Success
            ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result)
            : BadRequest(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ArticleDto>>> Update(Guid id, [FromBody] UpdateArticleCommand command, [FromServices] IValidator<UpdateArticleCommand> validator)
    {
        // Валидация
        var validationResult = await validator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse<ArticleDto>.Fail("Ошибка валидации", errors));
        }

        var commandWithId = command with { Id = id };
        var result = await mediator.SendAsync<UpdateArticleCommand, ApiResponse<ArticleDto>>(commandWithId);

        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse>> Delete(Guid id, [FromQuery] bool permanent = false)
    {
        var command = new DeleteArticleCommand(id, permanent);
        var result = await mediator.SendAsync<DeleteArticleCommand, ApiResponse>(command);

        return result.Success ? Ok(result) : NotFound(result);
    }
}