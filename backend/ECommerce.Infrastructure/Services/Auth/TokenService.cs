using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ECommerce.Infrastructure.Services.Auth;

/// <summary>
/// Implements JWT token generation.
/// Reads configuration from appsettings (Issuer, Audience, SecretKey, ExpiryInMinutes)
/// and builds a signed JWT containing the user's identity claims.
/// </summary>
public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public (string token, DateTime expiresAt) GenerateToken(User user)
    {
        // 1. Read JWT settings from appsettings.json / appsettings.Development.json
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey   = jwtSettings["SecretKey"]!;
        var issuer      = jwtSettings["Issuer"]!;
        var audience    = jwtSettings["Audience"]!;
        var expiryMins  = int.Parse(jwtSettings["ExpiryMinutes"]!);

        // 2. Create the signing key from our secret
        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // 3. Define the claims embedded inside the token.
        //    Using short claim keys (nameid, email, role) for clean JSON payload and small token footprint.
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email,  user.Email),
            new Claim("role",                        user.Role)
        };

        var expiresAt = DateTime.UtcNow.AddMinutes(expiryMins);

        // 4. Build the token descriptor
        var token = new JwtSecurityToken(
            issuer:             issuer,
            audience:           audience,
            claims:             claims,
            expires:            expiresAt,
            signingCredentials: creds
        );

        // 5. Serialize to the compact JWT string (xxxxx.yyyyy.zzzzz)
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return (tokenString, expiresAt);
    }
}
