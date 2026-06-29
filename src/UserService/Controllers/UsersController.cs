using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Models;
using UserService.Contracts;
using UserService.Services;

namespace UserService.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(IUserRepository users) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetAll()
    {
        var usersList = await users.GetAllAsync();
        return Ok(usersList);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<User>> GetById(Guid id)
    {
        var user = await users.GetByIdAsync(id);
        if (user is null) return NotFound();
        return user;
    }

    [HttpPost]
    public async Task<ActionResult<User>> Create([FromBody] CreateUserRequest request)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            CreatedAt = DateTime.UtcNow
        };

        await users.CreateAsync(user);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var deleted = await users.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}