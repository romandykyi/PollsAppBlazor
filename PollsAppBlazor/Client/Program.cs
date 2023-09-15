using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using PollsAppBlazor.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("PollsAppBlazor.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
	.AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services.AddMudServices();

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("PollsAppBlazor.ServerAPI"));

builder.Services.AddApiAuthorization();

await builder.Build().RunAsync();
