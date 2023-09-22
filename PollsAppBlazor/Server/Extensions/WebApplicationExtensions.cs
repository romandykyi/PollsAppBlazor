using Bogus;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;
using PollsAppBlazor.Server.Data;
using PollsAppBlazor.Server.Models;
using PollsAppBlazor.Shared.Polls;
using PollsAppBlazor.Shared.Users;

namespace PollsAppBlazor.Server.Extensions
{
	public static partial class WebApplicationExtensions
	{
		public static WebApplication CreateRoles(this WebApplication app)
		{
			using var scope = app.Services.CreateScope();

			var roleManager =
				scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
			var roles = new[] { Roles.Administrator, Roles.Moderator };

			foreach (var role in roles)
			{
				if (!roleManager.RoleExistsAsync(role).Result)
				{
					roleManager.CreateAsync(new(role)).Wait();
				}
			}
			return app;
		}

		public static WebApplication CreateAdministrator(this WebApplication app)
		{
			// Probably I shouldn't use this in production
			using var scope = app.Services.CreateScope();

			var userManager =
				scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

			string userName = "admin";
			string email = "admin@pollsapp.com";
			string password = "P@ssw0rd1sNotS3cur3^:(";

			if (userManager.FindByEmailAsync(email).Result == null)
			{
				ApplicationUser user = new()
				{
					UserName = userName,
					Email = email
				};

				userManager.CreateAsync(user, password).Wait();

				userManager.AddToRoleAsync(user, Roles.Administrator).Wait();
			}
			return app;
		}

		public static WebApplication CreateDummyData(this WebApplication app,
			int pollsNumber = 500, int usersNumber = 300,
			int minPollVotes = 15, int maxPollVotes = 250)
		{
			using var scope = app.Services.CreateScope();

			var dbContext =
				scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
			var userManager =
				scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
			var userStore =
				scope.ServiceProvider.GetRequiredService<IUserStore<ApplicationUser>>();
			var emailStore = (IUserEmailStore<ApplicationUser>)userStore;

			Randomizer.Seed = new(5);

			var faker = new Faker("en");

			// Random users
			string[] usersIds = new string[usersNumber];
			for (int i = 0; i < usersNumber; i++)
			{
				var newUser = Activator.CreateInstance<ApplicationUser>();

				Regex rgx = new("[^a-zA-Z0-9]");
				string userName = faker.Internet.UserName();
				userName = rgx.Replace(userName, "");
				string email = faker.Internet.Email();
				userStore.SetUserNameAsync(newUser, userName, CancellationToken.None).Wait();
				emailStore.SetEmailAsync(newUser, email, CancellationToken.None).Wait();
				var result = userManager.CreateAsync(newUser, "Password1!").Result;
				if (!result.Succeeded)
				{
					// Exit, because user with random name is alredy exists,
					// so dummy data have been already generated
					return app;
				}

				usersIds[i] = newUser.Id;
			}

			// Random polls
			DateTimeOffset minExpiryDate = DateTimeOffset.Now - TimeSpan.FromDays(90),
				maxExpiryDate = DateTimeOffset.Now + TimeSpan.FromDays(90);
			var pollFaker = new Faker<Poll>()
				.RuleFor(p => p.CreatorId, f => f.Random.CollectionItem(usersIds))
				.RuleFor(p => p.CreationDate, f => f.Date.RecentOffset(365))
				.RuleFor(p => p.ExpiryDate,
						 f => f.Random.Bool() ? f.Date.BetweenOffset(minExpiryDate, maxExpiryDate) : null)
				.RuleFor(p => p.Title, f => f.Commerce.ProductName())
				.RuleFor(p => p.ResultsVisibleBeforeVoting, f => f.Random.Bool())
				.RuleFor(p => p.Description,
				f => f.Random.Bool() ? string.Join(' ', f.Lorem.Words(5)) : null);
			Poll[] polls = pollFaker.GenerateLazy(pollsNumber).ToArray();
			dbContext.AddRange(polls);
			dbContext.SaveChanges();

			// Random options
			Option[][] options = new Option[pollsNumber][];
			for (int i = 0; i < pollsNumber; i++)
			{
				int optionsCount = faker.Random.Number(PollCreationDto.MinOptionsCount,
					PollCreationDto.MaxOptionsCount);
				options[i] = new Option[optionsCount];
				for (int j = 0; j < optionsCount; j++)
				{
					options[i][j] = new()
					{
						PollId = polls[i].Id,
						Description = faker.Lorem.Word()
					};
				}
			}
			dbContext.AddRange(options.SelectMany(o => o));
			dbContext.SaveChanges();

			// Random votes
			List<Vote> votes = new();
			for (int i = 0; i < pollsNumber; i++)
			{
				int pollVotesCount = faker.Random.Number(minPollVotes, maxPollVotes);
				int startUser = faker.Random.Number(0, usersIds.Length - 1);
				for (int j = 0; j < pollVotesCount; j++)
				{
					int rndOptionIndex = faker.Random.Number(0, options[i].Length - 1);
					Vote vote = new()
					{
						PollId = polls[i].Id,
						OptionId = options[i][rndOptionIndex].Id,
						UserId = usersIds[startUser]
					};
					startUser = (startUser + 1) % usersIds.Length;

					votes.Add(vote);
				}
			}
			dbContext.AddRange(votes);
			dbContext.SaveChanges();
			return app;
		}
	}
}
