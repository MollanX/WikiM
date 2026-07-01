using OpenMediator;

namespace UserService.Contracts;

public record GetUserByIdQuery(Guid Id) : ICommand<ApiResponse<UserDto>>;