using GOATY.Domain.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GOATY.Infrastructure.Data.Configurations
{
    public sealed class CustomerConfigurations : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.FirstName)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(c => c.LastName)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(c => c.Phone)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(c => c.Email)
                   .IsRequired()
                   .HasMaxLength(256);

            builder.Property(c => c.Address)
                   .IsRequired()
                   .HasMaxLength(300);

            builder.HasIndex(c => c.FirstName);

            builder.Ignore(c => c.FullName);

            builder.HasMany(c => c.Vehicles)
                   .WithOne(v => v.Customer)
                   .HasForeignKey(v => v.CustomerId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(c => c.Vehicles)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
