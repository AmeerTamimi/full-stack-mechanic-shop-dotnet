using GOATY.Application.Features.Billing.DTOs;
using GOATY.Application.Features.Customers.DTOs;
using GOATY.Application.Features.Employees.DTOs;
using GOATY.Domain.Customers;
using GOATY.Domain.Customers.Vehicles;
using GOATY.Domain.Employees;
using GOATY.Domain.RepairTasks;
using GOATY.Domain.WorkOrders;
using GOATY.Domain.WorkOrders.Billing;
using GOATY.Domain.WorkOrders.Enums;

namespace GOATY.Application.Features.WorkOrders.DTOs
{
    public sealed class WorkOrderDto
    {
        public Guid Id { get; set; }
        public State State { get; set; }
        public decimal Discount { get; set; }
        public int TotalTime { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalPartsCost { get; set; }
        public decimal TotalTechniciansCost { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public Bay Bay { get; set; }
        public VehicleDto? Vehicle { get; set; }
        public CustomerDto? Customer { get; set; }
        public EmployeeDto? Employee { get; set; }
        public InvoiceDto? Invoice { get; set; }
        public IEnumerable<WorkOrderRepairTasksDto> RepairTasks { get; set; } = [];
    }
}
