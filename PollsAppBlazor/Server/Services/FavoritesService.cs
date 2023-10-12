using Microsoft.EntityFrameworkCore;
using PollsAppBlazor.Server.Data;
using PollsAppBlazor.Server.Models;
using PollsAppBlazor.Shared.Polls;

namespace PollsAppBlazor.Server.Services
{
	public class FavoritesService 
	{
		private readonly ApplicationDbContext _context;

		public FavoritesService(ApplicationDbContext context)
		{
			_context = context;
		}

		private async Task<bool> DoesExistAsync(int pollId, string userId)
		{
			return await _context.Favorites
				.AsNoTracking()
				.AnyAsync(f => f.PollId == pollId && f.UserId == userId);
		}

        /// <summary>
        /// Get a Poll favorite status for the user.
        /// </summary>
        /// <remarks>
        /// This method doesn't check whether Poll exists.
        /// </remarks>
        /// <param name="pollId">ID of the Poll to be checked</param>
        /// <param name="userId">ID the user</param>
        public async Task<FavoriteDto> GetFavorite(int pollId, string userId)
		{
			return new()
			{
				PollId = pollId,
				IsFavorite = await DoesExistAsync(pollId, userId)
			};
		}

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
        public async Task<bool> AddToFavoritesAsync(int pollId, string userId)
		{
			// If poll doesn't exist
			if (!await _context.Polls
				.AsNoTracking()
				.AnyAsync(p => p.Id == pollId))
			{
				return false;
			}

			// If isn't already in favorites
			if (!await DoesExistAsync(pollId, userId))
			{
				Favorite favorite = new()
				{
					PollId = pollId,
					UserId = userId
				};

				_context.Add(favorite);
				await _context.SaveChangesAsync();
			}

			return true;
		}

        /// <summary>
        /// Remove a Poll from favorites for the user.
        /// </summary>
        /// <param name="pollId">ID of the Poll to be removed from favorites</param>
        /// <param name="userId">ID the user</param>
        /// <returns>
        /// <see langword="true" /> if the Poll was successfully removed from favorites;
        /// otherwise <see langword="false" /> if Poll wasn't in favorites.
        /// </returns>
        public async Task<bool> RemoveFromFavoritesAsync(int pollId, string userId)
		{
			var favorite = await _context.Favorites.
				FirstOrDefaultAsync(f => f.PollId == pollId && f.UserId == userId);

			// If favorite doesn't exist
			if (favorite == null)
			{
				return false;
			}

			_context.Remove(favorite);
			await _context.SaveChangesAsync();

			return true;
		}
	}
}
