using GOATY.Domain.Customers.Vehicles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GOATY.Infrastructure.Data.Configurations
{
    public sealed class VehicleConfigurations : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            builder.ToTable("Vehicles");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.CustomerId)
                   .IsRequired();

            builder.Property(v => v.Brand)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(v => v.Model)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(v => v.LicensePlate)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(v => v.Year)
                   .IsRequired();

            builder.Ignore(v => v.VehicleInfo);
        }
    }
}
