using GOATY.Domain.Common;
using GOATY.Domain.Common.Results;

namespace GOATY.Domain.Parts
{
    public sealed class Part : AuditableEntity
    {
        public string? Name { get; set; }
        public decimal Cost { get; set; }
        public int Quantity { get; set; }


        public static Result<Part> Create(Guid id, string name, decimal cost, int quantity)
        {
            if(cost == 0)
            {
                return PartErrors.CostRequiredError; // -> Result<Part>(error)
            }

            if(name is null)
            {
                return PartErrors.NameRequiredError;
            }

            if(quantity == 0)
            {
                return PartErrors.QuantityRequiredError;
            }

            return new Part
            {
                Id = id,
                Cost = cost,
                Name = name ,
                Quantity = quantity
            };
        }
    }
}
