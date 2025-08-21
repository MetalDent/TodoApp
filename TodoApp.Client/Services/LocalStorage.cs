using Microsoft.JSInterop;

namespace TodoApp.Client.Services
{
    public interface ILocalStorage
    {
        Task<string?> Get(string key);
        Task Set(string key, string value);
        Task Remove(string key);
    }

    public sealed class BrowserLocalStorage : ILocalStorage
    {
        private readonly IJSRuntime _js;
        public BrowserLocalStorage(IJSRuntime js) => _js = js;

        public Task<string?> Get(string key) =>
            _js.InvokeAsync<string?>("localStorage.getItem", key).AsTask();

        public Task Set(string key, string value) =>
            _js.InvokeVoidAsync("localStorage.setItem", key, value).AsTask();

        public Task Remove(string key) =>
            _js.InvokeVoidAsync("localStorage.removeItem", key).AsTask();
    }
}
