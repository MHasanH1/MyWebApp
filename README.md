# MyWebApp

A simple ASP.NET Core Web API project built with C# and .NET 10.

## Tech Stack

- C#
- .NET 10
- ASP.NET Core Minimal API
- Entity Framework Core
- PostgreSQL
- FluentValidation

## Project Structure

```text
MyWebApp/
├── Data/
├── DTOs/
├── Filters/
├── Handlers/
├── Migrations/
├── Models/
├── Services/
├── Validators/
├── Program.cs
└── MyWebApp.csproj
```

## Features

- User CRUD operations
- PostgreSQL database integration
- Entity Framework Core
- Dependency Injection
- DTOs for request models
- FluentValidation
- Endpoint Filters for request validation
- Global exception handling
- Async database operations
- EF Core migrations
- Configuration through .NET User Secrets

## API Endpoints

| Method | Endpoint      | Description           |
| ------ | ------------- | --------------------- |
| GET    | `/`           | Health/basic response |
| GET    | `/users`      | Get all users         |
| GET    | `/users/{id}` | Get a user by ID      |
| POST   | `/users`      | Create a new user     |
| PUT    | `/users/{id}` | Update a user         |
| DELETE | `/users/{id}` | Delete a user         |

## Getting Started

### Prerequisites

- .NET 10 SDK
- PostgreSQL

### Clone the repository

```bash
git clone <repository-url>
cd MyWebApp
```

### Configure the database

The database connection string is stored using .NET User Secrets and is not committed to the repository.

Initialize User Secrets if needed:

```bash
dotnet user-secrets init
```

Then configure the connection string:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=mywebapp;Username=postgres;Password=your-password"
```

### Apply migrations

```bash
dotnet ef database update
```

### Run the application

```bash
dotnet run
```

## Development

This project is currently being developed as a learning project to explore C#, .NET, and ASP.NET Core concepts through practical implementation.

The project will gradually evolve as new concepts and features are introduced.
