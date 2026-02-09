using GOATY.Domain.Parts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class PartConfiguration : IEntityTypeConfiguration<Part>
{
    public void Configure(EntityTypeBuilder<Part> builder)
    {
        builder.HasKey(p => p.Id);

        builder.HasData(
            new Part
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Engine Oil",
                Cost = 25.50m,
                Quantity = 100,
                CreatedAtUtc = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
                LastModifiedUtc = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero)
            },
            new Part
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Brake Pads",
                Cost = 45.00m,
                Quantity = 50,
                CreatedAtUtc = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
                LastModifiedUtc = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero)
            },
            new Part
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "Air Filter",
                Cost = 15.75m,
                Quantity = 200,
                CreatedAtUtc = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
                LastModifiedUtc = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero)
            },
            new Part
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Name = "Spark Plug",
                Cost = 8.99m,
                Quantity = 300,
                CreatedAtUtc = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
                LastModifiedUtc = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero)
            },
            new Part
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                Name = "Battery",
                Cost = 120.00m,
                Quantity = 20,
                CreatedAtUtc = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
                LastModifiedUtc = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero)
            }
        );
    }
}
