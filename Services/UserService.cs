using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.DTOs;
using MyWebApp.Models;

namespace MyWebApp.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly ILogger<UserService> _logger;

    public UserService(AppDbContext context, ILogger<UserService> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task<List<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        User? user = await _context.Users.FindAsync(id);

        if (user is null)
        {
            _logger.LogWarning("User {UserId} was not found", id);
            return null;
        }

        _logger.LogInformation("User {UserId} found", id);

        return user;
    }

    public async Task<User> CreateAsync(CreateUserRequest request)
    {
        User user = new User { Name = request.Name, Age = request.Age };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} created", user.Id);

        return user;
    }

    public async Task<User?> UpdateAsync(int id, UpdateUserRequest request)
    {
        User? user = await GetByIdAsync(id);

        if (user == null)
        {
            return null;
        }

        user.Name = request.Name;
        user.Age = request.Age;

        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} updated", id);

        return user;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        User? user = await GetByIdAsync(id);

        if (user == null)
        {
            return false;
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} deleted", id);

        return true;
    }
}