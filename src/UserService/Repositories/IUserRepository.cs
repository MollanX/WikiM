using UserService.Models;

namespace UserService.Services;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUsernameAsync(string username);
    Task<IEnumerable<User>> GetAllAsync(int page, int pageSize, string? role = null);
    Task AddAsync(User user);
    Task SaveChangesAsync();
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsByEmailAsync(string email, Guid? excludeId = null);
    Task<bool> ExistsByUsernameAsync(string username, Guid? excludeId = null);
}