using GOATY.Domain.RepairsTask.Parts;

namespace GOATY.Domain.RepairTasks
{
    public sealed class RepairTaskDetails
    {
        public Guid RepairTaskId { get; set; }
        public Guid PartId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public RepairTask RepairTask { get; set; } = null!;
        public Part Part { get; set; } = null!;

        public RepairTaskDetails(
            Guid repairTaskId,
            Guid partId,
            int quantity,
            decimal unitPrice)
        {
            RepairTaskId = repairTaskId;
            PartId = partId;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }
    }
}
