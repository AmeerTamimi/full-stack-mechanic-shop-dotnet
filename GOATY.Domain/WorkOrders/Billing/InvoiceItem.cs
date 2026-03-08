using GOATY.Domain.Common.Constans;
using GOATY.Domain.Common.Results;
using GOATY.Domain.Parts;
using GOATY.Domain.RepairTasks;

namespace GOATY.Domain.WorkOrders.Billing
{
    public sealed class InvoiceItem
    {
        public Guid Id { get; private set; }
        public decimal TechnicianCost { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal Total { get; private set; }
        public string Description { get; set; }
        public Guid InvoiceId { get; private set; }
        public Invoice Invoice { get; private set; }
        public Guid? RepairTaskId { get; private set; }
        public Guid? PartId { get; private set; }
        public RepairTask? RepairTask { get; private set; }
        public Part? Part { get; private set; }

        public InvoiceItem(Guid id,
                           Guid invoiceId,
                           decimal technicianCost,
                           int quantity,
                           decimal unitPrice,
                           decimal total,
                           Guid? repairTaskId,
                           Guid? partId)
        {
            Id = id;
            InvoiceId = invoiceId;
            TechnicianCost = technicianCost;
            Quantity = quantity;
            UnitPrice = unitPrice;
            Total = total;
            RepairTaskId = repairTaskId;
            PartId = partId;
        }
        public static Result<InvoiceItem> Create(
            Guid id,
            Guid invoiceId,
            decimal technicianCost,
            int quantity,
            decimal unitPrice,
            Guid? repairTaskId,
            Guid? partId)
        {
            if (Guid.Empty == id)
            {
                return InvoiceItemErrors.InvalidId;
            }
            if (Guid.Empty == invoiceId)
            {
                return InvoiceItemErrors.InvalidInvoiceId;
            }

            if (technicianCost < GOATYConstans.TechnicianBase)
            {
                return InvoiceItemErrors.InvalidTechnicianCost;
            }

            if (quantity <= 0)
            {
                return InvoiceItemErrors.InvalidQuantity;
            }

            if (unitPrice < 0)
            {
                return InvoiceItemErrors.InvalidUnitPrice;
            }

            if (repairTaskId is null && partId is null)
            {
                return InvoiceItemErrors.MissingSource;
            }

            if (repairTaskId is not null && partId is not null)
            {
                return InvoiceItemErrors.ConflictingSources;
            }

            unitPrice += technicianCost;
            decimal total = (quantity * unitPrice);
            
            return new InvoiceItem(
                id,
                invoiceId,
                technicianCost,
                quantity,
                unitPrice,
                total,
                repairTaskId,
                partId);
        }
    }
}
