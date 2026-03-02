using GOATY.Domain.Parts;
using GOATY.Domain.RepairTasks;

namespace GOATY.Application.Features.RepairTasks.DTOs
{
    public sealed class RepairTaskDetailsDto
    {
        public Guid RepairTaskId { get; set; }
        public Guid PartId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
