using UserService.Contracts;
using OpenMediator;

namespace UserService.Services;

public class DeleteUserHandler(IUserRepository repository) : ICommandHandler<DeleteUserCommand, ApiResponse>
{
    public async Task<ApiResponse> HandleAsync(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        var deleted = await repository.DeleteAsync(command.Id);
        return deleted
            ? ApiResponse.Ok("Пользователь удалён")
            : ApiResponse.Fail($"Пользователь с ID {command.Id} не найден");
    }
}