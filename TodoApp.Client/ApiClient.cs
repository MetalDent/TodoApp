using System.Net.Http.Headers;
using System.Net.Http.Json;
using TodoApp.Client.Services;

namespace TodoApp.Client;

public class ApiClient
{
    private readonly HttpClient _http;
    private readonly ILocalStorage _storage;

    public ApiClient(HttpClient http, ILocalStorage storage)
    {
        _http = http;
        _storage = storage;
    }

    private async Task AddBearerAsync()
    {
        var token = await _storage.Get("token");
        _http.DefaultRequestHeaders.Authorization =
            string.IsNullOrWhiteSpace(token) ? null : new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<T?> Get<T>(string url)
    {
        await AddBearerAsync();
        return await _http.GetFromJsonAsync<T>(url);
    }

    public async Task<HttpResponseMessage> Post<T>(string url, T body)
    {
        await AddBearerAsync();
        return await _http.PostAsJsonAsync(url, body);
    }

    public async Task<HttpResponseMessage> Put<T>(string url, T body)
    {
        await AddBearerAsync();
        return await _http.PutAsJsonAsync(url, body);
    }

    public async Task<HttpResponseMessage> Delete(string url)
    {
        await AddBearerAsync();
        return await _http.DeleteAsync(url);
    }
}