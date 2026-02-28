using GOATY.Application.Common.Interfaces;
using GOATY.Domain.Employees;
using GOATY.Domain.Identity;
using GOATY.Domain.RepairsTask.Parts;
using GOATY.Domain.RepairTasks;
using GOATY.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GOATY.Infrastructure.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options)
        : IdentityDbContext<AppUser>(options),
        IAppDbContext
    {
        public DbSet<Part> Parts { get; set; }
        public DbSet<RepairTask> RepairTasks { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
