using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using TodoApp.Client.Services;

namespace TodoApp.Client.Auth;

public class JwtAuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorage _storage;

    public JwtAuthStateProvider(ILocalStorage storage) => _storage = storage;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _storage.Get("token");
        if (string.IsNullOrWhiteSpace(token))
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        var identity = ToClaimsIdentity(token);
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public async Task SetToken(string? token)
    {
        if (string.IsNullOrWhiteSpace(token)) await _storage.Remove("token");
        else await _storage.Set("token", token);

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private static ClaimsIdentity ToClaimsIdentity(string jwt)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);
        return new ClaimsIdentity(token.Claims, authenticationType: "jwt");
    }
}