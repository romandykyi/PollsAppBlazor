using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PollsAppBlazor.Client.Auth;

public class JwtAuthStateProvider(TokenService tokenService) : AuthenticationStateProvider
{
    private readonly TokenService _tokenService = tokenService;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = _tokenService.AccessToken;
        if (string.IsNullOrEmpty(token))
        {
            bool refreshResult = await _tokenService.TryRefreshAsync();
            if (!refreshResult)
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
            token = _tokenService.AccessToken;
        }

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        var identity = new ClaimsIdentity(jwt.Claims, "jwt");
        var user = new ClaimsPrincipal(identity);
        Console.WriteLine("User is authenticated: " + user.Identity?.Name);
        return new AuthenticationState(user);
    }

    public void Notify() => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
}
