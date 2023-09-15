using Microsoft.AspNetCore.Identity;
using PollsAppBlazor.Server.Models;
using PollsAppBlazor.Server.Policy;

namespace PollsAppBlazor.Server.Extensions
{
	public static class WebApplicationExtensions
	{
		public static async void CreateRoles(this WebApplication app)
		{
			using var scope = app.Services.CreateScope();

			var roleManager =
				scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
			var roles = new[] { Roles.Administrator, Roles.Moderator };

			foreach (var role in roles)
			{
				if (!await roleManager.RoleExistsAsync(role))
				{
					await roleManager.CreateAsync(new(role));
				}
			}
		}

		public static async void CreateAdministrator(this WebApplication app)
		{
			// Probably I shouldn't use this in production
			using var scope = app.Services.CreateScope();

			var userManager =
				scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

			string userName = "admin";
			string email = "admin@pollsapp.com";
			string password = "P@ssw0rd1sNotS3cur3^:(";

			if (await userManager.FindByEmailAsync(email) == null)
			{
				ApplicationUser user = new()
				{
					UserName = userName,
					Email = email
				};

				await userManager.CreateAsync(user, password);

				await userManager.AddToRoleAsync(user, Roles.Administrator);
			}
		}
	}
}
