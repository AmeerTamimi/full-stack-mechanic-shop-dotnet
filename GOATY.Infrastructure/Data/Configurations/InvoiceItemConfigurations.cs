using GOATY.Domain.WorkOrders.Billing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GOATY.Infrastructure.Data.Configurations
{
    public sealed class InvoiceItemConfigurations : IEntityTypeConfiguration<InvoiceItem>
    {
        public void Configure(EntityTypeBuilder<InvoiceItem> builder)
        {
            builder.ToTable("InvoiceItem");

            builder.HasKey(it => it.Id);

            builder.Property(it => it.InvoiceId)
                   .IsRequired();

            builder.Property(it => it.TechnicianCost)
                   .IsRequired();

            builder.Property(it => it.Quantity)
                   .IsRequired();

            builder.Property(it => it.UnitPrice)
                   .IsRequired();

            builder.Property(it => it.Total)
                   .IsRequired();

            builder.HasOne(it => it.Invoice)
                   .WithMany(i => i.InvoiceItems)
                   .HasForeignKey(it => it.InvoiceId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(it => it.Part)
                   .WithMany()
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(it => it.RepairTask)
                   .WithMany()
                   .OnDelete(DeleteBehavior.NoAction);

            builder.ToTable(t => t.HasCheckConstraint(
                    "CK_InvoiceItem_ExactlyOneSource",
                    @"(
                        ([PartId] IS NOT NULL AND [RepairTaskId] IS NULL)
                        OR
                        ([PartId] IS NULL AND [RepairTaskId] IS NOT NULL)
                    )"
                ));
        }
    }
}
