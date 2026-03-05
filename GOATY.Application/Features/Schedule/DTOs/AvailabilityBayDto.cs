using GOATY.Application.Features.Employees.DTOs;
using GOATY.Domain.WorkOrders;
using GOATY.Domain.WorkOrders.Enums;

namespace GOATY.Application.Features.Schedule.DTOs
{
    public sealed class AvailabilityBayDto
    {
        public Guid? WorkOrderId { get; set; }
        public Bay Bay { get; set; }
        public DateTimeOffset StartAt { get; set; }
        public DateTimeOffset EndAt { get; set; }
        public string? Vehicle { get; set; }
        public EmployeeDto? Employee { get; set; }
        public bool IsOccupied { get; set; }
        public bool? IsAvailable { get; set; }
        public bool WorkOrderLocked { get; set; }
        public State? State { get; set; }
        public List<WorkOrderRepairTasks>? WorkOrderRepairTasks { get; set; }
    }
}