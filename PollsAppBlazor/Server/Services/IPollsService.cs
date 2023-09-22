﻿using PollsAppBlazor.Server.Models;
using PollsAppBlazor.Shared.Polls;

namespace PollsAppBlazor.Server.Services
{
	public interface IPollsService
	{
		/// <summary>
		/// Get ID of user who created the Poll.
		/// </summary>
		/// <param name="pollId">ID of a Poll</param>
		/// <returns>
		/// ID of the user who created Poll or <see langword="null" />
		/// if Poll was not found.
		/// </returns>
		Task<string?> GetCreatorIdAsync(int pollId);

		/// <summary>
		/// Check whether Poll is available for voting
		/// </summary>
		/// <param name="pollId"></param>
		/// <returns>
		/// <see langword="true" /> if Poll is available for voting,
		/// <see langword="false" /> otherwise
		/// </returns>
		Task<bool> IsPollActiveAsync(int pollId);

		/// <summary>
		/// Get a Poll by its ID.
		/// </summary>
		/// <param name="pollId">ID of the Poll we need to get</param>
		/// <param name="userId">ID of the user who requests a poll. Can be null</param>
		/// <returns>
		/// View of the Poll, or <see langword="null" /> if Poll was not found.
		/// Votes numbers will be included if user has permission to view them
		/// </returns>
		Task<PollViewDto?> GetByIdAsync(int pollId, string? userId = null);

		/// <summary>
		/// Filter Polls query into page.
		/// </summary>
		/// <returns>
		/// Polls from query that match given filter.
		/// </returns>
		Task<PollsPage> FilterPollsAsync(PollsPageFilter filter, IQueryable<Poll> query);

		/// <summary>
		/// Get Polls that meet filter.
		/// </summary>
		/// <returns>
		/// Polls that match given filter.
		/// </returns>
		Task<PollsPage> GetPollsAsync(PollsPageFilter filter);

		/// <summary>
		/// Create a Poll.
		/// </summary>
		/// <param name="poll">Poll DTO used for its creation</param>
		/// <param name="creatorId">ID of a user who creates the Poll</param>
		/// <returns>
		/// View of created Poll.
		/// </returns>
		Task<PollViewDto> CreatePollAsync(PollCreationDto poll, string creatorId);

		/// <summary>
		/// Get an editing representation of the Poll by its ID.
		/// </summary>
		/// <param name="pollId">ID of the Poll we need to get</param>
		/// <returns>
		/// editing representation of the Poll, or <see langword="null" /> if Poll was not found
		/// </returns>
		Task<PollCreationDto?> GetForEditById(int pollId);

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
		Task<bool?> EditPollAsync(PollEditDto poll, int pollId);

		/// <summary>
		/// Delete a Poll by its ID.
		/// </summary>
		/// <param name="pollId">ID of a Poll that needs to be deleted</param>
		/// <returns>
		/// <see langword="true" /> if the Poll was succesfully deleted;
		/// otherwise <see langword="false"/> if the Poll was not found.
		/// </returns>
		Task<bool> DeletePollAsync(int pollId);

		/// <summary>
		/// Make poll expired.
		/// </summary>
		/// <param name="pollId"></param>
		/// <returns>
		/// <see langword="true" /> on success;
		/// <see langword="false" /> if poll is already expired;
		/// <see langword="null" /> if poll was not found
		/// </returns>
		public Task<bool?> ExpirePollAsync(int pollId);
	}
}
