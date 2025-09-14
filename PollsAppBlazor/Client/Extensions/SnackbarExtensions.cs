using MudBlazor;

namespace PollsAppBlazor.Client.Extensions;

public static class SnackbarExtensions
{
    public static void UnexpectedError(this ISnackbar snackbar)
    {
        snackbar.Add("An unexpected error has occurred. Please, try again.", Severity.Error,
            options => options.CloseAfterNavigation = true);
    }
}
