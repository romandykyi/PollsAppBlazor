namespace PollsAppBlazor.Server.Services
{
	public interface IVotesService
	{
		/// <summary>
		/// Count votes for option.
		/// </summary>
		/// <param name="optionId">ID of the option which votes need to be count</param>
		/// <returns>
		/// Number of votes on the option
		/// </returns>
		Task<int> CountVotesAsync(int optionId);

		/// <summary>
		/// Get ID of an option which was voted by the user
		/// </summary>
		/// <param name="pollId">ID of the poll</param>
		/// <param name="userId">ID of the user</param>
		/// <returns>
		/// ID of voted by user option, or
		/// <see langword="null" /> if user didn't vote on this poll;
		/// </returns>
		Task<int?> GetVotedOptionAsync(int pollId, string userId);

		/// <summary>
		/// Vote for option
		/// </summary>
		/// <param name="pollId">ID of the poll which contains this option</param>
		/// <param name="optionId">ID of the option</param>
		/// <param name="userId">ID of the user who votes</param>
		Task VoteAsync(int pollId, int optionId, string userId);
	}
}
