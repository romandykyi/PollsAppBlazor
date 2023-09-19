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
	}
}
