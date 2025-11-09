using PollsAppBlazor.Server.DataAccess.Models;

namespace PollsAppBlazor.Application.Services.Auth.Session;

public interface IAuthSessionManager
{
    /// <summary>
    /// Initiates a new authentication session for the specified user.
    /// </summary>
    /// <param name="user">Owner of the session.</param>
    /// <param name="persistent">Indicates whether the session should be resumed later.</param>
    /// <param name="cancellationToken">A cancellation token to use.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the newly created authentication session.
    /// </returns>
    Task<AuthSession> StartSessionAsync(ApplicationUser user, bool persistent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resumes the current authentication session.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to use.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. 
    /// The task result contains the refresh token validation result.
    /// </returns>
    Task<RefreshResult> ResumeCurrentSessionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalidates the current session.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to use.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a boolean indicating whether the session was successfully invalidated.
    /// </returns>
    Task<bool> InvalidateCurrentSessionAsync(CancellationToken cancellationToken = default);
}
