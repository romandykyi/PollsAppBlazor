using PollsAppBlazor.DataAccess.Repositories.Interfaces;

namespace PollsAppBlazor.Application.Admin;

public class AdminUserService(IUserRepository userRepository) : IAdminUserService
{
    private readonly IUserRepository _userRepository = userRepository;

    public Task<bool> DeleteUserData(string userName)
    {
        return _userRepository.DeleteUserData(userName);
    }
}
