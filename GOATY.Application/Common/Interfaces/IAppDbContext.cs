using GOATY.Domain.Customers;
using GOATY.Domain.Customers.Vehicles;
using GOATY.Domain.Employees;
using GOATY.Domain.Identity;
using GOATY.Domain.Parts;
using GOATY.Domain.RepairTasks;
using GOATY.Domain.WorkOrders;
using GOATY.Domain.WorkOrders.Billing;
using Microsoft.EntityFrameworkCore;

namespace GOATY.Application.Common.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<Part> Parts { get; }
        DbSet<Employee> Employees { get; }
        DbSet<RefreshToken> RefreshTokens { get; }
        DbSet<RepairTask> RepairTasks { get; }
        DbSet<Customer> Customers { get; }
        DbSet<Vehicle> Vehicles { get; }
        DbSet<WorkOrder> WorkOrders { get; }
        DbSet<Invoice> Invoices { get;}

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
