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
        public AuthenticationContext(DbContextOptions options) : base(options)
        {
             
        }

        public DbSet<User> users { get; set; }

        public User GetUserByEmail(string email)
        {
            return users.Single(u => u.NormalizedEmail == email.ToUpper());
        }
    }
}
