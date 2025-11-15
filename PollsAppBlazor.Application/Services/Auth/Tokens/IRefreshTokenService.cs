namespace PollsAppBlazor.Application.Services.Auth.Tokens;

/// <summary>
/// An interface for a service that manages refresh tokens.
/// </summary>
public interface IRefreshTokenService
{
    /// <summary>
    /// Generates a refresh token for the user and saves it asynchronously.
    /// </summary>
    /// <param name="userId">ID of the user who will receive the token.</param>
    /// <param name="persistent">Indicates whether the token should be persistent.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>
    /// A a generated refresh token or <see langword="null" /> if user does not exist.
    /// </returns>
    Task<RefreshTokenValue?> GenerateAsync(string userId, bool persistent, CancellationToken cancellationToken);

    /// <summary>
    /// Revokes the refresh token of the user asynchronously
    /// </summary>
    /// <param name="token">Value of the token to be revoked.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>
    /// <see langword="true" /> on success; <see langword="false" /> otherwise.
    /// </returns>
    Task<bool> RevokeAsync(string token, CancellationToken cancellationToken);

    /// <summary>
    /// Revokes all refresh tokens of the user asynchronously.
    /// </summary>
    /// <param name="userId">ID of the user whose tokens will be revoked.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>
    /// <see langword="true" /> on success; <see langword="false" /> otherwise.
    /// </returns>
    Task<bool> RevokeAllUserTokensAsync(string userId, CancellationToken cancellationToken);

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
    Task<RefreshTokenValidationResult> ValidateAsync(string token, CancellationToken cancellationToken);
}