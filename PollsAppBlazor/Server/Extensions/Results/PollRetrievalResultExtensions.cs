using Microsoft.AspNetCore.Mvc;
using PollsAppBlazor.Application.Services.Results;

namespace PollsAppBlazor.Server.Extensions.Results;

public static class PollRetrievalResultExtensions
{
    public static IActionResult ToActionResult<TPoll>(this PollRetrievalResult<TPoll> result)
    {
        return result.Error switch
        {
            PollRetrievalError.None => new OkObjectResult(result.Poll!),
            PollRetrievalError.PollNotFound => new NotFoundResult(),
            PollRetrievalError.PollDeleted => new BadRequestObjectResult("Poll was deleted"),
            _ => throw new InvalidOperationException($"Unknown {nameof(PollRetrievalError)} value")
        };
    }
}
