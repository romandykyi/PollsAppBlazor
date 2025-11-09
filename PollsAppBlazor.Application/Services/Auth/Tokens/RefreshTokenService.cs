using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using PollsAppBlazor.Application.Options;
using PollsAppBlazor.DataAccess.Repositories.Interfaces;
using PollsAppBlazor.Server.DataAccess.Models;
using System.Security.Cryptography;

namespace PollsAppBlazor.Application.Services.Auth.Tokens;

public class RefreshTokenService(
    UserManager<ApplicationUser> userManager,
    IRefreshTokenRepository repository,
    IOptions<RefreshTokenOptions> options) : IRefreshTokenService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IRefreshTokenRepository _repository = repository;
    private readonly RefreshTokenOptions _tokenOptions = options.Value;

    /// <summary>
    /// Generates a refresh token for the user and saves it asynchronously.
    /// </summary>
    /// <param name="userId">ID of the user who will receive the token.</param>
    /// <param name="persistent">Indicates whether the token should be persistent.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>
    /// A a generated refresh token or <see langword="null" /> if user does not exist.
    /// </returns>
    public async Task<RefreshTokenValue?> GenerateAsync(string userId, bool persistent, CancellationToken cancellationToken)
    {
        // Check if user exists
        if (await _userManager.FindByIdAsync(userId) == null)
            return null;

        // Generate a token
        string tokenValue;
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            int bytesSize = ((4 * _tokenOptions.Size / 3) + 3) & ~3;
            byte[] refreshTokenBytes = new byte[bytesSize];
            rng.GetBytes(refreshTokenBytes);
            tokenValue = Convert.ToBase64String(refreshTokenBytes);
        }

        // Set the expiration date and assign token to the user
        DateTime validTo = persistent ?
            DateTime.UtcNow.AddDays(_tokenOptions.ExpirationDays) :
            DateTime.UtcNow.AddMinutes(_tokenOptions.ShortExpirationMinutes);

        // Save the token
        RefreshTokenValue token = new(tokenValue, persistent, validTo);
        await _repository.CreateAsync(userId, token, cancellationToken);

        return token;
    }

    /// <summary>
    /// Revokes the refresh token of the user asynchronously
    /// </summary>
    /// <param name="userId">ID of the user whose token is being revoked.</param>
    /// <param name="token">Value of the token to be revoked.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>
    /// <see langword="true" /> on success; <see langword="false" /> otherwise.
    /// </returns>
    public Task<bool> RevokeAsync(string userId, string token, CancellationToken cancellationToken)
    {
        return _repository.RevokeAsync(userId, token, cancellationToken);
    }

    /// <summary>
    /// Checks whether refresh token is valid asynchronously.
    /// </summary>
    /// <remarks>
    /// May rotate the token value as a side effect.
    /// </remarks>
    /// <param name="userId">ID of the user whose token is being validated.</param>
    /// <param name="token">Value of the token to be validated.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. 
    /// The task result contains the validated refresh token value or 
    /// <see langword="null" /> if the token is invalid.
    /// </returns>
    public async Task<RefreshTokenValue?> ValidateAsync(string userId, string token, CancellationToken cancellationToken)
    {
        var refreshToken = await _repository.GetAsync(userId, token, cancellationToken);

        if (refreshToken == null || DateTime.UtcNow > refreshToken.ExpiresAt)
        {
            // Token is invalid or expired
            return null;
        }

        return new RefreshTokenValue(refreshToken.Value, refreshToken.Persistent, refreshToken.ExpiresAt);
    }
}