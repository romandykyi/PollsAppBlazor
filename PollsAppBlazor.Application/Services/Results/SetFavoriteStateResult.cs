namespace PollsAppBlazor.Application.Services.Results;

public enum SetFavoriteStateResult
{
    /// <summary>
    /// The state was successfully updated or was already in the requested state.
    /// </summary>
    Success,
    /// <summary>
    /// Poll was not found.
    /// </summary>
    NotFound,
    /// <summary>
    /// Poll was deleted.
    /// </summary>
    Deleted
}
