using PollsAppBlazor.Shared.Auth;
using System.Net.Http.Json;

namespace PollsAppBlazor.Client.Auth;

public class TokenService(IHttpClientFactory http)
{
    private readonly IHttpClientFactory _http = http;
    private string? _accessToken;

    public string? AccessToken => _accessToken;

    public void SetToken(string token)
    {
        _accessToken = token;
    }

    public void Clear()
    {
        _accessToken = null;
    }

    public async Task<bool> TryRefreshAsync()
    {
        var client = _http.CreateClient(HttpClientName.PublicApiClientName);
        var res = await client.PostAsync("/api/auth/refresh", null);
        if (!res.IsSuccessStatusCode)
        {
            Clear();
            return false;
        }

        var dto = await res.Content.ReadFromJsonAsync<AccessDto>();
        if (dto?.AccessToken == null)
        {
            Clear();
            return false;
        }
        SetToken(dto.AccessToken);
        return true;
    }
}