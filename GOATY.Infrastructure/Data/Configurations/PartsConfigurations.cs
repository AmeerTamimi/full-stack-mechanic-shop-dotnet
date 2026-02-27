using GOATY.Domain.RepairsTask.Parts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class PartConfiguration : IEntityTypeConfiguration<Part>
{
    public void Configure(EntityTypeBuilder<Part> builder)
    {
        builder.HasKey(p => p.Id);

        builder.HasIndex(p => p.Name);

        builder.Property(p => p.Name)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(p => p.Cost)
               .IsRequired()
               .HasColumnType("decimal(18,2)");

        builder.Property(p => p.Quantity)
               .IsRequired();
    }
}
