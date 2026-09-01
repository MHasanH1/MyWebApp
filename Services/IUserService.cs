using MyWebApp.DTOs;
using MyWebApp.Models;

namespace MyWebApp.Services;

public interface IUserService
{
    Task<List<User>> GetAllAsync();

    Task<User?> GetByIdAsync(int id);

    Task<User> CreateAsync(CreateUserRequest request);

    Task<User?> UpdateAsync(int id, UpdateUserRequest request);

    Task<bool> DeleteAsync(int id);
}