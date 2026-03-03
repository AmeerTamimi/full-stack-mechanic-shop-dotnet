using GOATY.Domain.WorkOrders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GOATY.Infrastructure.Data.Configurations
{
    public sealed class WorkOrderConfigurations : IEntityTypeConfiguration<WorkOrder>
    {
        public void Configure(EntityTypeBuilder<WorkOrder> builder)
        {
            builder.ToTable("WorkOrders");

            builder.HasKey(wo => wo.Id);

            builder.HasOne(wo => wo.Vehicle)
                   .WithMany(v => v.WorkOrders)
                   .HasForeignKey(wo => wo.VehicleId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(wo => wo.Customer)
                   .WithMany(c => c.WorkOrders)
                   .HasForeignKey(wo => wo.CustomerId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(wo => wo.Employee)
                   .WithMany(v => v.WorkOrders)
                   .HasForeignKey(wo => wo.EmployeeId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(wo => wo.RepairTasks)
                   .WithOne(r => r.WorkOrder)
                   .HasForeignKey(r => r.WorkOrderId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}