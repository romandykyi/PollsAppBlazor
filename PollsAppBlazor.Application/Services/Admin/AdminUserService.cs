using PollsAppBlazor.Application.Services.Auth.Tokens;
using PollsAppBlazor.DataAccess.Repositories.Interfaces;

namespace PollsAppBlazor.Application.Services.Admin;

public class AdminUserService(
    IUserRepository userRepository,
    IRefreshTokenService refreshTokenService
    ) : IAdminUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IRefreshTokenService _refreshTokenService = refreshTokenService;

    public async Task<bool> DeleteUserData(string userName, CancellationToken cancellationToken)
    {
        await _refreshTokenService.RevokeAllUserTokensAsync(userName, cancellationToken);
        return await _userRepository.DeleteUserData(userName, CancellationToken.None);
    }
}
