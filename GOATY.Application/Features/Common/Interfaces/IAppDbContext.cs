using GOATY.Domain.Employees;
using GOATY.Domain.Parts;
using GOATY.Domain.RefreshTokens;
using Microsoft.EntityFrameworkCore;

namespace GOATY.Application.Features.Common.Interfaces
{
    public interface IAppDbContext
    {
        public DbSet<Part> Parts { get; }
        public DbSet<Employee> Employees { get; }
        public DbSet<RefreshToken> RefreshTokens { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
