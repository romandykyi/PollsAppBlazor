using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using PollsAppBlazor.Server.Data;
using PollsAppBlazor.Server.Models;
using PollsAppBlazor.Shared.Polls;
using System.Text.Json.Serialization;

namespace PollsAppBlazor.Server.Services
{
	public class PollsService
	{
		// We need a crator ID for some checks later, so this class is introduced to avoid
		// additional DB queries
		private class PollViewDtoWithCreatorId : PollViewDto
		{
			[JsonIgnore]
			public string CreatorId { get; set; } = null!;
		}

		private readonly ApplicationDbContext _dataContext;
		private readonly VotesService _votesService;

		public PollsService(ApplicationDbContext dataContext, VotesService votesService)
		{
			_dataContext = dataContext;
			_votesService = votesService;
		}

        /// <summary>
        /// Get ID of user who created the Poll.
        /// </summary>
        /// <param name="pollId">ID of a Poll</param>
        /// <returns>
        /// ID of the user who created Poll or <see langword="null" />
        /// if Poll was not found.
        /// </returns>
        public async Task<string?> GetCreatorIdAsync(int pollId)
		{
			return await _dataContext.Polls
				.AsNoTracking()
				.Where(p => p.Id == pollId)
				.Select(p => p.CreatorId)
				.FirstOrDefaultAsync();
		}

        /// <summary>
        /// Check whether Poll is available for voting
        /// </summary>
        /// <param name="pollId"></param>
        /// <returns>
        /// <see langword="true" /> if Poll is available for voting,
        /// <see langword="false" /> otherwise
        /// </returns>
        public async Task<bool> IsPollActiveAsync(int pollId)
		{
			// Assuming that poll exists
			return await _dataContext.Polls
				.AsNoTracking()
				.Where(p => p.Id == pollId)
				.Select(p => p.IsActive)
				.FirstAsync();
		}

        /// <summary>
        /// Get a Poll by its ID.
        /// </summary>
        /// <param name="pollId">ID of the Poll we need to get</param>
        /// <param name="userId">ID of the user who requests a poll. Can be null</param>
        /// <returns>
        /// View of the Poll, or <see langword="null" /> if Poll was not found.
        /// Votes numbers will be included if user has permission to view them
        /// </returns>
        public async Task<PollViewDto?> GetByIdAsync(int pollId, string? userId = null)
		{
			// Try to get a poll with options from DB
			var poll = await _dataContext.Polls
				.Include(p => p.Options!)
				.Include(p => p.Creator)
				.AsNoTracking()
				.Where(p => p.Id == pollId)
				// Select poll
				.Select(p => new PollViewDtoWithCreatorId()
				{
					Id = p.Id,
					Title = p.Title,
					Description = p.Description,
					CreationDate = p.CreationDate,
					ExpiryDate = p.ExpiryDate,
					Creator = p.Creator!.UserName!,
					CreatorId = p.CreatorId,
					AreVotesVisible = p.ResultsVisibleBeforeVoting,
					// Select options
					Options = p.Options!.Select(o => new OptionViewDto()
					{
						Id = o.Id,
						Description = o.Description
					}).ToList()
				}).FirstOrDefaultAsync();

			if (poll == null) return null;

			// Determine poll status
			if (userId != null)
			{
				// If user created this Poll
				if (poll.CreatorId == userId)
				{
					poll.CurrentUserCanEdit = true;
					poll.AreVotesVisible = true;
				}
				// If user has voted
				int? votedOptionId = await _votesService.GetVotedOptionAsync(pollId, userId);
				if (votedOptionId != null)
				{
					poll.AreVotesVisible = true;
					poll.VotedOptionId = votedOptionId;
				}
			}
			if (!poll.AreVotesVisible)
			{
				poll.AreVotesVisible = poll.IsExpired;
			}
			if (poll.AreVotesVisible)
			{
				// Add votes counts to options
				for (int i = 0; i < poll.Options.Count; i++)
				{
					var option = poll.Options[i];
					option.VotesCount = await _votesService.CountVotesAsync(option.Id);
				}
			}

			return poll;
		}

        /// <summary>
        /// Filter Polls query into page.
        /// </summary>
        /// <returns>
        /// Polls from query that match given filter.
        /// </returns>
        public async Task<PollsPage> FilterPollsAsync(PollsPageFilter filter, IQueryable<Poll> query)
		{
			// Don't show expired Polls
			if (!filter.ShowExpired)
			{
				query = query
					.Where(p => p.ExpiryDate == null || DateTimeOffset.Now < p.ExpiryDate);
			}
			// Search by title
			if (filter.Title != null)
			{
				query = query
					.Where(p => p.Title!.Contains(filter.Title));
			}
			// Search by creator
			if (filter.Creator != null)
			{
				query = query
					.Where(p => p.Creator!.UserName!.Contains(filter.Creator));
			}
			// Sort
			query = filter.SortMode switch
			{
				PollsSortMode.MostVoted => query.OrderByDescending(p => p.Votes!.Count),
				PollsSortMode.Oldest => query.OrderBy(p => p.CreationDate),
				// Newest
				_ => query.OrderByDescending(p => p.CreationDate)
			};

			// Count all matching Polls
			int count = await query.CountAsync();

			// Select only needed data
			var filteredQuery = query.Select(p => new PollPreviewDto()
			{
				Id = p.Id,
				Title = p.Title,
				CreationDate = p.CreationDate,
				ExpiryDate = p.ExpiryDate,
				Creator = p.Creator!.UserName!,
				VotesCount = p.Votes!.Count
			});

			// Apply pagination
			filteredQuery = filteredQuery
				.Skip(filter.PageSize * (filter.Page - 1))
				.Take(filter.PageSize);

			return new()
			{
				TotalPollsCount = count,
				Polls = await filteredQuery.ToListAsync()
			};
		}

        /// <summary>
        /// Get Polls that meet filter.
        /// </summary>
        /// <returns>
        /// Polls that match given filter.
        /// </returns>
        public async Task<PollsPage> GetPollsAsync(PollsPageFilter filter)
		{
			return await FilterPollsAsync(filter,
				_dataContext.Polls
				.Include(p => p.Creator)
				.Include(p => p.Votes)
				.AsNoTracking());
		}

        /// <summary>
        /// Create a Poll.
        /// </summary>
        /// <param name="poll">Poll DTO used for its creation</param>
        /// <param name="creatorId">ID of a user who creates the Poll</param>
        /// <returns>
        /// View of created Poll.
        /// </returns>
        public async Task<PollViewDto> CreatePollAsync(PollCreationDto poll, string creatorId)
		{
			// Create a poll
			Poll newPoll = new()
			{
				Title = poll.Title,
				Description = poll.Description,
				CreationDate = DateTimeOffset.Now,
				ExpiryDate = poll.ExpiryDate,
				CreatorId = creatorId,
				ResultsVisibleBeforeVoting = poll.ResultsVisibleBeforeVoting
			};
			_dataContext.Add(newPoll);
			await _dataContext.SaveChangesAsync();

			// Create options for this poll
			var options = poll.Options.Select(o => new Option()
			{
				Description = o.Description,
				PollId = newPoll.Id
			});
			_dataContext.AddRange(options);
			await _dataContext.SaveChangesAsync();

			// Return created poll
			return (await GetByIdAsync(newPoll.Id))!;
		}

        /// <summary>
        /// Get an editing representation of the Poll by its ID.
        /// </summary>
        /// <param name="pollId">ID of the Poll we need to get</param>
        /// <returns>
        /// editing representation of the Poll, or <see langword="null" /> if Poll was not found
        /// </returns>
        public async Task<PollCreationDto?> GetForEditById(int pollId)
		{
			return await _dataContext.Polls
				.AsNoTracking()
				.Include(p => p.Options)
				.Where(p => p.Id == pollId)
				.Select(p => new PollCreationDto()
				{
					Title = p.Title,
					Description = p.Description,
					ExpiryDate = p.ExpiryDate,
					ResultsVisibleBeforeVoting = p.ResultsVisibleBeforeVoting,
					Options = p.Options!.Select(o => new OptionCreationDto()
					{
						Description = o.Description
					}).ToList()
				})
				.FirstOrDefaultAsync();
		}

        /// <summary>
        /// Edit a Poll.
        /// </summary>
        /// <param name="poll">Updated values of the Poll</param>
        /// <param name="pollId">ID of the Poll</param>
        /// <returns>
        /// <see langword="true" /> if the Poll was succesfully edited;
        /// <see langword="false"/> if the Poll is not active;
        /// otherwise <see langword="null"/> if the Poll was not found
        /// </returns>
        public async Task<bool?> EditPollAsync(PollEditDto poll, int pollId)
		{
			Poll? actualPoll = await _dataContext.Polls
				.FirstOrDefaultAsync(p => p.Id == pollId);

			if (actualPoll == null)
			{
				// Poll doesn't exist
				return null;
			}
			if (!actualPoll.IsActive)
			{
				// Poll is not active
				return false;
			}

			if (poll.Title != null) actualPoll.Title = poll.Title;
			if (poll.Description != null) actualPoll.Description = poll.Description;
			if (poll.ResultsVisibleBeforeVoting != null) 
				actualPoll.ResultsVisibleBeforeVoting = poll.ResultsVisibleBeforeVoting == true;

			_dataContext.Update(actualPoll);
			await _dataContext.SaveChangesAsync();

			return true;
		}

        /// <summary>
        /// Delete a Poll by its ID.
        /// </summary>
        /// <param name="pollId">ID of a Poll that needs to be deleted</param>
        /// <returns>
        /// <see langword="true" /> if the Poll was succesfully deleted;
        /// otherwise <see langword="false"/> if the Poll was not found.
        /// </returns>
        public async Task<bool> DeletePollAsync(int pollId)
		{
			Poll? poll = await _dataContext.Polls
				.Include(p => p.Options)
				.Include(p => p.Votes)
				.Include(p => p.Favorites)
				.FirstOrDefaultAsync(p => p.Id == pollId);
			if (poll == null)
			{
				// Poll is not found
				return false;
			}

			_dataContext.Remove(poll);
			await _dataContext.SaveChangesAsync();

			return true;
		}

        /// <summary>
        /// Make poll expired.
        /// </summary>
        /// <param name="pollId"></param>
        /// <returns>
        /// <see langword="true" /> on success;
        /// <see langword="false" /> if poll is already expired;
        /// <see langword="null" /> if poll was not found
        /// </returns>
        public async Task<bool?> ExpirePollAsync(int pollId)
		{
			var poll = await _dataContext.Polls.FirstOrDefaultAsync(p => p.Id == pollId);
			if (poll == null) return null;

			if (!poll.IsActive) return false;

			poll.ExpiryDate = DateTimeOffset.Now;
			_dataContext.Update(poll);
			await _dataContext.SaveChangesAsync();

			return true;
        }
	}
}
