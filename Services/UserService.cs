using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.DTOs;
using MyWebApp.Models;

namespace MyWebApp.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User> CreateAsync(CreateUserRequest request)
    {
        User user = new User { Name = request.Name, Age = request.Age };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

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

        return true;
    }
}