using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using PollsAppBlazor.Server.Data;
using PollsAppBlazor.Server.Models;
using PollsAppBlazor.Server.Policy;
using System.Net;
using System.Reflection;

namespace PollsAppBlazor.Server.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddSwagger(this IServiceCollection services)
		{
			services.AddSwaggerGen(options =>
			{
				options.SwaggerDoc("v1", new()
				{
					Title = "Polls App",
					Version = "v1"
				});

				var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
			});

			return services;
		}

		public static IServiceCollection AddCustomizedIdentity(this IServiceCollection services)
		{
			services
				.AddDefaultIdentity<ApplicationUser>(options =>
				{
					options.Password.RequireDigit = true;
					options.Password.RequireLowercase = false;
					options.Password.RequireUppercase = false;
					options.Password.RequireNonAlphanumeric = false;
					options.Password.RequiredLength = 6;
					options.Password.RequiredUniqueChars = 3;
				})
				.AddRoles<IdentityRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>();

			services.AddIdentityServer()
				.AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

			services
				.AddAuthentication()
				.AddJwtBearer();

			services.ConfigureApplicationCookie(options =>
			{
				// Return 401 when user is not authrorized
				options.Events.OnRedirectToLogin = context =>
				{
					context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
					return Task.CompletedTask;
				};
				// Return 403 when user don't have access permission
				options.Events.OnRedirectToAccessDenied = context =>
				{
					context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
					return Task.CompletedTask;
				};
			});

			return services;
		}

		public static IServiceCollection AddCustomizedAuthorization(this IServiceCollection services)
		{
			services.AddTransient<IAuthorizationHandler, PollEditAuthorizationHandler>();
			services.AddTransient<IAuthorizationHandler, OptionEditAuthorizationHandler>();

			services.AddAuthorization(options =>
			{
				options.AddPolicy(Policies.CanEditPoll, policy =>
				{
					policy.RequireAuthenticatedUser();
					policy.AddRequirements(new PollEditAuthorizationRequirement());
				});
				options.AddPolicy(Policies.CanEditOption, policy =>
				{
					policy.RequireAuthenticatedUser();
					policy.AddRequirements(new OptionEditAuthorizationRequirement());
				});
			});

			return services;
		}
	}
}
