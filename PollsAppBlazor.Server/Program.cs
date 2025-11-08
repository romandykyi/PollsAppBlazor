using PollsAppBlazor.Server.Extensions;
using PollsAppBlazor.Server.Extensions.Infrastructure;
using PollsAppBlazor.Server.Extensions.Safety;
using PollsAppBlazor.Server.Extensions.Seeding;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

builder.AddApplicationLogging();

builder.AddApplicationConfigurationOptions();

// Add services to the container.
builder.ConnectDatabase();
builder.ConfigureEmailService();

services
    .RegisterRepositories()
    .RegisterApplicationServices();

services.AddMemoryCache();
services.AddRazorPages();

if (builder.Environment.IsDevelopment())
{
    services.AddSwagger();
    builder.AddDebugOptions();
}

services
    .ConfigureIdentity()
    .AddCustomizedAuthentication()
    .AddCustomizedAuthorization();

services
    .ConfigureControllers(addAntiForgery: false)
    .AddAppRateLimiting();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseProductionErrorHandling();
    app.UseHsts();
}

app.UseMigrationsEndPoint();

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseRateLimiter();

app.UseCookiePolicy();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("index.html");

app
    .CreateRoles()
    .CreateAdministrator();

if (app.Environment.IsDevelopment())
{
    app.CreateDummyData();
}

app.Run();