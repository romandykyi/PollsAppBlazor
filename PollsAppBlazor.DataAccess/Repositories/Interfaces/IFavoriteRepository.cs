namespace PollsAppBlazor.DataAccess.Repositories.Interfaces;

public interface IFavoriteRepository
{
    /// <summary>
    /// Checks whether the poll <paramref name="pollId"/> is marked as favorite by
    /// user <paramref name="userId"/>.
    /// </summary>
    /// <param name="pollId">Poll's ID.</param>
    /// <param name="userId">User's ID.</param>
    /// <returns>
    /// <see langword="true"/> if the poll is marked as favorite;
    /// <see langword="false"/> if the favorite doesn't exist.
    /// </returns>
    Task<bool> ExistsAsync(int pollId, string userId);

    /// <summary>
    /// Marks poll <paramref name="pollId"/> as favorite for the user <paramref name="userId"/>.
    /// </summary>
    /// <param name="pollId">Poll's ID.</param>
    /// <param name="userId">User's ID.</param>
    /// <returns>
    /// <see langword="true"/> if the poll was marked as favorite;
    /// <see langword="false"/> if the poll was already marked as favorite.
    /// </returns>
    Task<bool> AddAsync(int pollId, string userId);

    /// <summary>
    /// Removes poll <paramref name="pollId"/> from favorites for the user <paramref name="userId"/>.
    /// </summary>
    /// <param name="pollId">Poll's ID.</param>
    /// <param name="userId">User's ID.</param>
    /// <returns>
    /// <see langword="true"/> if the poll was removed;
    /// <see langword="false"/> if the poll wasn't marked as favorite.
    /// </returns>
    Task<bool> RemoveAsync(int pollId, string userId);
}
