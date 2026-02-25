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

            builder.HasMany(p => p.RepairTaskDetails)
                   .WithOne(r => r.RepairTask);
        }
    }
}
