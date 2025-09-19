using Microsoft.JSInterop;

namespace PollsAppBlazor.Client.Utils;

public class LocalStorageUtils(IJSRuntime jsr)
{
    private readonly IJSRuntime _jsRuntime = jsr;

    public async ValueTask SetAsync(string key, string value) => await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, value);

    public async ValueTask<string?> GetAsync(string key, string? defaultValue = null) => await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key) ?? defaultValue;

    public async ValueTask RemoveAsync(string key) => await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);

    public async ValueTask SetSessionAsync(string key, string value) => await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", key, value);

    public async ValueTask<string?> GetSessionAsync(string key, string? defaultValue = null) => await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", key) ?? defaultValue;

    public async ValueTask RemoveSessionAsync(string key) => await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", key);
}
