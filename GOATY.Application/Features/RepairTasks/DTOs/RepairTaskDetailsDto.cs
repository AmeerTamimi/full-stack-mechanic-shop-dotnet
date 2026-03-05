using GOATY.Application.Features.Parts.DTOs;
using GOATY.Domain.Parts;
using GOATY.Domain.RepairTasks;

namespace GOATY.Application.Features.RepairTasks.DTOs
{
    public sealed class RepairTaskDetailsDto
    {
        public PartDto? Part { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
