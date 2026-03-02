using GOATY.Domain.Common.Results;
using GOATY.Domain.Parts;

namespace GOATY.Domain.RepairTasks
{
    public sealed class RepairTaskDetails
    {
        public Guid RepairTaskId { get; private set; }
        public Guid PartId { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public RepairTask RepairTask { get; private set; } = null!;
        public Part Part { get; private set; } = null!;

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

        public Result<Updated> Update(int quantity,
                                      decimal unitPrice)
        {
            if (quantity <= 0)
            {
                return RepairTaskErrors.InvalidQuantity;
            }
            if (unitPrice <= 0)
            {
                return RepairTaskErrors.InvalidUnitPrice;
            }

            Quantity = quantity;
            UnitPrice = unitPrice;

            return Result.Updated;
        }
    }
}