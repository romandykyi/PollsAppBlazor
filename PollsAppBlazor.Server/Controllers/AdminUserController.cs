using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PollsAppBlazor.Application.Services.Admin;
using PollsAppBlazor.Shared.Users;

namespace PollsAppBlazor.Server.Controllers;

[Route("api/admin/users")]
public class AdminUserController(IAdminUserService adminUserService) : ControllerBase
{
    private readonly IAdminUserService _adminUserService = adminUserService;

    /// <summary>
    /// (ADMIN ONLY) Deletes user's personal data and soft deletes their polls and account
    /// </summary>
    /// <response code="204">Success</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Caller lacks permisions</response>
    /// <response code="404">Not found</response>
    /// <response code="409">Attempt to delete yourself</response>
    [HttpDelete]
    [Route("{userName}")]
    [DisableRateLimiting]
    [Authorize(Roles = Roles.Administrator)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteUserData([FromRoute] string userName, CancellationToken cancellationToken)
    {
        string currentUserName = User.Identity!.Name!;
        if (currentUserName == userName) return Conflict();

        bool result = await _adminUserService.DeleteUserData(userName, cancellationToken);
        return result ? NoContent() : NotFound();
    }
}
