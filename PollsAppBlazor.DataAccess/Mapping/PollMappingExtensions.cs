using PollsAppBlazor.Server.DataAccess.Models;
using PollsAppBlazor.Shared.Options;
using PollsAppBlazor.Shared.Polls;
using PollsAppBlazor.Shared.Users;

namespace PollsAppBlazor.DataAccess.Mapping;

public static class PollMappingExtensions
{
    public static PollViewDto ToPollViewDto(this Poll poll)
    {
        return new PollViewDto()
        {
            Id = poll.Id,
            Title = poll.Title,
            Description = poll.Description,
            CreationDate = poll.CreationDate,
            ExpiryDate = poll.ExpiryDate,
            Creator = new PollCreatorDto()
            {
                Id = poll.CreatorId,
                Username = poll.Creator!.UserName!
            },
            ResultsVisibleBeforeVoting = poll.ResultsVisibleBeforeVoting,
            // Select options
            Options = poll.Options!.Select(o => new OptionViewDto()
            {
                Id = o.Id,
                Description = o.Description
            }).ToList()
        };
    }
}
