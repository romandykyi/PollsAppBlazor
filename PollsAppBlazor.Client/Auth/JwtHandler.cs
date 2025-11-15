using System.Net;
using System.Net.Http.Headers;

namespace PollsAppBlazor.Client.Auth;

public class JwtHandler(TokenService tokenService) : DelegatingHandler
{
    private readonly TokenService _tokenService = tokenService;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage req, CancellationToken ct)
    {
        var token = _tokenService.AccessToken;
        if (!string.IsNullOrEmpty(token))
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var res = await base.SendAsync(req, ct);
        if (res.StatusCode != HttpStatusCode.Unauthorized)
            return res;

        // Try to refresh the token if we got a 401 Unauthorized
        if (!await _tokenService.TryRefreshAsync()) return res;

        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.AccessToken);
        res.Dispose();
        return await base.SendAsync(req, ct);
    }
}
