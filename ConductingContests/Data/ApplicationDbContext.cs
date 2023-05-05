using ConductingContests.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ConductingContests.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public override DbSet<User> Users { get; set; }
        public DbSet<Contest> Contests { get; set; }
        public DbSet<ContestCategory> ContestCategories { get; set; }
        public DbSet<OfferedService> OfferedServices { get; set; }
        public DbSet<ParticipationRequest> ParticipationRequests { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Contest>()
                .Property(x => x.Status)
                .HasConversion<string>();
            
            builder.Entity<ParticipationRequest>()
                .Property(x => x.Status)
                .HasConversion<string>();
        }
    }
}

