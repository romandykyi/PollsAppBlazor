using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PollsAppBlazor.DataAccess.Repositories.Interfaces;
using PollsAppBlazor.Server.DataAccess;

namespace PollsAppBlazor.DataAccess.Repositories.Implementations;

public class UserRepository(ApplicationDbContext dbContext) : IUserRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<bool> DeleteUserData(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName)) return false;

        string? userId = await _dbContext.Users
            .AsNoTracking()
            .Where(u => u.UserName == userName)
            .Select(u => u.Id)
            .FirstOrDefaultAsync();
        if (userId == null) return false;

        await using IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            await _dbContext.Users
                .Where(x => x.Id == userId)
                .ExecuteUpdateAsync(x => x
                    .SetProperty(x => x.UserName, _ => null)
                    .SetProperty(x => x.NormalizedUserName, _ => null)
                    .SetProperty(x => x.Email, _ => null)
                    .SetProperty(x => x.NormalizedEmail, _ => null)
                    .SetProperty(x => x.PasswordHash, _ => null)
                    .SetProperty(x => x.EmailConfirmed, false)
                    .SetProperty(x => x.IsDeleted, true)
                );

            await _dbContext.Polls
                .Where(x => x.CreatorId == userId)
                .ExecuteUpdateAsync(x => x
                    .SetProperty(x => x.IsDeleted, _ => true)
                );

            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}