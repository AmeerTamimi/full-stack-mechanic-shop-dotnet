using GOATY.Domain.Common.Enums;
using GOATY.Domain.RepairTasks;
using GOATY.Domain.WorkOrders;

namespace GOATY.Application.Features.WorkOrders.DTOs
{
    public sealed class WorkOrderRepairTasksDto
    {
        public Guid WorkOrderId { get; set; }
        public Guid RepairTaskId { get; set; }
        public TimeStamps Time { get; set; }
        public decimal Cost { get; set; }
    }
}
