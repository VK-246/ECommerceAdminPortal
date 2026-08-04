using ECommerce.Application.DTOs.Auth;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using ECommerce.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Services;

/// <summary>
/// Implements authentication business logic.
/// Handles user registration and login.
/// Delegates password hashing to IPasswordHasher and token generation to ITokenService
/// (Single Responsibility — each service does one thing).
/// </summary>
public class AuthService : IAuthService
{
    private readonly IAppDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(IAppDbContext context, ITokenService tokenService, IPasswordHasher passwordHasher)
    {
        _context        = context;
        _tokenService   = tokenService;
        _passwordHasher = passwordHasher;
    }

    // -------------------------------------------------------
    // Register
    // -------------------------------------------------------
    public async Task<UserDto> RegisterAsync(RegisterDto dto)
    {
        // 1. Validate that the role is a recognised value
        if (!Enum.TryParse<UserRole>(dto.Role, ignoreCase: true, out var parsedRole))
            throw new BadRequestException($"Invalid role '{dto.Role}'. Must be 'Admin' or 'Editor'.");

        // 2. Check for duplicate email (unique constraint guard)
        bool emailExists = await _context.Users
            .AnyAsync(u => u.Email == dto.Email.ToLower());

        if (emailExists)
            throw new BadRequestException("A user with this email already exists.");

        // 3. Hash the password — NEVER store plaintext
        string passwordHash = _passwordHasher.Hash(dto.Password);

        // 4. Build and persist the User entity
        var user = new User
        {
            Id           = Guid.NewGuid(),
            Email        = dto.Email.ToLower(),
            PasswordHash = passwordHash,
            Role         = parsedRole.ToString(),
            CreatedAt    = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // 5. Return a safe output DTO (no password hash exposed)
        return new UserDto
        {
            Id        = user.Id,
            Email     = user.Email,
            Role      = user.Role,
            CreatedAt = user.CreatedAt
        };
    }

    // -------------------------------------------------------
    // Login
    // -------------------------------------------------------
    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        // 1. Find the user by email
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email.ToLower());

        // 2. Verify user exists AND password matches the stored hash.
        //    Checking both in one condition prevents user enumeration:
        //    attacker cannot tell if the email doesn't exist vs the password is wrong.
        if (user is null || !_passwordHasher.Verify(dto.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid email or password.");

        // 3. Delegate token generation to ITokenService
        var (token, expiresAt) = _tokenService.GenerateToken(user);

        return new AuthResponseDto
        {
            Token     = token,
            Email     = user.Email,
            Role      = user.Role,
            ExpiresAt = expiresAt
        };
    }
}
