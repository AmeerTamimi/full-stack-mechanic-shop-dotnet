using GOATY.Domain.RepairTasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GOATY.Infrastructure.Data.Configurations
{
    public sealed class RepairTaskDetailsConfigurations : IEntityTypeConfiguration<RepairTaskDetails>
    {
        public void Configure(EntityTypeBuilder<RepairTaskDetails> builder)
        {
            builder.ToTable("RepairTaskDetails");

            builder.HasKey(rd => new { rd.RepairTaskId, rd.PartId });

            builder.HasOne(rd => rd.RepairTask)
                   .WithMany(r => r.RepairTaskDetails)
                   .HasForeignKey(rd => rd.RepairTaskId);

            builder.HasOne(rd => rd.Part)
                   .WithMany(p => p.RepairTaskDetails)
                   .HasForeignKey(rd => rd.PartId);

            builder.Property(p => p.Quantity)
                   .IsRequired();
        }
    }
}
