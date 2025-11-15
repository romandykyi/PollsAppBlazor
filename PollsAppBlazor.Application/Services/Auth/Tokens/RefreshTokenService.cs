using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using PollsAppBlazor.Application.Options;
using PollsAppBlazor.DataAccess.Models;
using PollsAppBlazor.DataAccess.Repositories.Interfaces;
using PollsAppBlazor.Server.DataAccess.Models;
using System.Security.Cryptography;
using System.Text;

namespace PollsAppBlazor.Application.Services.Auth.Tokens;

public class RefreshTokenService(
    UserManager<ApplicationUser> userManager,
    IRefreshTokenRepository repository,
    IOptions<RefreshTokenOptions> options) : IRefreshTokenService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IRefreshTokenRepository _repository = repository;
    private readonly RefreshTokenOptions _tokenOptions = options.Value;

    private string GenerateTokenValue()
    {
        string tokenValue;
        using RandomNumberGenerator rng = RandomNumberGenerator.Create();

        int bytesSize = ((4 * _tokenOptions.Size / 3) + 3) & ~3;
        byte[] refreshTokenBytes = new byte[bytesSize];
        rng.GetBytes(refreshTokenBytes);
        tokenValue = Convert.ToBase64String(refreshTokenBytes);

        return tokenValue;
    }

    private string HashTokenValue(string tokenValue)
    {
        byte[] secretKey = Encoding.UTF8.GetBytes(_tokenOptions.SecretKey);
        using var hmac = new HMACSHA256(secretKey);
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(tokenValue));
        return Convert.ToBase64String(hashBytes);
    }

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
        string tokenValue = GenerateTokenValue();
        string hashedTokenValue = HashTokenValue(tokenValue);

        // Set the expiration date and assign token to the user
        DateTime validTo = persistent ?
            DateTime.UtcNow.AddDays(_tokenOptions.ExpirationDays) :
            DateTime.UtcNow.AddMinutes(_tokenOptions.ShortExpirationMinutes);

        // Save the token
        RefreshToken token = new()
        {
            UserId = userId,
            TokenHash = hashedTokenValue,
            ValidTo = validTo,
            Persistent = persistent
        };
        await _repository.CreateAsync(userId, token, cancellationToken);

        string encodedValue = RefreshTokenEncoder.Encode(token.Id, tokenValue);
        return new RefreshTokenValue(encodedValue, persistent, validTo);
    }

    /// <summary>
    /// Revokes all refresh tokens of the user asynchronously.
    /// </summary>
    /// <param name="userId">ID of the user whose tokens will be revoked.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>
    /// <see langword="true" /> on success; <see langword="false" /> otherwise.
    /// </returns>
    public Task<bool> RevokeAllUserTokensAsync(string userId, CancellationToken cancellationToken)
    {
        return _repository.RevokeAllUserTokensAsync(userId, cancellationToken);
    }

    /// <summary>
    /// Revokes the refresh token of the user asynchronously
    /// </summary>
    /// <param name="token">Value of the token to be revoked.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>
    /// <see langword="true" /> on success; <see langword="false" /> otherwise.
    /// </returns>
    public Task<bool> RevokeAsync(string token, CancellationToken cancellationToken)
    {
        if (!RefreshTokenEncoder.TryDecode(token, out Guid tokenId, out _))
        {
            return Task.FromResult(false);
        }
        return _repository.RevokeAsync(tokenId, cancellationToken);
    }

    /// <summary>
    /// Checks whether refresh token is valid asynchronously.
    /// </summary>
    /// <remarks>
    /// May rotate the token value as a side effect.
    /// </remarks>
    /// <param name="token">Value of the token to be validated.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. 
    /// The task result contains the validation result.
    /// </returns>
    public async Task<RefreshTokenValidationResult> ValidateAsync(string token, CancellationToken cancellationToken)
    {
        if (!RefreshTokenEncoder.TryDecode(token, out Guid tokenId, out string tokenValue))
        {
            return RefreshTokenValidationResult.Fail(RefreshFailureReason.InvalidToken);
        }

        var refreshToken = await _repository.GetAsync(tokenId, cancellationToken);
        if (refreshToken == null)
        {
            return RefreshTokenValidationResult.Fail(RefreshFailureReason.InvalidToken);
        }
        // Check the token validity
        if (refreshToken.TokenHash != HashTokenValue(tokenValue))
        {
            return RefreshTokenValidationResult.Fail(RefreshFailureReason.InvalidToken);
        }
        if (DateTime.UtcNow > refreshToken.ValidTo)
        {
            return RefreshTokenValidationResult.Fail(RefreshFailureReason.ExpiredToken);
        }

        // Rotate the token
        string newTokenValue = GenerateTokenValue();
        string newHashedTokenValue = HashTokenValue(newTokenValue);
        if (!await _repository.ReplaceAsync(tokenId, newHashedTokenValue, cancellationToken))
        {
            return RefreshTokenValidationResult.Fail(RefreshFailureReason.InvalidToken);
        }

        string encodedNewTokenValue = RefreshTokenEncoder.Encode(tokenId, newTokenValue);
        RefreshTokenValue updatedValue = new(encodedNewTokenValue, refreshToken.Persistent, refreshToken.ValidTo);
        return RefreshTokenValidationResult.Success(updatedValue, refreshToken.UserId);
    }
}