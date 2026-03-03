using GOATY.Domain.WorkOrders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GOATY.Infrastructure.Data.Configurations
{
    public sealed class WorkOrderRepairTasksConfigurations : IEntityTypeConfiguration<WorkOrderRepairTasks>
    {
        public void Configure(EntityTypeBuilder<WorkOrderRepairTasks> builder)
        {
            builder.ToTable("WorkOrderRepairTasks");

            builder.HasKey(wr => new { wr.WorkOrderId, wr.RepairTaskId });

            builder.HasOne(wr => wr.WorkOrder)
                   .WithMany(wo => wo.WorkOrderRepairTasks)
                   .HasForeignKey(wr => wr.WorkOrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(wr => wr.RepairTask)
                   .WithMany(r => r.WorkOrderRepairTasks)
                   .HasForeignKey(wr => wr.RepairTaskId);
        }
    }
}