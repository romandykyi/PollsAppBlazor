using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using PollsAppBlazor.Client;
using PollsAppBlazor.Client.Utils;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

string baseUri = builder.HostEnvironment.IsProduction() ?
    "https://pollsappblazor.azurewebsites.net" : "https://localhost:7093";

builder.Services.AddHttpClient("PollsAppBlazor.ServerAPI", client => client.BaseAddress = new Uri(baseUri))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();
builder.Services.AddHttpClient<PublicClient>(client => client.BaseAddress = new Uri(baseUri));

builder.Services.AddScoped<LocalStorageUtils>();
builder.Services.AddMudServices();

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("PollsAppBlazor.ServerAPI"));

builder.Services.AddOidcAuthentication(options =>
{
    options.ProviderOptions.Authority = "https://localhost:7093";
    options.ProviderOptions.ClientId = "PollsAppBlazor.Client";
    options.ProviderOptions.ResponseType = "code";
    options.ProviderOptions.DefaultScopes.Add("openid");
    options.ProviderOptions.DefaultScopes.Add("profile");
    options.ProviderOptions.DefaultScopes.Add("api1");
});

await builder.Build().RunAsync();
