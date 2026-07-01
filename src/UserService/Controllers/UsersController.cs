using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OpenMediator.Buses;
using UserService.Contracts;

namespace UserService.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(IMediatorBus mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<UserDto>>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? role = null)
    {
        var query = new GetUsersQuery(page, pageSize, role);
        var result = await mediator.SendAsync<GetUsersQuery, ApiResponse<List<UserDto>>>(query);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetById(Guid id)
    {
        var query = new GetUserByIdQuery(id);
        var result = await mediator.SendAsync<GetUserByIdQuery, ApiResponse<UserDto>>(query);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<UserDto>>> Create([FromBody] CreateUserCommand command, [FromServices] IValidator<CreateUserCommand> validator)
    {
        var validationResult = await validator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse<UserDto>.Fail("Ошибка валидации", errors));
        }

        var result = await mediator.SendAsync<CreateUserCommand, ApiResponse<UserDto>>(command);
        return result.Success
            ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result)
            : Conflict(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<UserDto>>> Update(Guid id, [FromBody] UpdateUserCommand command, [FromServices] IValidator<UpdateUserCommand> validator)
    {
        var validationResult = await validator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse<UserDto>.Fail("Ошибка валидации", errors));
        }

        var commandWithId = command with { Id = id };
        var result = await mediator.SendAsync<UpdateUserCommand, ApiResponse<UserDto>>(commandWithId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse>> Delete(Guid id)
    {
        var command = new DeleteUserCommand(id);
        var result = await mediator.SendAsync<DeleteUserCommand, ApiResponse>(command);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPatch("{id:guid}/role")]
    public async Task<ActionResult<ApiResponse<UserDto>>> UpdateRole(Guid id, [FromBody] UpdateUserRoleCommand command)
    {
        var commandWithId = command with { Id = id };
        var result = await mediator.SendAsync<UpdateUserRoleCommand, ApiResponse<UserDto>>(commandWithId);
        return result.Success ? Ok(result) : NotFound(result);
    }
}