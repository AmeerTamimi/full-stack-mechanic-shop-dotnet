using GOATY.Application.Features.Customers.Mappers;
using GOATY.Application.Features.Employees.Mapping;
using GOATY.Application.Features.WorkOrders.DTOs;
using GOATY.Domain.WorkOrders;

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
                EndTime = model.EndTime,
                Vehicle = model.Vehicle is null ? null : model.Vehicle.ToDto(),
                Customer = model.Customer is null ? null : model.Customer.ToDto(),
                Employee = model.Employee is null ? null : model.Employee.ToDto(),
                RepairTasks = model.WorkOrderRepairTasks.ToDtos()
            };
        }
        public static List<WorkOrderDto> ToDtos(this List<WorkOrder> models)
        {
            return models.ConvertAll(model => model.ToDto());
        }
    }
}

