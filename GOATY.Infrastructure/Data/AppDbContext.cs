using GOATY.Application.Common;
using GOATY.Domain.Parts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace GOATY.Infrastructure.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options)
        : DbContext(options), IAppDbContext
    {
        public DbSet<Part> Parts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
