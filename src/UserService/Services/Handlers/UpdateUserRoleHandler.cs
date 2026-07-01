using UserService.Contracts;
using UserService.Models;
using OpenMediator;

namespace UserService.Services;

public class UpdateUserRoleHandler(IUserRepository repository) : ICommandHandler<UpdateUserRoleCommand, ApiResponse<UserDto>>
{
    public async Task<ApiResponse<UserDto>> HandleAsync(UpdateUserRoleCommand command, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<UserRole>(command.Role, true, out var role))
            return ApiResponse<UserDto>.Fail($"Неизвестная роль: {command.Role}");

        var user = await repository.GetByIdAsync(command.Id);
        if (user == null)
            return ApiResponse<UserDto>.Fail($"Пользователь с ID {command.Id} не найден");

        user.Role = role;
        user.UpdatedAt = DateTime.UtcNow;

        await repository.SaveChangesAsync();

        return ApiResponse<UserDto>.Ok(user.ToDto(), $"Роль изменена на {role}");
    }
}