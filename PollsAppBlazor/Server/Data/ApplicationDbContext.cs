using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PollsAppBlazor.Server.Models;

namespace PollsAppBlazor.Server.Data
{
	public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
	{
		public DbSet<Poll> Polls { get; set; }
		public DbSet<Option> Options { get; set; }
		public DbSet<Vote> Votes { get; set; }

		public ApplicationDbContext(
			DbContextOptions options,
			IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
		{
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			// Vote->Poll
			builder.Entity<Vote>()
				.HasOne(v => v.Poll)
				.WithMany(p => p.Votes)
				.HasForeignKey(v => v.PollId)
				.IsRequired()
				.OnDelete(DeleteBehavior.ClientCascade);

			// Vote->Option
			builder.Entity<Vote>()
				.HasOne(v => v.Option)
				.WithMany(p => p.Votes)
				.HasForeignKey(v => v.OptionId)
				.IsRequired()
				.OnDelete(DeleteBehavior.ClientCascade);
		}
	}
}