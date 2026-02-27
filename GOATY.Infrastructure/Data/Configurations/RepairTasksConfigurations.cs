using GOATY.Domain.RepairTasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GOATY.Infrastructure.Data.Configurations
{
    public sealed class RepairTasksConfigurations : IEntityTypeConfiguration<RepairTask>
    {
        public void Configure(EntityTypeBuilder<RepairTask> builder)
        {
            builder.ToTable("RepairTasks");

            builder.HasKey(p => p.Id);

            builder.HasIndex(p => p.Name);

            builder.Property(rt => rt.Name)
               .IsRequired()
               .HasMaxLength(100);

            builder.Property(rt => rt.Description)
               .IsRequired()
               .HasMaxLength(1000);

            builder.Property(rt => rt.TimeEstimated)
           .IsRequired()
           .HasColumnType("decimal(18,2)");

            builder.Property(rt => rt.CostEstimated)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");
        }
    }
}
