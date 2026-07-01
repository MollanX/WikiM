using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Models;

namespace UserService.Services;

public class UserRepository(UserDbContext context, ILogger<UserRepository> logger) : IUserRepository
{
    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await context.Users.FindAsync(id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant());
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<IEnumerable<User>> GetAllAsync(int page, int pageSize, string? role = null)
    {
        IQueryable<User> query = context.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(role) && Enum.TryParse<UserRole>(role, true, out var parsedRole))
            query = query.Where(u => u.Role == parsedRole);

        return await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task AddAsync(User user)
    {
        context.Users.Add(user);
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var user = await context.Users.FindAsync(id);
        if (user == null) return false;

        context.Users.Remove(user);
        await context.SaveChangesAsync();

        logger.LogInformation("Deleted user: {UserId}", id);
        return true;
    }

    public async Task<bool> ExistsByEmailAsync(string email, Guid? excludeId = null)
    {
        var query = context.Users.Where(u => u.Email == email.ToLowerInvariant());
        if (excludeId.HasValue)
            query = query.Where(u => u.Id != excludeId.Value);

        return await query.AnyAsync();
    }

    public async Task<bool> ExistsByUsernameAsync(string username, Guid? excludeId = null)
    {
        var query = context.Users.Where(u => u.Username == username);
        if (excludeId.HasValue)
            query = query.Where(u => u.Id != excludeId.Value);

        return await query.AnyAsync();
    }
}