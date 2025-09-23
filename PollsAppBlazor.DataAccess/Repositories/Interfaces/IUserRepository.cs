namespace PollsAppBlazor.DataAccess.Repositories.Interfaces;

public interface IUserRepository
{
    /// <summary>
    /// Soft deletes the user, erases their personal data and all polls
    /// created by the account.
    /// </summary>
    /// <param name="userName">Username of the user to delete.</param>
    /// <param name="cancellationToken">The cancellation token to use.</param>
    /// <returns>
    /// <see langword="true" /> if the user was deleted successfully,
    /// <see langword="false" /> if the user does not exist or is already deleted.
    /// </returns>
    Task<bool> DeleteUserData(string userName, CancellationToken cancellationToken);
}
