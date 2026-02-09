using GOATY.Domain.Common;

namespace GOATY.Domain.Parts
{
    public sealed class Part : AuditableEntity
    {
        public decimal Cost { get; set; }
        public string? Name { get; set; }
        public int Quantity { get; set; }
    }
}
