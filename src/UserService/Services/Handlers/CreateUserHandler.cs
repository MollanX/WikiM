using UserService.Contracts;
using UserService.Models;
using OpenMediator;

namespace UserService.Services;

public class CreateUserHandler(IUserRepository repository) : ICommandHandler<CreateUserCommand, ApiResponse<UserDto>>
{
    public async Task<ApiResponse<UserDto>> HandleAsync(CreateUserCommand command, CancellationToken cancellationToken)
    {
        if (await repository.ExistsByEmailAsync(command.Email))
            return ApiResponse<UserDto>.Fail("Пользователь с таким email уже существует");

        if (await repository.ExistsByUsernameAsync(command.Username))
            return ApiResponse<UserDto>.Fail("Пользователь с таким именем уже существует");

        var user = new User
        {
            Id = Guid.CreateVersion7(),
            Username = command.Username,
            Email = command.Email.ToLowerInvariant(),
            Phone = command.Phone,
            Bio = command.Bio,
            BirthDate = command.BirthDate,
            Role = UserRole.User,
            CreatedAt = DateTime.UtcNow
        };

        await repository.AddAsync(user);
        await repository.SaveChangesAsync();

        return ApiResponse<UserDto>.Ok(user.ToDto(), "Пользователь успешно создан");
    }
}