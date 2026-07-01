using UserService.Contracts;
using OpenMediator;

namespace UserService.Services;

public class GetUsersHandler(IUserRepository repository) : ICommandHandler<GetUsersQuery, ApiResponse<List<UserDto>>>
{
    public async Task<ApiResponse<List<UserDto>>> HandleAsync(GetUsersQuery query, CancellationToken cancellationToken)
    {
        var users = await repository.GetAllAsync(query.Page, query.PageSize, query.Role);
        var dtos = users.Select(u => u.ToDto()).ToList();
        return ApiResponse<List<UserDto>>.Ok(dtos);
    }
}