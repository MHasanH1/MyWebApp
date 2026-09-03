using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.DTOs;
using MyWebApp.Handlers;
using MyWebApp.Models;

namespace MyWebApp.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly ILogger<UserService> _logger;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserService(AppDbContext context, ILogger<UserService> logger, IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _logger = logger;
        _passwordHasher = passwordHasher;
    }
    
    public async Task<List<UserResponse>> GetAllAsync()
    {
        List<User> users = await _context.Users.ToListAsync();

        List<UserResponse> response = users.Select(user => new UserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Age = user.Age,
        }).ToList();

        return response;
    }

    public async Task<bool> EmailExistAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<UserResponse?> GetByIdAsync(int id)
    {
        User? user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            _logger.LogWarning("User {UserId} was not found", id);
            return null;
        }

        UserResponse userResponse = new UserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Age = user.Age,
        };

        _logger.LogInformation("User {UserId} found", id);

        return userResponse;
    }

    public async Task<UserResponse> RegisterAsync(RegisterUserRequest request)
    {
        bool EmailExist = await EmailExistAsync(request.Email);

        if (EmailExist)
        {
            throw new ConflictException("Email already exist");
        }

        User user = new User { Age = request.Age, Name = request.Name, Email = request.Email };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password); 

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        UserResponse userResponse = new UserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Age = user.Age,
        };

        _logger.LogInformation("User {UserId} registered", user.Id);

        return userResponse;
    }

    public async Task<User> CreateAsync(CreateUserRequest request)
    {
        User user = new User { Name = request.Name, Age = request.Age, Email = request.Email };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} created", user.Id);

        return user;
    }

    public async Task<UserResponse?> UpdateAsync(int id, UpdateUserRequest request)
    {
        UserResponse? user = await GetByIdAsync(id);

        if (user == null)
        {
            return null;
        }

        user.Name = request.Name;
        user.Age = request.Age;
        user.Email = request.Email;

        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} updated", id);

        return user;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        User? user = await _context.Users.FindAsync(id);

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