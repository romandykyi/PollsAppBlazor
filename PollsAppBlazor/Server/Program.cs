using PollsAppBlazor.Server.Extensions;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// Add services to the container.
builder.ConnectDatabase();
builder.ConfigureEmailService();

services
    .RegisterRepositories()
    .RegisterApplicationServices();

services.AddMemoryCache();

if (builder.Environment.IsDevelopment())
{
    services.AddSwagger();
    builder.AddDebugOptions();
}

services.AddAntiforgery(options => options.HeaderName = "X-XSRF-Token");

builder.AddCustomizedIdentity();
services.AddCustomizedAuthorization();

services.ConfigureControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseMigrationsEndPoint();
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseCookiePolicy();
app.UseIdentityServer();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("index.html");

app.CreateRoles();

if (app.Environment.IsDevelopment())
{
    app.CreateAdministrator();
    app.CreateDummyData();
}

app.Run();