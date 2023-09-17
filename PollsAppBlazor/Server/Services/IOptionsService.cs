using PollsAppBlazor.Shared.Polls;

namespace PollsAppBlazor.Server.Services
{
	public interface IOptionsService
	{
		/// <summary>
		/// Get ID of a Poll that contains the Option.
		/// </summary>
		/// <param name="optionId">ID of the Option</param>
		/// <returns>
		/// ID of the Option or <see langword="null" /> if Option doesn't exist
		/// </returns>
		Task<int?> GetPollIdAsync(int optionId);

		/// <summary>
		/// Edit an Option.
		/// </summary>
		/// <param name="option">Updated values of the Option</param>
		/// <param name="optionId">ID of the Option</param>
		/// <returns>
		/// <see langword="true"/> if option was edited;
		/// <see langword="false"/> if option wasn't found
		/// </returns>
		Task<bool> EditOptionAsync(OptionEditDto option, int optionId);
	}
}
