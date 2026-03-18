using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using RetroVHS.Api.Models;

namespace RetroVHS.Api.Services.Auth;

/// <summary>
/// Ansvarar för att skapa JWT-tokens baserat på användare och roller.
/// </summary>
public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Konstruktor där vi hämtar konfiguration från appsettings.json
    /// </summary>
    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Skapar en JWT-token för en användare
    /// </summary>
    public string GenerateAccessToken(ApplicationUser user, IList<string> roles)
    {
        //  Hämta hemlig nyckel från appsettings
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
        );

        //  Signeringsalgoritm
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //  Claims - information som lagras i token
        var claims = new List<Claim>
        {
            // Unik identifierare för användaren
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),

            // Email
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),

            // Username (valfri men bra att ha)
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty)
        };

        //  Lägg till roller som claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        //  Hämta expiry från appsettings
        var expiresInMinutes = int.Parse(_configuration["Jwt:ExpiresInMinutes"]!);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
            signingCredentials: credentials
        );

        // Konvertera token till string
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}