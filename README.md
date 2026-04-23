# Freelancer Portfolio API

A comprehensive ASP.NET Core 8.0 Web API for managing a freelancer's portfolio. This API provides endpoints for managing projects, categories, technologies, and admin authentication.

## Architecture Overview

This project follows a **Layered Architecture** pattern with clear separation of concerns:

```
FreelancerPortfolio/
??? Entities/              # Domain models
??? Data/                  # EF Core DbContext
??? Repositories/          # Data access layer
?   ??? Interfaces/
?   ??? Implementations/
??? Services/              # Business logic layer
?   ??? Interfaces/
?   ??? Implementations/
??? DTOs/                  # Data Transfer Objects
??? Controllers/           # API endpoints
??? Validators/            # FluentValidation rules
??? Utilities/             # Helper classes
??? Constants/             # Constants
??? Middleware/            # Custom middleware
??? Mappings/              # AutoMapper profiles
??? Program.cs             # Application startup
```

## Key Features

- **RESTful API** with clean endpoints
- **JWT Authentication** for admin users
- **Entity Framework Core** with SQL Server
- **FluentValidation** for input validation
- **AutoMapper** for object mapping
- **Global Exception Handling** middleware
- **CORS** support for cross-origin requests

## Technology Stack

- **Framework**: ASP.NET Core 8.0
- **Database**: SQL Server (via Entity Framework Core)
- **Authentication**: JWT (JSON Web Tokens)
- **Validation**: FluentValidation
- **Mapping**: AutoMapper
- **ORM**: Entity Framework Core 8.0

## NuGet Packages

- Microsoft.EntityFrameworkCore.SqlServer (8.0.0)
- Microsoft.EntityFrameworkCore.Tools (8.0.0)
- FluentValidation (11.9.0)
- FluentValidation.DependencyInjectionExtensions (11.9.0)
- AutoMapper (13.0.1)
- AutoMapper.Extensions.Microsoft.DependencyInjection (12.0.1)
- System.IdentityModel.Tokens.Jwt (7.1.0)
- Microsoft.IdentityModel.Tokens (7.1.0)
- Microsoft.AspNetCore.Authentication.JwtBearer (8.0.0)
- Swashbuckle.AspNetCore (6.6.2)

## Setup Instructions

### Prerequisites
- .NET 8.0 SDK
- SQL Server (LocalDB or full version)
- Visual Studio 2022 or VS Code

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd FreelancerPortfolio
   ```

2. **Update appsettings**
   - Modify `appsettings.json` with your database connection string
   - Update JWT secret key (must be at least 32 characters)
   - Set JWT issuer and audience URLs

3. **Create and Apply Migrations**
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

4. **Run the Application**
   ```bash
   dotnet run
   ```

The API will be available at `https://localhost:7001` (or `http://localhost:5000`)

## API Endpoints

### Projects
- `GET /api/project` - Get all projects
- `GET /api/project/{id}` - Get project by ID
- `GET /api/project/slug/{slug}` - Get project by slug
- `GET /api/project/published` - Get published projects
- `GET /api/project/category/{categoryId}` - Get projects by category
- `GET /api/project/technology/{technologyId}` - Get projects by technology
- `POST /api/project` - Create project (Admin)
- `PUT /api/project/{id}` - Update project (Admin)
- `DELETE /api/project/{id}` - Delete project (Admin)

### Categories
- `GET /api/category` - Get all categories
- `GET /api/category/{id}` - Get category by ID
- `GET /api/category/slug/{slug}` - Get category by slug
- `POST /api/category` - Create category (Admin)
- `PUT /api/category/{id}` - Update category (Admin)
- `DELETE /api/category/{id}` - Delete category (Admin)

### Technologies
- `GET /api/technology` - Get all technologies
- `GET /api/technology/{id}` - Get technology by ID
- `GET /api/technology/slug/{slug}` - Get technology by slug
- `POST /api/technology` - Create technology (Admin)
- `PUT /api/technology/{id}` - Update technology (Admin)
- `DELETE /api/technology/{id}` - Delete technology (Admin)

### Authentication
- `POST /api/auth/login` - User login
- `POST /api/auth/refresh` - Refresh access token
- `POST /api/auth/logout` - Logout (revoke token)

## Configuration

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=FreelancerPortfolioDb;..."
  },
  "JWT": {
    "SecretKey": "your-secret-key-minimum-32-characters",
    "Issuer": "https://freelancerportfolio.com",
    "Audience": "FreelancerPortfolioAPI",
    "AccessTokenExpireMinutes": 15,
    "RefreshTokenExpireDays": 7
  }
}
```

## Database Schema

### Tables
- **AdminUsers** - Admin user accounts
- **RefreshTokens** - JWT refresh tokens
- **Categories** - Project categories
- **Technologies** - Technology stack
- **Projects** - Portfolio projects
- **ProjectTechnologies** - Many-to-many relationship between projects and technologies

## Entity Relationships

```
AdminUser (1) ---> (Many) RefreshToken
Category (1) ---> (Many) Project
Project (1) ---> (Many) ProjectTechnology (Many) <--- (1) Technology
```

## Authentication Flow

1. User logs in with username/password ? `POST /api/auth/login`
2. API returns access token + refresh token
3. Include access token in Authorization header: `Bearer {accessToken}`
4. When access token expires, use refresh token ? `POST /api/auth/refresh`
5. Get new access token + new refresh token
6. Logout invalidates the JWT ID ? `POST /api/auth/logout`

## Validation

All DTOs are validated using FluentValidation:
- Project title/description length limits
- Required fields validation
- Category and technology uniqueness by slug
- JWT token validation

## Error Handling

The API uses global exception handling middleware that:
- Catches all unhandled exceptions
- Returns consistent error responses
- Logs errors for debugging
- Handles validation exceptions specially

## Development

### Database Migrations

Create a new migration:
```bash
dotnet ef migrations add MigrationName
```

Apply pending migrations:
```bash
dotnet ef database update
```

Remove last migration:
```bash
dotnet ef migrations remove
```

### Swagger/OpenAPI

API documentation is available at `/swagger` in development environment.

## Future Enhancements

- [ ] Role-based authorization (Admin roles)
- [ ] Image upload functionality
- [ ] Pagination for large result sets
- [ ] Advanced filtering and search
- [ ] API rate limiting
- [ ] Caching layer (Redis)
- [ ] File storage (Azure Blob Storage)
- [ ] Email notifications
- [ ] Social media integration

## License

This project is licensed under the MIT License.

## Support

For issues or questions, please contact the development team.
