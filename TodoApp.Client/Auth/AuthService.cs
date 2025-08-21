using System.Net.Http.Json;

namespace TodoApp.Client.Auth;

public class AuthService
{
    private readonly HttpClient _http;
    private readonly JwtAuthStateProvider _state;

    public AuthService(HttpClient http, JwtAuthStateProvider state)
    {
        _http = http; _state = state;
    }

    public async Task<bool> Login(string email, string password)
    {
        var resp = await _http.PostAsJsonAsync("api/auth/login", new { email, password });
        if (!resp.IsSuccessStatusCode) return false;
        var data = await resp.Content.ReadFromJsonAsync<TokenDto>();
        await _state.SetToken(data!.token);
        return true;
    }

    public async Task<bool> Register(string email, string password)
    {
        var resp = await _http.PostAsJsonAsync("api/auth/register", new { email, password });
        return resp.IsSuccessStatusCode;
    }

    public void Logout() => _state.SetToken(null);

    private record TokenDto(string token);
}
