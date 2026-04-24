using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AssetManagement.Application.Common.Interfaces;
using AssetManagement.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AssetManagement.Infrastructure.Identity;

// Implementazione concreta di IJwtService
// Genera e valida token JWT usando la chiave segreta da appsettings.json
public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(User user)
    {
        // Legge la chiave segreta da appsettings.json → sezione Jwt → Key
        var key = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Chiave JWT non configurata.");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Claims — informazioni sull'utente incluse nel token
        // Il client può leggere questi dati dal token senza chiamare il server
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("fullName", user.FullName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Durata del token — legge da config, default 24 ore
        var expiryHours = int.Parse(_configuration["Jwt:ExpiryHours"] ?? "24");

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(expiryHours),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string? ValidateToken(string token)
    {
        var key = _configuration["Jwt:Key"];
        if (key == null) return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            ValidateIssuer = true,
            ValidIssuer = _configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = _configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero // nessuna tolleranza sulla scadenza
        };

        try
        {
            var principal = tokenHandler.ValidateToken(
                token, validationParameters, out _);

            // Restituisce l'email dell'utente estratta dal token validato
            return principal.FindFirstValue(JwtRegisteredClaimNames.Email);
        }
        catch
        {
            // Token non valido o scaduto
            return null;
        }
    }

    public int GetExpiryHours()
    {
        return int.Parse(_configuration["Jwt:ExpiryHours"] ?? "24");
    }
}