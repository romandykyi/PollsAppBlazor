using Microsoft.EntityFrameworkCore;
using PollsAppBlazor.Server.Data;
using PollsAppBlazor.Server.Models;

namespace PollsAppBlazor.Server.Services
{
	public class FavoritesService : IFavoritesService
	{
		private readonly ApplicationDbContext _context;

		public FavoritesService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<bool> AddToFavoritesAsync(int pollId, string userId)
		{
			// If poll doesn't exist
			if (!await _context.Polls.AnyAsync(p => p.Id == pollId))
			{
				return false;
			}

			// If isn't already in favorites
			if (!await _context.Favorites.AnyAsync(f => f.PollId == pollId && f.UserId == userId))
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
