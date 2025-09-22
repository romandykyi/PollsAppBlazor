using MudBlazor;

namespace PollsAppBlazor.Client.Extensions;

public static class SnackbarExtensions
{
    public static void TooManyRequestsError(this ISnackbar snackbar)
    {
        snackbar.Add("Too many requests. Please try again later.", Severity.Warning,
            options => options.CloseAfterNavigation = true);
    }

    public static void Error(this ISnackbar snackbar, string errorMessage)
    {
        snackbar.Add(errorMessage, Severity.Error,
            options => options.CloseAfterNavigation = true);
    }

    public static void UnexpectedError(this ISnackbar snackbar)
    {
        snackbar.Error("An unexpected error has occurred. Please, try again.");
    }
}
