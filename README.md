# Freelancer Portfolio API

A production-ready ASP.NET Core 8.0 Web API for managing a freelancer's portfolio. Deployed on **Render** with **Neon PostgreSQL** as the database provider.

## Architecture Overview

This project follows a **Layered Architecture** pattern with clear separation of concerns:

```
FreelancerPortfolio/
├── Entities/              # Domain models
├── Data/                  # EF Core DbContext + Migrations
├── Repositories/          # Data access layer
│   ├── Interfaces/
│   └── Implementations/
├── Services/              # Business logic layer
│   ├── Interfaces/
│   └── Implementations/
├── DTOs/                  # Data Transfer Objects
├── Controllers/           # API endpoints
├── Validators/            # FluentValidation rules
├── Utilities/             # Helper classes (Slug, Password)
├── Constants/             # Application constants
├── Middleware/            # Global exception handling
├── Mappings/              # AutoMapper profiles
└── Program.cs             # Application startup
```

## Technology Stack

| Component         | Technology                           |
|-------------------|--------------------------------------|
| Framework         | ASP.NET Core 8.0                     |
| ORM               | Entity Framework Core 8.0            |
| Database          | Neon PostgreSQL (via Npgsql)         |
| Authentication    | JWT (JSON Web Tokens + Refresh)      |
| Validation        | FluentValidation 11.x                |
| Mapping           | AutoMapper 16.x                      |
| Password Hashing  | BCrypt.Net-Next (work factor 12)     |
| Rate Limiting     | Built-in .NET 7+ Rate Limiter        |
| Logging           | Serilog (Console sink)               |
| API Docs          | Swagger/OpenAPI (development only)   |
| Health Checks     | AspNetCore.HealthChecks.NpgSql       |

## API Endpoints

### Authentication
| Method | Endpoint              | Auth Required | Description        |
|--------|----------------------|---------------|--------------------|
| POST   | `/api/auth/login`    | No            | Login admin user   |
| POST   | `/api/auth/refresh`  | No            | Refresh JWT token  |
| POST   | `/api/auth/logout`   | Yes           | Logout + revoke    |

### Projects
| Method | Endpoint                              | Auth Required | Description              |
|--------|--------------------------------------|---------------|--------------------------|
| GET    | `/api/project`                        | No            | Get all projects         |
| GET    | `/api/project/published`              | No            | Get published projects   |
| GET    | `/api/project/{id}`                   | No            | Get project by ID        |
| GET    | `/api/project/slug/{slug}`            | No            | Get project by slug      |
| GET    | `/api/project/category/{categoryId}`  | No            | Filter by category       |
| GET    | `/api/project/technology/{techId}`    | No            | Filter by technology     |
| POST   | `/api/project`                        | Yes           | Create project (Admin)   |
| PUT    | `/api/project/{id}`                   | Yes           | Update project (Admin)   |
| DELETE | `/api/project/{id}`                   | Yes           | Delete project (Admin)   |

### Categories
| Method | Endpoint               | Auth Required | Description        |
|--------|------------------------|---------------|--------------------|
| GET    | `/api/category`         | No            | Get all categories |
| GET    | `/api/category/{id}`    | No            | Get by ID          |
| GET    | `/api/category/slug/{s}`| No            | Get by slug        |
| POST   | `/api/category`         | Yes           | Create (Admin)     |
| PUT    | `/api/category/{id}`    | Yes           | Update (Admin)     |
| DELETE | `/api/category/{id}`    | Yes           | Delete (Admin)     |

### Technologies
| Method | Endpoint                | Auth Required | Description        |
|--------|-------------------------|---------------|--------------------|
| GET    | `/api/technology`        | No            | Get all            |
| GET    | `/api/technology/{id}`   | No            | Get by ID          |
| GET    | `/api/technology/slug/{s}` | No          | Get by slug        |
| POST   | `/api/technology`        | Yes           | Create (Admin)     |
| PUT    | `/api/technology/{id}`   | Yes           | Update (Admin)     |
| DELETE | `/api/technology/{id}`   | Yes           | Delete (Admin)     |

### Health
| Method | Endpoint  | Description                      |
|--------|-----------|----------------------------------|
| GET    | `/health` | Health check (Render uses this)  |

---

## Local Development Setup

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- PostgreSQL (local or Docker) **or** a free [Neon](https://neon.tech) database
- `dotnet-ef` CLI tool

### 1. Install EF Core CLI
```bash
dotnet tool install --global dotnet-ef
```

### 2. Clone and configure
```bash
git clone <repository-url>
cd FreelancerPortfolio_SimpleLayeredArchitecture
```

Edit `FreelancerPortfolio/appsettings.Development.json` and set your local PostgreSQL connection string:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=FreelancerPortfolioDb;Username=postgres;Password=your_password"
  }
}
```

### 3. Apply migrations
```bash
cd FreelancerPortfolio
dotnet ef database update
```

### 4. Run
```bash
dotnet run
```

The API starts at `https://localhost:7001` (or `http://localhost:5000`).  
Swagger UI is available at: `http://localhost:5000/swagger`

**Default dev admin credentials** (auto-seeded in Development):
- Username: `admin`
- Password: `Admin@12345`

---

## Database Migration Commands

```bash
# Create a new migration
dotnet ef migrations add MigrationName

# Apply pending migrations
dotnet ef database update

# Remove last migration (if not applied)
dotnet ef migrations remove

# Generate SQL script (for review)
dotnet ef migrations script

# Apply to a specific connection string
dotnet ef database update --connection "Host=...;Database=...;"
```

---

## Neon PostgreSQL Setup

1. Go to [neon.tech](https://neon.tech) and create a free account.
2. Create a new project → note down the **connection string**.
3. The connection string format is:
   ```
   Host=ep-xxx.us-east-2.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=your_password;SSL Mode=Require
   ```
4. Set this as `ConnectionStrings__DefaultConnection` in Render environment variables.
5. Migrations run automatically on startup.

---

## Render Deployment

### Using render.yaml (recommended)
The `render.yaml` at the repo root defines the web service configuration.

1. Push this repo to GitHub.
2. In [Render Dashboard](https://render.com), click **New → Blueprint** and connect your GitHub repo.
3. Render will detect `render.yaml` and create the service.
4. Set the secret environment variables in the Render dashboard:
   - `ConnectionStrings__DefaultConnection` — your Neon connection string
   - `JWT__SecretKey` — at least 32 random characters
   - `Cors__AllowedOrigins` — comma-separated list of your Vercel domains

### Manual Setup
1. Create a **Web Service** in Render.
2. Connect your GitHub repo.
3. Configure:
   - **Build Command:** `dotnet publish FreelancerPortfolio/FreelancerPortfolio.csproj -c Release -o out`
   - **Start Command:** `dotnet out/FreelancerPortfolio.dll`
   - **Health Check Path:** `/health`
4. Add the environment variables listed below.

---

## Vercel Frontend Setup

Configure these in your Vercel project → Settings → Environment Variables:
- `NEXT_PUBLIC_API_BASE_URL` = `https://your-api.onrender.com`

See the frontend repo for detailed instructions.

---

## Environment Variables

| Variable | Required | Example | Purpose |
|---|---|---|---|
| `ConnectionStrings__DefaultConnection` | ✅ | `Host=...;Database=...;Username=...;Password=...;SSL Mode=Require` | Neon PostgreSQL connection |
| `JWT__SecretKey` | ✅ | `a-random-32-plus-character-string` | JWT signing key (≥32 chars) |
| `JWT__Issuer` | ✅ | `https://your-api.onrender.com` | JWT issuer claim |
| `JWT__Audience` | ✅ | `FreelancerPortfolioAPI` | JWT audience claim |
| `JWT__AccessTokenExpireMinutes` | ❌ | `15` | Access token lifetime (default: 15) |
| `JWT__RefreshTokenExpireDays` | ❌ | `7` | Refresh token lifetime (default: 7) |
| `Cors__AllowedOrigins` | ✅ | `https://app.vercel.app,https://custom.com` | Allowed CORS origins (comma-separated) |
| `ASPNETCORE_ENVIRONMENT` | ✅ | `Production` | Runtime environment |
| `PORT` | ❌ | `10000` | HTTP port (Render injects this automatically) |
| `DevSeed__AdminPassword` | ❌ | `Admin@12345` | Dev-only admin seed password |

> **Note:** In ASP.NET Core, double-underscore `__` is used as the section separator when setting config via environment variables (equivalent to `:` in JSON).

---

## Post-Deploy Verification Checklist

- [ ] `GET /health` returns `200 Healthy`
- [ ] `POST /api/auth/login` with admin credentials returns JWT tokens
- [ ] `GET /api/project/published` returns `200` (may be empty array initially)
- [ ] `GET /api/category` returns `200`
- [ ] `GET /api/technology` returns `200`
- [ ] Swagger UI is **not** accessible at `/swagger` in production
- [ ] CORS: frontend domain can call the API without CORS errors
- [ ] Create a project with the admin token to verify write operations work
- [ ] Logs visible in Render dashboard

---

## Authentication Flow

1. `POST /api/auth/login` → returns `accessToken` + `refreshToken`
2. Include `Authorization: Bearer {accessToken}` header for protected endpoints
3. When access token expires (15 min by default), call `POST /api/auth/refresh` with `refreshToken`
4. To logout: `POST /api/auth/logout` with `refreshToken`

---

## Error Response Format

All errors follow a consistent JSON contract:
```json
{
  "success": false,
  "message": "Validation failed.",
  "errors": ["Field X is required.", "Field Y must be less than 100 characters."],
  "statusCode": 400
}
```

---

## Security Features

- **BCrypt** password hashing with work factor 12
- **JWT** with configurable expiry and zero clock skew
- **Refresh token** rotation (single-use, revocable)
- **Rate limiting** on auth endpoints (10 requests/minute on login/refresh)
- **Security headers**: X-Content-Type-Options, X-Frame-Options, X-XSS-Protection
- **HTTPS forwarded headers** support for Render reverse proxy
- **FluentValidation** on all input DTOs
- **Unique slug constraints** in database

---

## License

MIT
