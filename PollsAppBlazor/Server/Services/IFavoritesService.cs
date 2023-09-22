namespace PollsAppBlazor.Server.Services
{
	public interface IFavoritesService
	{
		/// <summary>
		/// Add a Poll to favorites for the user.
		/// </summary>
		/// <remarks>
		/// If the Poll is already in favorites then nothing will happen.
		/// </remarks>
		/// <param name="pollId">ID of the Poll to be addded to favorites</param>
		/// <param name="userId">ID the user</param>
		/// <returns>
		/// <see langword="true" /> if the Poll was added to favorites or was already in favorites;
		/// otherwise <see langword="false" /> if the Poll was not found.
		/// </returns>
		Task<bool> AddToFavoritesAsync(int pollId, string userId);

		/// <summary>
		/// Remove a Poll from favorites for the user.
		/// </summary>
		/// <param name="pollId">ID of the Poll to be removed from favorites</param>
		/// <param name="userId">ID the user</param>
		/// <returns>
		/// <see langword="true" /> if the Poll was successfully removed from favorites;
		/// otherwise <see langword="false" /> if Poll wasn't in favorites.
		/// </returns>
		Task<bool> RemoveFromFavoritesAsync(int pollId, string userId);
	}
}
