using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyWebApp.Data;
using MyWebApp.DTOs;
using MyWebApp.Filters;
using MyWebApp.Handlers;
using MyWebApp.Models;
using MyWebApp.Services;
using MyWebApp.Validators;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
  options.UseNpgsql(
    builder.Configuration.GetConnectionString("DefaultConnection")
  );
});
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["Jwt:SecretKey"]!
                )
            )
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddScoped<ITokenService, TokenService>();

var app = builder.Build();

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello!");

app.MapGet("/users", async (IUserService userService) =>
{
  List<UserResponse> users = await userService.GetAllAsync();

  return Results.Ok(new
  {
    data = users,
    message = "users fetched"
  });
})
.RequireAuthorization();

app.MapGet("/users/{id}", async (int id, IUserService userService) => {
  UserResponse? user = await userService.GetByIdAsync(id);

  if (user == null)
  {
    return Results.NotFound(new
    {
      message = "User not found",
    });
  }

  return Results.Ok(new
  {
    data = user,
    message = "OK"
  });
});

app.MapGet("/me", (HttpContext httpContext) =>
{
  var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
  var email  = httpContext.User.FindFirst(ClaimTypes.Email)?.Value;
  var role  = httpContext.User.FindFirst(ClaimTypes.Role)?.Value;

  return Results.Ok(new
  {
    userId,
    email,
    role
  });
})
.RequireAuthorization();

app.MapPost("/auth/register", async (RegisterUserRequest request, IUserService userService) =>
{
  UserResponse user = await userService.RegisterAsync(request);

  return Results.Created($"/users/${user.Id}", new {
    message = "User registered",
    data = new
    {
      user.Id,
      user.Name,
      user.Age,
      user.Email
    } 
  });
})
.AddEndpointFilter<ValidationFilter<RegisterUserRequest>>();

app.MapPost("/auth/login", async (LoginRequest request, IUserService userService, ITokenService tokenService) =>
{
  User? user = await userService.LoginAsync(request);

  if (user == null)
  {
    return Results.Unauthorized();
  }

  string token = tokenService.CreateToken(user.Id, user.Email, user.Role);

  return Results.Ok(new
  {
    data = new
    {
      user = new UserResponse
    {
      Id = user.Id,
      Name = user.Name,
      Email = user.Email,
      Age = user.Age
    },
    token
    },
    message = "Login successful"
  });
})
.AddEndpointFilter<ValidationFilter<LoginRequest>>();

app.MapPost("/users", async (CreateUserRequest request, IUserService userService) =>
{
  User createdUser = await userService.CreateAsync(request);

  return Results.Created($"/users/{createdUser.Id}", new
  {
    data = createdUser,
    message = "User created"
  });
})
.AddEndpointFilter<ValidationFilter<CreateUserRequest>>();

app.MapPut("/users/{id}", async (int id, UpdateUserRequest request, IUserService userService) =>
{
  UserResponse? updatedUser = await userService.UpdateAsync(id, request);

  if (updatedUser == null) return Results.NotFound(new
  {
    message = "User not found"
  });

  return Results.Ok(new {
    data = updatedUser,
    message = "User updated"
  });
})
.AddEndpointFilter<ValidationFilter<UpdateUserRequest>>();

app.MapDelete("/users/{id}", async (int id, IUserService userService) =>
{
  bool deleted = await userService.DeleteAsync(id);

  if (!deleted) return Results.NotFound(new
  {
    message = "User not found"
  });

  return Results.NoContent();
})
.RequireAuthorization(policy => policy.RequireRole("Admin"));

app.Run();
