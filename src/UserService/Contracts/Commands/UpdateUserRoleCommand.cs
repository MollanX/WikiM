using OpenMediator;

namespace UserService.Contracts;

public record UpdateUserRoleCommand(Guid Id, string Role) : ICommand<ApiResponse<UserDto>>;