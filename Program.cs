using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.DTOs;
using MyWebApp.Filters;
using MyWebApp.Handlers;
using MyWebApp.Models;
using MyWebApp.Services;
using MyWebApp.Validators;

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

var app = builder.Build();

app.UseExceptionHandler();

app.MapGet("/", () => "Hello!");

app.MapGet("/users", async (IUserService userService) =>
{
  List<User> users = await userService.GetAllAsync();

  return Results.Ok(new
  {
    data = users,
    message = "users fetched"
  });
});

app.MapGet("/users/{id}", async (int id, IUserService userService) => {
  User? user = await userService.GetByIdAsync(id);

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
  User? updatedUser = await userService.UpdateAsync(id, request);

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
});


app.Run();
