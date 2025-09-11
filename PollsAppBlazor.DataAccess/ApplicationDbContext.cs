using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PollsAppBlazor.Server.DataAccess.Models;

namespace PollsAppBlazor.Server.DataAccess;

public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
{
    public DbSet<Poll> Polls { get; set; }
    public DbSet<Option> Options { get; set; }
    public DbSet<Vote> Votes { get; set; }
    public DbSet<Favorite> Favorites { get; set; }

    public ApplicationDbContext(
        DbContextOptions options,
        IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // User->CreatedPolls
        builder.Entity<ApplicationUser>()
            .HasMany(u => u.CreatedPolls)
            .WithOne(p => p.Creator)
            .HasForeignKey(p => p.CreatorId)
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientCascade);

        // Votes->Poll
        builder.Entity<Vote>()
            .HasOne(v => v.Poll)
            .WithMany(p => p.Votes)
            .HasForeignKey(v => v.PollId)
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientCascade);

        // Votes->Option
        builder.Entity<Vote>()
            .HasOne(v => v.Option)
            .WithMany(p => p.Votes)
            .HasForeignKey(v => v.OptionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientCascade);

        // Favorites->Poll
        builder.Entity<Favorite>()
            .HasOne(f => f.Poll)
            .WithMany(p => p.Favorites)
            .HasForeignKey(f => f.PollId)
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientCascade);

        // Users->Polls
        builder.Entity<ApplicationUser>()
            .HasMany(u => u.FavoritePolls)
            .WithMany()
            .UsingEntity<Favorite>();
    }
}