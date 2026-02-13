using GOATY.Domain.Common;
using GOATY.Domain.Common.Results;

namespace GOATY.Domain.Parts
{
    public sealed class Part : AuditableEntity
    {
        public string? Name { get; set; }
        public decimal Cost { get; set; }
        public int Quantity { get; set; }


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

            return new Part
            {
                Id = id,
                Cost = cost,
                Name = name ,
                Quantity = quantity
            };
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
