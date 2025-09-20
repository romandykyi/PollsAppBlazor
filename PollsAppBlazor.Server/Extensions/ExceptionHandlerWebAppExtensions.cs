using Microsoft.AspNetCore.Diagnostics;

namespace PollsAppBlazor.Server.Extensions;

public static class ExceptionHandlerWebAppExtensions
{
    public static WebApplication UseProductionErrorHandling(this WebApplication app)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var exceptionFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                if (exceptionFeature != null)
                {
                    // Example of structured error response
                    var error = new
                    {
                        Message = "An unexpected error occurred. Please try again later.",
                        exceptionFeature.Path
                    };

                    await context.Response.WriteAsJsonAsync(error);
                }
            });
        });
        return app;
    }
}
