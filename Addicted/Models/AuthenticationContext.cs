using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Addicted.Models
{
    public class AuthenticationContext : IdentityDbContext
    {
        public AuthenticationContext(DbContextOptions<AuthenticationContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bet>().HasMany(b => b.BetOptions).WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }


        public DbSet<User> Users { get; set; }
        public DbSet<Bet> Bets { get; set; }
        public DbSet<Offer> Offer { get; set; }
        public DbSet<BetOption> BetOption { get; set; }

        public List<User> GetAllUsers()
        {
            return Users.Include(u => u.Coins).ToList();
        }
        public User GetUserByEmail(string email)
        {
            return Users.Single(u => u.NormalizedEmail == email.ToUpper());
        }
    }
}
