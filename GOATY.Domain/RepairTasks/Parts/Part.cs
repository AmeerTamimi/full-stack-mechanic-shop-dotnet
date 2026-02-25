using GOATY.Domain.Common;
using GOATY.Domain.Common.Results;
using GOATY.Domain.RepairTasks;

namespace GOATY.Domain.RepairsTask.Parts
{
    public sealed class Part : AuditableEntity
    {
        public string? Name { get; set; }
        public decimal Cost { get; set; }
        public int Quantity { get; set; }
        public List<RepairTaskDetails> RepairTaskDetails { get; set; } = [];

        private Part() { }
        private Part(
            Guid id,
            string name,
            decimal cost,
            int quantity) 
            : base(id)
        { }
        public static Result<Part> Create(Guid id,
                                          string name,
                                          decimal cost,
                                          int quantity
                                        )
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return PartErrors.NameInvalidError;
            }

            if (cost <= 0)
            {
                return PartErrors.CostInvalidError; // -> Result<Part>(error)
            }

            if (quantity <= 0)
            {
                return PartErrors.QuantityInvalidError;
            }

            return new Part(id , name , cost , quantity);
        }

        public static Result<Updated> Update(Part partToUpdate ,
                                          string name ,
                                          decimal cost ,
                                          int quantity
                                        )
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return PartErrors.NameInvalidError;
            }

            if (cost <= 0)
            {
                return PartErrors.CostInvalidError;
            }

            if (quantity <= 0)
            {
                return PartErrors.QuantityInvalidError;
            }

            partToUpdate.Name = name;
            partToUpdate.Cost = cost;
            partToUpdate.Quantity = quantity;

            return Result.Updated;
        }
    }
}
