namespace PollsAppBlazor.Application.Admin;

public interface IAdminUserService
{
    /// <summary>
    /// Soft deletes the user, their polls and erases user's personal data.
    /// </summary>
    /// <param name="userName">Username of the user to delete.</param>
    /// <returns>
    /// <see langword="true" /> if the user was deleted successfully,
    /// <see langword="false" /> if the user does not exist or is already deleted.
    /// </returns>
    Task<bool> DeleteUserData(string userName);
}
