using GOATY.Domain.Common.Results;
using GOATY.Domain.RepairsTask.Parts;
using GOATY.Domain.RepairTasks.Enums;
using System.Xml.Linq;

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

        private RepairTaskDetails() { }
        private RepairTaskDetails(
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

        public static Result<RepairTaskDetails> Create(Guid repairTaskId,
                                                       Guid partId,
                                                       int quantity,
                                                       decimal unitPrice)
        {
            if (Guid.Empty == repairTaskId)
            {
                return RepairTaskErrors.InvalidId;
            }
            if (Guid.Empty == partId)
            {
                return RepairTaskErrors.InvalidPartId;
            }

            if (quantity <= 0)
            {
                return RepairTaskErrors.InvalidQuantity;
            }
            if (unitPrice <= 0)
            {
                return RepairTaskErrors.InvalidUnitPrice;
            }

            return new RepairTaskDetails(repairTaskId, partId, quantity, unitPrice);
        }
    }
}
