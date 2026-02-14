using GOATY.Application.Features.Common;
using GOATY.Domain.Employees;
using GOATY.Domain.Parts;
using Microsoft.EntityFrameworkCore;

namespace GOATY.Infrastructure.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options)
        : DbContext(options), IAppDbContext
    {
        public DbSet<Part> Parts { get; set; }
        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
