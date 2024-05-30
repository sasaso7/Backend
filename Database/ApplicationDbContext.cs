using BankBackend.Database.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EFGetStarted.Database
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Activity> Activities { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>()
            .HasMany(u => u.Accounts)
            .WithOne()
            .HasForeignKey(a => a.UserId)
            .IsRequired();

            builder.Entity<Activity>()
            .HasOne(a => a.Account)
            .WithMany()
            .HasForeignKey(a => a.AccountId);

            builder.HasDefaultSchema("BankServer");
        }
    }
}
