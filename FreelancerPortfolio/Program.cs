using System.Text;
using System.Threading.RateLimiting;
using FluentValidation;
using FreelancerPortfolio.Constants;
using FreelancerPortfolio.Data;
using FreelancerPortfolio.Mappings;
using FreelancerPortfolio.Middleware;
using FreelancerPortfolio.Repositories.Implementations;
using FreelancerPortfolio.Repositories.Interfaces;
using FreelancerPortfolio.Services.Implementations;
using FreelancerPortfolio.Services.Interfaces;
using FreelancerPortfolio.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

// ─── Bootstrap Serilog early ──────────────────────────────────────────────────
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // ─── Serilog ──────────────────────────────────────────────────────────────
    builder.Host.UseSerilog((ctx, services, config) =>
        config.ReadFrom.Configuration(ctx.Configuration)
              .WriteTo.Console());

    // ─── Bind PORT for Render ─────────────────────────────────────────────────
    var port = Environment.GetEnvironmentVariable("PORT");
    if (!string.IsNullOrEmpty(port))
    {
        builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
    }

    // ─── Database ─────────────────────────────────────────────────────────────
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not set.");

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(connectionString));

    // ─── JWT ──────────────────────────────────────────────────────────────────
    var jwtSection = builder.Configuration.GetSection("JWT");
    var jwtSecret = jwtSection["SecretKey"]
        ?? throw new InvalidOperationException("JWT:SecretKey is not configured.");

    if (jwtSecret.Length < AppConstants.MinJwtSecretLength)
        throw new InvalidOperationException(
            $"JWT:SecretKey must be at least {AppConstants.MinJwtSecretLength} characters for security.");

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSection["Issuer"],
                ValidAudience = jwtSection["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                ClockSkew = TimeSpan.Zero,
            };
        });
    builder.Services.AddAuthorization();

    // ─── CORS ─────────────────────────────────────────────────────────────────
    var allowedOrigins = builder.Configuration["Cors:AllowedOrigins"]
        ?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        ?? Array.Empty<string>();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy(AppConstants.CorsPolicy, policy =>
        {
            if (allowedOrigins.Length > 0)
                policy.WithOrigins(allowedOrigins);
            else
                policy.AllowAnyOrigin();
            policy.AllowAnyHeader().AllowAnyMethod();
        });
    });

    // ─── Rate Limiting (built-in .NET 7+) ────────────────────────────────────
    builder.Services.AddRateLimiter(options =>
    {
        options.AddFixedWindowLimiter("auth", limiter =>
        {
            limiter.PermitLimit = 10;
            limiter.Window = TimeSpan.FromMinutes(1);
            limiter.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            limiter.QueueLimit = 0;
        });
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    });

    // ─── Controllers + Validation ─────────────────────────────────────────────
    builder.Services.AddControllers()
        .ConfigureApiBehaviorOptions(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

    builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

    // ─── AutoMapper ───────────────────────────────────────────────────────────
    builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

    // ─── Repositories ─────────────────────────────────────────────────────────
    builder.Services.AddScoped<IAdminUserRepository, AdminUserRepository>();
    builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
    builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
    builder.Services.AddScoped<ITechnologyRepository, TechnologyRepository>();
    builder.Services.AddScoped<IProjectRepository, ProjectRepository>();

    // ─── Services ─────────────────────────────────────────────────────────────
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<ICategoryService, CategoryService>();
    builder.Services.AddScoped<ITechnologyService, TechnologyService>();
    builder.Services.AddScoped<IProjectService, ProjectService>();

    // ─── Swagger ──────────────────────────────────────────────────────────────
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Freelancer Portfolio API",
            Version = "v1",
            Description = "API for managing a freelancer's portfolio projects, categories, and technologies.",
        });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Enter your JWT token",
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                },
                Array.Empty<string>()
            }
        });
    });

    // ─── Health Checks ────────────────────────────────────────────────────────
    builder.Services.AddHealthChecks()
        .AddNpgSql(connectionString);

    // ─── Build ────────────────────────────────────────────────────────────────
    var app = builder.Build();

    // ─── Forwarded Headers (required for Render proxy) ────────────────────────
    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    });

    // ─── Global Exception Handling ────────────────────────────────────────────
    app.UseMiddleware<GlobalExceptionMiddleware>();

    // ─── Swagger (Development only) ───────────────────────────────────────────
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Freelancer Portfolio API v1");
            c.RoutePrefix = "swagger";
        });
    }

    // ─── Security Headers ─────────────────────────────────────────────────────
    app.Use(async (context, next) =>
    {
        context.Response.Headers["X-Content-Type-Options"] = "nosniff";
        context.Response.Headers["X-Frame-Options"] = "DENY";
        context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
        context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
        await next();
    });

    app.UseSerilogRequestLogging();
    app.UseCors(AppConstants.CorsPolicy);
    app.UseRateLimiter();
    app.UseAuthentication();
    app.UseAuthorization();

    // ─── Health Check ─────────────────────────────────────────────────────────
    app.MapHealthChecks(AppConstants.HealthCheckPath);

    // ─── Auth rate limiting ───────────────────────────────────────────────────
    app.MapControllers();

    // ─── Apply migrations on startup (production-safe) ────────────────────────
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<AppDbContext>>();
        try
        {
            logger.LogInformation("Applying pending database migrations...");
            await db.Database.MigrateAsync();
            logger.LogInformation("Database migrations applied successfully.");

            // Seed admin user in development
            if (app.Environment.IsDevelopment())
            {
                await SeedDevelopmentDataAsync(db, logger, builder.Configuration);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while applying migrations.");
            throw;
        }
    }

    Log.Information("Starting Freelancer Portfolio API...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application startup failed.");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

// ─── Dev seed helper ──────────────────────────────────────────────────────────
static async Task SeedDevelopmentDataAsync(
    AppDbContext db,
    Microsoft.Extensions.Logging.ILogger<AppDbContext> logger,
    IConfiguration config)
{
    if (!db.AdminUsers.Any())
    {
        var seedPassword = config["DevSeed:AdminPassword"] ?? "Admin@12345";
        var adminUser = new FreelancerPortfolio.Entities.AdminUser
        {
            Username = AppConstants.DefaultAdminUsername,
            Email = "admin@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(seedPassword, workFactor: 12),
        };
        db.AdminUsers.Add(adminUser);
        await db.SaveChangesAsync();
        logger.LogInformation("Development seed: admin user created (username: {Username})", adminUser.Username);
    }
}
