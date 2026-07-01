using UserService.Contracts;
using OpenMediator;

namespace UserService.Services;

public class GetUserByIdHandler(IUserRepository repository) : ICommandHandler<GetUserByIdQuery, ApiResponse<UserDto>>
{
    public async Task<ApiResponse<UserDto>> HandleAsync(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        var user = await repository.GetByIdAsync(query.Id);
        if (user == null)
            return ApiResponse<UserDto>.Fail($"Пользователь с ID {query.Id} не найден");

        return ApiResponse<UserDto>.Ok(user.ToDto());
    }
}