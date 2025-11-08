using PollsAppBlazor.DataAccess.Dto;

namespace PollsAppBlazor.DataAccess.Repositories.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshTokenValidationDto?> GetAsync(string userId, string tokenValue, CancellationToken cancellationToken);

    Task CreateAsync(string userId, string tokenValue, DateTime validTo, CancellationToken cancellationToken);

    Task<bool> RevokeAsync(string userId, string tokenValue, CancellationToken cancellationToken);
}
