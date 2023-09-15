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

			//// Poll->Options
			//builder.Entity<Poll>()
			//	.HasMany(p => p.Options)
			//	.WithOne(o => o.Poll)
			//	.HasForeignKey(o => o.PollId)
			//	.IsRequired()
			//	.OnDelete(DeleteBehavior.Cascade);

			//// Poll->Votes
			//builder.Entity<Poll>()
			//	.HasMany(p => p.Votes)
			//	.WithOne(v => v.Poll)
			//	.HasForeignKey(v => v.PollId)
			//	.IsRequired()
			//	.OnDelete(DeleteBehavior.Cascade);

			//// Options->Votes
			//builder.Entity<Option>()
			//	.HasMany(o => o.Votes)
			//	.WithOne(v => v.Option)
			//	.HasForeignKey(v => v.OptionId)
			//	.IsRequired()
			//	.OnDelete(DeleteBehavior.Cascade);

			// Vote->Poll
			builder.Entity<Vote>()
				.HasOne(v => v.Poll)
				.WithMany(p => p.Votes)
				.HasForeignKey(v => v.PollId)
				.IsRequired(false)
				.OnDelete(DeleteBehavior.NoAction);

			// Vote->Option
			builder.Entity<Vote>()
				.HasOne(v => v.Option)
				.WithMany(p => p.Votes)
				.HasForeignKey(v => v.OptionId)
				.IsRequired(false)
				.OnDelete(DeleteBehavior.NoAction);

			//// User->Votes
			//builder.Entity<ApplicationUser>()
			//	.HasMany(u => u.Votes)
			//	.WithOne(v => v.User)
			//	.HasForeignKey(v => v.UserId)
			//	.IsRequired()
			//	.OnDelete(DeleteBehavior.Cascade);
		}
	}
}