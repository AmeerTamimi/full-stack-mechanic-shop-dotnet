using GOATY.Domain.RepairTasks;
using GOATY.Domain.WorkOrders;
using GOATY.Domain.WorkOrders.Enums;

namespace GOATY.Application.Features.WorkOrders.DTOs
{
    public sealed class WorkOrderDto
    {
        public State State { get; set; }
        public int TotalTime { get; set; }
        public decimal TotalCost { get; set; }
        public DateTime StartTime { get; set; }
        public Guid VehicleId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid EmployeeId { get; set; }
        public IEnumerable<WorkOrderRepairTasksDto> RepairTasks { get; set; } = [];
    }
}
