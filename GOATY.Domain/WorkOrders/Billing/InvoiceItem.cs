using GOATY.Domain.Parts;
using GOATY.Domain.RepairTasks;

namespace GOATY.Domain.WorkOrders.Billing
{
    public sealed class InvoiceItem
    {
        public Guid Id { get; set; }
        public int TechnicianCost { get; private set; }
        public int Quantity { get; private set; }
        public int UnitPrice { get; private set; }
        public int Total { get; private set; }
        public Guid? RepairTaskId { get; private set; }
        public Guid? PartId { get; private set; }
        public RepairTask? RepairTask { get; private set; }
        public Part? Part { get; private set; }

        public InvoiceItem(Guid id,
                           int technicianCost,
                           int quantity,
                           int unitPrice,
                           int total,
                           Guid? repairTaskId,
                           Guid? partId)
        {
            Id = id;
            TechnicianCost = technicianCost;
            Quantity = quantity;
            UnitPrice = unitPrice;
            Total = total;
            RepairTaskId = repairTaskId;
            PartId = partId;
        }
    }
}
