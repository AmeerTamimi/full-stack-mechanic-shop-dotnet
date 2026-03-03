using GOATY.Application.Features.WorkOrders.DTOs;
using GOATY.Domain.WorkOrders;
using GOATY.Domain.WorkOrders.Enums;

namespace GOATY.Application.Features.WorkOrders.Mappers
{
    public static class WorkOrderMapper
    {
        public static WorkOrderDto ToDto(this WorkOrder model)
        {
            return new WorkOrderDto
            {
                Id = model.Id,
                Bay = model.Bay,
                State = model.State,
                TotalTime = model.TotalTime,
                TotalCost = model.TotalCost,
                StartTime = model.StartTime,
                VehicleId = model.VehicleId,
                CustomerId = model.CustomerId,
                EmployeeId = model.EmployeeId,
                RepairTasks = model.WorkOrderRepairTasks.ToDtos()
            };
        }
        public static List<WorkOrderDto> ToDtos(this List<WorkOrder> models)
        {
            return models.ConvertAll(model => model.ToDto());
        }
    }
}

