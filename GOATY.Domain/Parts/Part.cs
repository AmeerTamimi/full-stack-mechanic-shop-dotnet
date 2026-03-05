using GOATY.Domain.Common;
using GOATY.Domain.Common.Results;
using GOATY.Domain.RepairTasks;

namespace GOATY.Domain.Parts
{
    public sealed class Part : AuditableEntity
    {
        public string? Name { get; private set; }
        public decimal Cost { get; private set; }
        public int Quantity { get; private set; }
        public List<RepairTaskDetails> RepairTaskDetails { get; private set; } = [];

        private Part() { }
        private Part(
            Guid id,
            string name,
            decimal cost,
            int quantity) 
            : base(id)
        {
            Id = id;
            Name = name;
            Cost = cost;
            Quantity = quantity;
        }
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

        public Result<Updated> Update(string name,
                                             decimal cost,
                                             int quantity)
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

            Name = name;
            Cost = cost;
            Quantity = quantity;

            return Result.Updated;
        }
    }
}
