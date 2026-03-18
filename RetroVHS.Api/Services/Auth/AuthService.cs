using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RetroVHS.Api.Data;
using RetroVHS.Api.Models;
using RetroVHS.Shared.DTOs.Auth;

namespace RetroVHS.Api.Services.Auth;

/// <summary>
/// Innehåller all affärslogik för autentisering och token-hantering.
/// Vi håller controllers tunna och lägger validering och logik här.
/// </summary>
public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ApplicationDbContext _context;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Konstruktor där vi injicerar de beroenden som behövs för auth-flödet.
    /// </summary>
    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ApplicationDbContext context,
        IJwtTokenService jwtTokenService,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
        _jwtTokenService = jwtTokenService;
        _configuration = configuration;
    }

    public async Task<AuthResultDto> RegisterAsync(RegisterRequestDto request)
    {
        // Kontrollera om email redan finns
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return new AuthResultDto
            {
                Succeeded = false,
                Message = "En användare med denna e-post finns redan.",
                Errors = new List<string> { "Email already exists" }
            };
        }

        // Kontrollera om username redan finns (om ni använder det)
        var existingUsername = await _userManager.FindByNameAsync(request.Email);
        if (existingUsername != null)
        {
            return new AuthResultDto
            {
                Succeeded = false,
                Message = "Användarnamnet är redan taget.",
                Errors = new List<string> { "Username already exists" }
            };
        }

        // Skapa ny användare
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Nickname = request.Nickname,
            CreatedAt = DateTime.UtcNow,
            IsBlocked = false
        };

        //  Skapa användare med lösenord
        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return new AuthResultDto
            {
                Succeeded = false,
                Message = "Registrering misslyckades.",
                Errors = result.Errors.Select(e => e.Description).ToList()
            };
        }

        // Lägg till standardroll (Member)
        await _userManager.AddToRoleAsync(user, "Member");

        // Bygg auth svar (token + refresh token)
        var authResponse = await BuildAuthResponseAsync(user);

        return new AuthResultDto
        {
            Succeeded = true,
            Message = "Registrering lyckades.",
            Data = authResponse
        };
    }



    public async Task<AuthResultDto> LoginAsync(LoginRequestDto request)
    {
        // Försök hitta användaren via e-post
        var user = await _userManager.FindByEmailAsync(request.Email);

        // Av säkerhetsskäl returnerar vi samma felmeddelande
        // oavsett om e-post eller lösenord är fel
        if (user == null)
        {
            return new AuthResultDto
            {
                Succeeded = false,
                Message = "Fel e-post eller lösenord.",
                Errors = new List<string> { "Invalid email or password" }
            };
        }

        // Kontrollera om användaren är blockerad
        if (user.IsBlocked)
        {
            return new AuthResultDto
            {
                Succeeded = false,
                Message = "Kontot är blockerat. Kontakta administratör.",
                Errors = new List<string> { "User is blocked" }
            };
        }

        // Kontrollera lösenordet via Identity
        var passwordResult = await _signInManager.CheckPasswordSignInAsync(
            user,
            request.Password,
            lockoutOnFailure: false);

        if (!passwordResult.Succeeded)
        {
            return new AuthResultDto
            {
                Succeeded = false,
                Message = "Fel e-post eller lösenord.",
                Errors = new List<string> { "Invalid email or password" }
            };
        }

        // Bygg auth-svar med access token + refresh token
        var authResponse = await BuildAuthResponseAsync(user);

        return new AuthResultDto
        {
            Succeeded = true,
            Message = "Inloggning lyckades.",
            Data = authResponse
        };
    }



    public async Task<AuthResultDto> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
        // Hitta refresh token i databasen
        var existingToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);

        if (existingToken == null)
        {
            return new AuthResultDto
            {
                Succeeded = false,
                Message = "Ogiltig refresh token.",
                Errors = new List<string> { "Invalid refresh token" }
            };
        }

        //  Kontrollera om token är expired
        if (existingToken.ExpiresAt < DateTime.UtcNow)
        {
            return new AuthResultDto
            {
                Succeeded = false,
                Message = "Refresh token har gått ut.",
                Errors = new List<string> { "Token expired" }
            };
        }

        //  Kontrollera om den redan är använd
        if (existingToken.UsedAt != null)
        {
            return new AuthResultDto
            {
                Succeeded = false,
                Message = "Refresh token har redan använts.",
                Errors = new List<string> { "Token already used" }
            };
        }

        // Kontrollera om den är revoked
        if (existingToken.RevokedAt != null)
        {
            return new AuthResultDto
            {
                Succeeded = false,
                Message = "Refresh token är återkallad.",
                Errors = new List<string> { "Token revoked" }
            };
        }

        //  Hämta användaren
        var user = existingToken.User;

        if (user == null || user.IsBlocked)
        {
            return new AuthResultDto
            {
                Succeeded = false,
                Message = "Ogiltig användare.",
                Errors = new List<string> { "Invalid user" }
            };
        }

        //  Markera token som använd (token rotation)
        existingToken.UsedAt = DateTime.UtcNow;

        // Spara ändringen
        await _context.SaveChangesAsync();

        // Skapa ny access token + refresh token
        var authResponse = await BuildAuthResponseAsync(user);

        return new AuthResultDto
        {
            Succeeded = true,
            Message = "Token uppdaterad.",
            Data = authResponse
        };
    }





    public async Task<AuthResultDto> LogoutAsync(string refreshToken)
    {
        //  Hitta refresh token
        var existingToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (existingToken == null)
        {
            return new AuthResultDto
            {
                Succeeded = false,
                Message = "Ogiltig refresh token.",
                Errors = new List<string> { "Invalid token" }
            };
        }

        // Om redan revoked, gör inget
        if (existingToken.RevokedAt != null)
        {
            return new AuthResultDto
            {
                Succeeded = false,
                Message = "Token är redan utloggad.",
                Errors = new List<string> { "Already revoked" }
            };
        }

        // Markera som revoked
        existingToken.RevokedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new AuthResultDto
        {
            Succeeded = true,
            Message = "Utloggning lyckades."
        };
    }



    /// <summary>
    /// Skapar en kryptografiskt säker refresh token.
    /// </summary>
    private static string GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(randomBytes);
    }

    /// <summary>
    /// Skapar ett komplett auth-svar med access token, refresh token,
    /// användardata och roller.
    /// </summary>
    private async Task<AuthResponseDto> BuildAuthResponseAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtTokenService.GenerateAccessToken(user, roles);
        var expiresInMinutes = int.Parse(_configuration["Jwt:ExpiresInMinutes"]!);

        var refreshToken = new RefreshToken
        {
            Token = GenerateRefreshToken(),
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            Token = accessToken,
            RefreshToken = refreshToken.Token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expiresInMinutes),
            Roles = roles.ToList(),
            User = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Nickname = user.Nickname,
                Email = user.Email ?? string.Empty,
                IsBlocked = user.IsBlocked
            }
        };
    }
}