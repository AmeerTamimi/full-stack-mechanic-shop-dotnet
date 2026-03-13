using GOATY.Application.Features.Billing.Mappers;
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
                State = model.State,
                Discount = model.Discount,
                TotalTime = model.TotalTime,
                TotalCost = model.TotalCost,
                TotalPartsCost = model.TotalPartsCost,
                TotalTechniciansCost = model.TotalTechniciansCost,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                Bay = model.Bay,
                Vehicle = model.Vehicle is null ? null : model.Vehicle.ToDto(),
                Customer = model.Customer is null ? null : model.Customer.ToDto(),
                Employee = model.Employee is null ? null : model.Employee.ToDto(),
                Invoice = model.Invoice is null ? null : model.Invoice.ToDto(),
                RepairTasks = model.WorkOrderRepairTasks.ToDtos()
            };
        }
        public static List<WorkOrderDto> ToDtos(this List<WorkOrder> models)
        {
            return models.ConvertAll(model => model.ToDto());
        }
    }
}

