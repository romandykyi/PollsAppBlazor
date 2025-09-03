using PollsAppBlazor.Shared.Polls;

namespace PollsAppBlazor.DataAccess.Repositories.Options;

/// <summary>
/// Polls page retrieval options.
/// </summary>
/// <param name="Parameters">Pagination and filtering parameters to use.</param>
/// <param name="CreatorId">Optional ID of the creator to filter polls by.</param>
/// <param name="FavoritesOfUserId">Optional ID of the user to get favorites from.</param>
public record PollsRetrievalOptions(
    PollsPagePaginationParameters Parameters,
    string? CreatorId,
    string? FavoritesOfUserId
    );