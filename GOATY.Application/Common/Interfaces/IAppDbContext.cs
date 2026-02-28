using GOATY.Domain.Employees;
using GOATY.Domain.Identity;
using GOATY.Domain.RepairsTask.Parts;
using GOATY.Domain.RepairTasks;
using Microsoft.EntityFrameworkCore;

namespace GOATY.Application.Common.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<Part> Parts { get; }
        DbSet<Employee> Employees { get; }
        DbSet<RefreshToken> RefreshTokens { get; }
        DbSet<RepairTask> RepairTasks { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
