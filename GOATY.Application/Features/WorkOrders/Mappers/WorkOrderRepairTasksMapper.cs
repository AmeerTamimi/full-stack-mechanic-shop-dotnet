using GOATY.Application.Features.WorkOrders.DTOs;
using GOATY.Domain.Common.Enums;
using GOATY.Domain.WorkOrders;

namespace GOATY.Application.Features.WorkOrders.Mappers
{
    public static class WorkOrderRepairTasksMapper
    {
        public static WorkOrderRepairTasksDto ToDto(this WorkOrderRepairTasks model)
        {
            return new WorkOrderRepairTasksDto
            {
                WorkOrderId = model.WorkOrderId,
                RepairTaskId = model.RepairTaskId,
                Time = model.Time,
                Cost = model.Cost,
            };
        }
        public static List<WorkOrderRepairTasksDto> ToDtos(this IEnumerable<WorkOrderRepairTasks> models)
        {
            return models.Select(model => model.ToDto()).ToList();
        }
    }
}
