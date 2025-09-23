namespace PollsAppBlazor.Application.Services.Interfaces;

/// <summary>
/// Service for working with poll options.
/// </summary>
public interface IOptionService
{
    /// <summary>
    /// Gets ID of the Poll that contains the option.
    /// </summary>
    /// <param name="optionId">ID of the option.</param>
    /// <param name="cancellationToken">The cancellation token to use.</param>
    /// <returns>
    /// ID of the Poll or <see langword="null" /> if it doesn't exist.
    /// </returns>
    Task<int?> GetPollIdAsync(int optionId, CancellationToken cancellationToken);
}
