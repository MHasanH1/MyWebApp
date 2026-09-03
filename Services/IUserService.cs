using MyWebApp.DTOs;
using MyWebApp.Models;

namespace MyWebApp.Services;

public interface IUserService
{
    Task<List<UserResponse>> GetAllAsync();
    Task<bool> EmailExistAsync(string email);
    Task<UserResponse?> GetByIdAsync(int id);
    Task<UserResponse> RegisterAsync(RegisterUserRequest request);
    Task<User> CreateAsync(CreateUserRequest request);
    Task<UserResponse?> UpdateAsync(int id, UpdateUserRequest request);
    Task<bool> DeleteAsync(int id);
}