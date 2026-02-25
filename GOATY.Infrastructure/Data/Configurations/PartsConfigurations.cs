using GOATY.Domain.RepairsTask.Parts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class PartConfiguration : IEntityTypeConfiguration<Part>
{
    public void Configure(EntityTypeBuilder<Part> builder)
    {
        builder.HasKey(p => p.Id);

        builder.HasIndex(p => p.Name);

        builder.HasMany(p => p.RepairTaskDetails)
               .WithOne(r => r.Part);
    }
}
