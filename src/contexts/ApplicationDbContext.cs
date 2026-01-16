using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cyberpunk_market_api.src.models;
using Microsoft.EntityFrameworkCore;

namespace cyberpunk_market_api.src.contexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions) : base(dbContextOptions) { }

        public DbSet<Users> users { get; set; }
        public DbSet<UserRoles> userRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}