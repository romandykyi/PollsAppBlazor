using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using PollsAppBlazor.Client;
using PollsAppBlazor.Client.Auth;
using PollsAppBlazor.Client.Utils;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

if (builder.HostEnvironment.IsProduction())
{
    builder.Logging.SetMinimumLevel(LogLevel.Warning);
}

builder.Services
    .AddScoped<TokenService>()
    .AddScoped<JwtHandler>()
    .AddScoped<AuthenticationStateProvider, JwtAuthStateProvider>()
    .AddAuthorizationCore()
    .AddScoped<LocalStorageUtils>()
    .AddMudServices();

builder.Services.AddHttpClient(HttpClientName.AuthApiClientName, c =>
{
    c.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
}).AddHttpMessageHandler<JwtHandler>();

builder.Services.AddHttpClient(HttpClientName.PublicApiClientName, c =>
{
    c.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
});

await builder.Build().RunAsync();
