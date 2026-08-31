using System.Text;
using ECommerce.Api.Middleware;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using ECommerce.Infrastructure.Data;
using ECommerce.Infrastructure.Repositories;
using ECommerce.Infrastructure.Services.Auth;
using ECommerce.Infrastructure.Services;
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
builder.Services.AddScoped<IDashboardService, DashboardService>();

// --- Infrastructure Services ---
// Register token generation. AuthService asks for ITokenService; DI injects TokenService.
builder.Services.AddScoped<ITokenService, TokenService>();

// Register password hashing. BCrypt stays in Infrastructure — Application only sees the interface.
builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();

// Register AI Service with HttpClient
builder.Services.AddHttpClient<IAiService, GeminiAiService>();

// --- CORS ---
// Reads allowed origins from the CORS_ALLOWED_ORIGINS environment variable.
// In local dev this falls back to localhost:4200.
// In production (Render), set: CORS_ALLOWED_ORIGINS=https://vinayak-portfolio.dev
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularApp", policy =>
    {
        var allowedOrigins = (builder.Configuration["CORS_ALLOWED_ORIGINS"]
            ?? "http://localhost:4200")
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Required for cookies to be sent cross-origin
    });
});

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

    // Tell the JWT middleware to look for the token in the cookie if it's not in the Auth header
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.ContainsKey("ecommerce_token"))
            {
                context.Token = context.Request.Cookies["ecommerce_token"];
            }
            return Task.CompletedTask;
        }
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

    // THIS IS THE MISSING PIECE:
    // Tells Swagger to actually ATTACH the Bearer token to every request
    // once authorized. Without this, the lock icon appears but the token
    // is never sent in the Authorization header.
    //
    // Note: OpenApiReference was REMOVED in Swashbuckle v10 / Microsoft.OpenApi v2.
    // The new API uses OpenApiSecuritySchemeReference with a document delegate.
    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
    });
});


// =======================================================
// SECTION 2: Build the Application
// =======================================================

var app = builder.Build();

// =======================================================
// AUTO MIGRATION — runs pending EF Core migrations on startup.
// This is required for Render (PaaS) deployments where we
// cannot manually run `dotnet ef database update`.
// Safe to run on every startup — EF Core skips already-applied migrations.
// =======================================================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

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

// Apply CORS policy before Auth
app.UseCors("AngularApp");

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
