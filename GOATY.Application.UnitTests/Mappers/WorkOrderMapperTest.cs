using GOATY.Application.Features.WorkOrders.Mappers;
using GOATY.Domain.WorkOrders;
using GOATY.Domain.WorkOrders.Billing;
using GOATY.Domain.WorkOrders.Enums;
using GOATY.Tests.Common.Customers;
using GOATY.Tests.Common.Employees;
using GOATY.Tests.Common.WorkOrders;
using GOATY.Tests.Common.WorkOrders.Invoices;
using System.Reflection.Emit;

namespace GOATY.Application.UnitTests.Mappers
{
    public sealed class WorkOrderMapperTest
    {
        [Fact]
        public void ToDto_ShoulMapCorrectly()
        {
            var customer = CustomerFactory.Create().Value;
            var vehicle = VehicleFactory.Create().Value;
            var employee = TechnicianFactory.Create().Value;
            var repairTask = WorkOrderRepairTaskFactory.Create().Value;
            var invoice = InvoiceFactory.Create();

            var workOrderId = Guid.NewGuid();
            var workOrder = WorkOrderFactory.Create(
                id: workOrderId,
                vehicleId: vehicle.Id,
                customerId: customer.Id,
                employeeId: employee.Id,
                bay: Bay.A,
                startTime: DateTime.UtcNow.AddMinutes(5),
                discount: 10m,
                repairTasks: [repairTask]).Value;

            var dto = workOrder.ToDto();

            Assert.Equal(workOrder.Id, dto.Id);
            Assert.Equal(workOrder.Bay, dto.Bay);
            Assert.Equal(workOrder.StartTime, dto.StartTime);
            Assert.Equal(workOrder.State, dto.State);

            Assert.NotNull(dto.Employee);
            Assert.Equal(workOrder.EmployeeId, dto.Employee!.Id);
            Assert.Equal($"{employee.FirstName} {employee.LastName}", $"{dto.Employee.FirstName} {dto.Employee.LastName}");

            Assert.NotNull(dto.Vehicle);
            Assert.Equal(vehicle.Id, dto.Vehicle!.Id);
            Assert.Equal(vehicle.Brand, dto.Vehicle.Brand);
            Assert.Equal(vehicle.Model, dto.Vehicle.Model);
            Assert.Equal(vehicle.Year, dto.Vehicle.Year);
            Assert.Equal(vehicle.LicensePlate, dto.Vehicle.LicensePlate);

            Assert.Single(dto.RepairTasks);
            Assert.Equal(workOrder.TotalPartsCost, dto.TotalPartsCost);
            Assert.Equal(workOrder.TotalTechniciansCost, dto.TotalTechniciansCost);
            Assert.Equal(workOrder.TotalCost, dto.TotalCost);
            Assert.Equal(workOrder.TotalTime, dto.TotalTime);
        }
        //public static WorkOrderDto ToDto(this WorkOrder model)
        //{
        //    return new WorkOrderDto
        //    {
        //        Id = model.Id,
        //        Bay = model.Bay,
        //        State = model.State,
        //        TotalTime = model.TotalTime,
        //        TotalCost = model.TotalCost,
        //        StartTime = model.StartTime,
        //        EndTime = model.EndTime,
        //        Vehicle = model.Vehicle is null ? null : model.Vehicle.ToDto(),
        //        Customer = model.Customer is null ? null : model.Customer.ToDto(),
        //        Employee = model.Employee is null ? null : model.Employee.ToDto(),
        //        RepairTasks = model.WorkOrderRepairTasks.ToDtos()
        //    };
        //}
    }
}
