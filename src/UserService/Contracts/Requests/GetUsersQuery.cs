using OpenMediator;

namespace UserService.Contracts;

public record GetUsersQuery(int Page = 1, int PageSize = 20, string? Role = null) : ICommand<ApiResponse<List<UserDto>>>;