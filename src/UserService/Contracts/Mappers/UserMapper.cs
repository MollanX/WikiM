using UserService.Models;

namespace UserService.Contracts;

public static class UserMapper
{
    public static UserDto ToDto(this User user) => new()
    {
        Id = user.Id,
        Username = user.Username,
        Email = user.Email,
        Phone = user.Phone,
        Bio = user.Bio,
        BirthDate = user.BirthDate,
        Age = user.Age,
        Role = user.Role.ToString(),
        CreatedAt = user.CreatedAt
    };
}