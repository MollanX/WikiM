using UserService.Contracts;
using OpenMediator;

namespace UserService.Services;

public class UpdateUserHandler(IUserRepository repository) : ICommandHandler<UpdateUserCommand, ApiResponse<UserDto>>
{
    public async Task<ApiResponse<UserDto>> HandleAsync(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var user = await repository.GetByIdAsync(command.Id);
        if (user == null)
            return ApiResponse<UserDto>.Fail($"Пользователь с ID {command.Id} не найден");

        if (await repository.ExistsByEmailAsync(command.Email, command.Id))
            return ApiResponse<UserDto>.Fail("Пользователь с таким email уже существует");

        if (await repository.ExistsByUsernameAsync(command.Username, command.Id))
            return ApiResponse<UserDto>.Fail("Пользователь с таким именем уже существует");

        user.Username = command.Username;
        user.Email = command.Email.ToLowerInvariant();
        user.Phone = command.Phone;
        user.Bio = command.Bio;
        user.BirthDate = command.BirthDate;
        user.UpdatedAt = DateTime.UtcNow;

        await repository.SaveChangesAsync();

        return ApiResponse<UserDto>.Ok(user.ToDto(), "Профиль обновлён");
    }
}