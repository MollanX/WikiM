using OpenMediator;

namespace UserService.Contracts;

public record DeleteUserCommand(Guid Id) : ICommand<ApiResponse>;