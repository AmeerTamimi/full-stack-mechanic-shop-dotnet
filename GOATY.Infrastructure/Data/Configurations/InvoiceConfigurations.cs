using GOATY.Domain.WorkOrders.Billing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GOATY.Infrastructure.Data.Configurations
{
    public sealed class InvoiceConfigurations : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.ToTable("Invoices");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.IssuedAt)
                   .IsRequired();

            builder.Property(i => i.Status)
                   .HasConversion<string>()
                   .IsRequired();

            builder.Property(i => i.SubTotal)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(i => i.Discount)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(i => i.Tax)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(i => i.Total)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(i => i.WorkOrderId)
                   .IsRequired();

            builder.HasOne(i => i.WorkOrder)
                   .WithOne(wo => wo.Invoice)
                   .HasForeignKey<Invoice>(i => i.WorkOrderId);
        }
    }
}
