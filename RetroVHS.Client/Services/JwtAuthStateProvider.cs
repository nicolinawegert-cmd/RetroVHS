using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using RetroVHS.Shared.DTOs.Auth;

namespace RetroVHS.Client.Services;

/// <summary>
/// Läser claims direkt från JWT-tokenen som API:t returnerar
/// och integrerar med Blazors autentiseringssystem.
/// Stödjer AuthorizeView, [Authorize] och CascadingAuthenticationState.
/// </summary>
public class JwtAuthStateProvider : AuthenticationStateProvider, IAppAuthStateProvider
{
    private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
        => Task.FromResult(new AuthenticationState(_currentUser));

    /// <summary>
    /// Anropas efter lyckad inloggning. Parsar JWT-claims och lägger till
    /// användardata (t.ex. förnamn) som extra claims.
    /// Om tokenen inte kan parsas loggas användaren ut i stället för att
    /// ett undantag kastas.
    /// </summary>
    public void MarkUserAsAuthenticated(string jwtToken, UserDto user)
    {
        var claims = ParseClaimsFromJwt(jwtToken).ToList();

        // Tom claims-lista betyder malformad token — behandla som utloggning
        if (claims.Count == 0)
        {
            MarkUserAsLoggedOut();
            return;
        }

        // Lägg till användardata som inte finns i JWT:n
        claims.Add(new Claim("firstName", user.FirstName));
        claims.Add(new Claim("lastName", user.LastName));
        if (user.Nickname is not null)
            claims.Add(new Claim("nickname", user.Nickname));

        _currentUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    /// <summary>
    /// Anropas vid utloggning. Rensar auth-state.
    /// </summary>
    public void MarkUserAsLoggedOut()
    {
        _currentUser = new(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    /// <summary>
    /// Uppdaterar visningsnamn-claims i minnet efter profiluppdatering.
    /// Rör inte JWT-tokenen — enbart in-memory state.
    /// </summary>
    public void UpdateDisplayClaims(RetroVHS.Shared.DTOs.Auth.UserDto user)
    {
        var identity = _currentUser.Identity as ClaimsIdentity;
        if (identity == null) return;

        foreach (var claim in identity.Claims.Where(c => c.Type is "firstName" or "lastName" or "nickname").ToList())
            identity.RemoveClaim(claim);

        identity.AddClaim(new Claim("firstName", user.FirstName));
        identity.AddClaim(new Claim("lastName", user.LastName));
        if (user.Nickname is not null)
            identity.AddClaim(new Claim("nickname", user.Nickname));

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    /// <summary>
    /// Parsar payload-delen av en JWT-token och returnerar claims.
    /// Returnerar en tom samling om tokenen är malformad eller inte kan avkodas,
    /// så att anroparen kan hantera ett ogiltigt tillstånd utan undantag.
    /// </summary>
    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        try
        {
            var parts = jwt.Split('.');
            if (parts.Length != 3)
                return [];

            var jsonBytes = ParseBase64Url(parts[1]);
            var pairs = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonBytes);
            if (pairs is null)
                return [];

            var claims = new List<Claim>();
            foreach (var (key, value) in pairs)
            {
                if (value.ValueKind == JsonValueKind.Array)
                {
                    foreach (var element in value.EnumerateArray())
                    {
                        var str = element.GetString();
                        if (str is not null)
                            claims.Add(new Claim(key, str));
                    }
                }
                else
                {
                    claims.Add(new Claim(key, value.ToString()));
                }
            }
            return claims;
        }
        catch
        {
            return [];
        }
    }

    /// <summary>
    /// Avkodar base64url (JWT-format) till bytes.
    /// </summary>
    private static byte[] ParseBase64Url(string base64Url)
    {
        var base64 = base64Url.Replace('-', '+').Replace('_', '/');
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}
