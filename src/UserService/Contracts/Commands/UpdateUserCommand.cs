using OpenMediator;

namespace UserService.Contracts;

public record UpdateUserCommand(
    Guid Id,
    string Username,
    string Email,
    string? Phone = null,
    string? Bio = null,
    DateOnly? BirthDate = null
) : ICommand<ApiResponse<UserDto>>;