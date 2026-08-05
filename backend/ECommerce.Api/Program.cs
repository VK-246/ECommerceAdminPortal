using System.Text;
using ECommerce.Api.Middleware;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using ECommerce.Infrastructure.Data;
using ECommerce.Infrastructure.Repositories;
using ECommerce.Infrastructure.Services.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

// -------------------------------------------------------
// Program.cs — API Entry Point
// -------------------------------------------------------
// Minimal hosting setup for .NET 10. Registers services
// and configures the HTTP pipeline.
// -------------------------------------------------------

var builder = WebApplication.CreateBuilder(args);

// =======================================================
// SECTION 1: Service Registration (Dependency Injection)
// =======================================================

// --- Database ---
// Register EF Core DbContext with PostgreSQL (Npgsql) provider.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register the IAppDbContext interface → AppDbContext implementation.
// This lets the Application layer resolve IAppDbContext without referencing Infrastructure.
builder.Services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

// Repositories
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// --- Application Services ---
// Register the Auth service. Controllers ask for IAuthService; DI injects AuthService.
builder.Services.AddScoped<IAuthService, AuthService>();

// Core Domain Services
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();

// --- Infrastructure Services ---
// Register token generation. AuthService asks for ITokenService; DI injects TokenService.
builder.Services.AddScoped<ITokenService, TokenService>();

// Register password hashing. BCrypt stays in Infrastructure — Application only sees the interface.
builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();

// --- Controllers ---
builder.Services.AddControllers();

// --- JWT Bearer Authentication ---
// Tell ASP.NET Core how to validate incoming JWT tokens on every protected request.
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"]!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,           // Reject expired tokens
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        NameClaimType = "nameid",
        RoleClaimType = "role"
    };
});

builder.Services.AddAuthorization();

// --- Swagger / OpenAPI (with JWT support) ---
// Adds a padlock icon in Swagger UI so we can test protected endpoints
// by pasting our JWT token in one place.
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ECommerce Admin API", Version = "v1" });

    // Adds a Bearer token input box in Swagger UI.
    // Enter your JWT token here to authenticate all requests in Swagger.
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste your JWT token here. Swagger will automatically prepend 'Bearer '."
    });
});


// =======================================================
// SECTION 2: Build the Application
// =======================================================

var app = builder.Build();

// =======================================================
// SECTION 3: HTTP Pipeline Configuration (Middleware Order)
// =======================================================
// ORDER MATTERS: Each request passes through these in sequence.
// GlobalExceptionMiddleware must be FIRST so it wraps everything below.

// 1. Global exception handler — catches all unhandled exceptions in the pipeline
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    // Swagger UI available at /swagger in development
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ECommerce Admin API v1"));
    app.MapOpenApi();
}

// 2. Redirect HTTP → HTTPS
app.UseHttpsRedirection();

// 3. Authentication — validates the JWT token on each request
app.UseAuthentication();

// 4. Authorization — checks [Authorize] attributes after auth is confirmed
app.UseAuthorization();

// 5. Map controller routes (e.g., /api/auth/login, /api/categories)
app.MapControllers();

// Health check endpoint — confirms the API is running
app.MapGet("/api/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
    .WithName("HealthCheck")
    .WithTags("Health");

app.Run();
