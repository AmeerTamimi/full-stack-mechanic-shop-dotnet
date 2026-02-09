using GOATY.Domain.Parts;
using Microsoft.EntityFrameworkCore;

namespace GOATY.Application.Common
{
    public interface IAppDbContext
    {
        public DbSet<Part> Parts { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
