﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PollsAppBlazor.Server.Data;
using PollsAppBlazor.Server.Models;
using PollsAppBlazor.Shared.Polls;
using System.Text.Json.Serialization;

namespace PollsAppBlazor.Server.Services
{
    public class PollsService : IPollsService
	{
		// We need a crator ID for some checks later, so this class is introduced to avoid
		// additional DB queries
		private class PollViewDtoWithCreatorId : PollViewDto
		{
			[JsonIgnore]
			public string CreatorId { get; set; } = null!;
		}

		private readonly ApplicationDbContext _dataContext;
		private readonly IVotesService _votesService;

		public PollsService(ApplicationDbContext dataContext, IVotesService votesService)
		{
			_dataContext = dataContext;
			_votesService = votesService;
		}

		/// <inheritdoc />
		public async Task<string?> GetCreatorIdAsync(int pollId)
		{
			return await _dataContext.Polls
				.AsNoTracking()
				.Where(p => p.Id == pollId)
				.Select(p => p.CreatorId)
				.FirstOrDefaultAsync();
		}

		/// <inheritdoc />
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
					Creator = p.Creator!.UserName!,
					CreatorId = p.CreatorId,
					// Select options
					Options = p.Options!.Select(o => new OptionViewDto()
					{
						Id = o.Id,
						Description = o.Description
					}).ToList()
				}).FirstOrDefaultAsync();

			if (poll == null) return poll;

			// Check whether user can view votes(they created this poll or voted on it)
			if (userId != null && (poll.CreatorId == userId ||
				await _votesService.GetVotedOptionAsync(pollId, userId) != null))
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

		/// <inheritdoc />
		public async Task<IEnumerable<PollPreviewDto>> GetNewestPollsAsync(int count)
		{
			return await _dataContext.Polls
				.Include(p => p.Creator)
				.AsNoTracking()
				.OrderByDescending(p => p.CreationDate)
				.Select(p => new PollPreviewDto()
				{
					Id = p.Id,
					Title = p.Title,
					CreationDate = p.CreationDate,
					Creator = p.Creator!.UserName!
				})
				.Take(count)
				.ToListAsync();
		}

		/// <inheritdoc />
		public async Task<PollViewDto> CreatePollAsync(PollCreationDto poll, string creatorId)
		{
			// Create a poll
			Poll newPoll = new()
			{
				Title = poll.Title,
				Description = poll.Description,
				CreationDate = DateTimeOffset.Now,
				CreatorId = creatorId
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

		/// <inheritdoc />
		public async Task<bool> EditPollAsync(PollEditDto poll, int pollId)
		{
			Poll? actualPoll = await _dataContext.Polls
				.FirstOrDefaultAsync(p => p.Id == pollId);

			if (actualPoll == null)
			{
				// Poll doesn't exist
				return false;
			}

			if (poll.Title != null) actualPoll.Title = poll.Title;
			if (poll.Description != null) actualPoll.Description = poll.Description;

			_dataContext.Update(actualPoll);
			await _dataContext.SaveChangesAsync();

			return true;
		}

		/// <inheritdoc />
		public async Task<bool> DeletePollAsync(int pollId)
		{
			Poll? poll = await _dataContext.Polls
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
	}
}