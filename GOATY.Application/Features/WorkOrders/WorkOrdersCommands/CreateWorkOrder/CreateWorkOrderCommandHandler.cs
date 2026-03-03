using GOATY.Application.Common;
using GOATY.Application.Common.Interfaces;
using GOATY.Application.Features.WorkOrders.DTOs;
using GOATY.Application.Features.WorkOrders.Mappers;
using GOATY.Domain.Common.Results;
using GOATY.Domain.WorkOrders;
using GOATY.Domain.WorkOrders.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace GOATY.Application.Features.WorkOrders.WorkOrdersCommands.CreateWorkOrder
{
    public sealed class CreateWorkOrderCommandHandler(
        IAppDbContext context,
        ILogger<CreateWorkOrderCommandHandler> logger,
        HybridCache cache)
        : IRequestHandler<CreateWorkOrderCommand, Result<WorkOrderDto>>
    {
        public async Task<Result<WorkOrderDto>> Handle(CreateWorkOrderCommand request, CancellationToken ct)
        {
            var vehicle = await context.Vehicles
                                       .SingleOrDefaultAsync(v => v.Id == request.VehicleId, ct);

            if (vehicle is null)
            {
                return Error.NotFound(
                    code: "Vehicle_NotFound",
                    description: $"Vehicle With Id {request.VehicleId} was Not Found"
                );
            }

            var customer = await context.Customers
                                        .SingleOrDefaultAsync(c => c.Id == request.CustomerId, ct);

            if (customer is null)
            {
                return Error.NotFound(
                    code: "Customer_NotFound",
                    description: $"Customer With Id {request.CustomerId} was Not Found"
                );
            }

            var isCustomerHasVehicle = await context.Customers
                .Where(c => c.Id == request.CustomerId)
                .SelectMany(c => c.Vehicles)
                .AnyAsync(v => v.Id == request.VehicleId);

            if (!isCustomerHasVehicle)
            {
                return ApplicationErrors.CustomerDoesNotOwnVehicle;
            }

            var employee = await context.Employees
                                        .SingleOrDefaultAsync(e => e.Id == request.EmployeeId, ct);

            if (employee is null)
            {
                return Error.NotFound(
                    code: "Employee_NotFound",
                    description: $"Employee With Id {request.EmployeeId} was Not Found"
                );
            }

            var workOrderRepairTasks = request.WorkOrderRepairTasks;

            var duplicatedRepairTaskOnRequest = workOrderRepairTasks
                .GroupBy(wr => wr.Id)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicatedRepairTaskOnRequest.Count > 0)
            {
                var repairTaskId = duplicatedRepairTaskOnRequest[0];

                return Error.Conflict(
                    code: "WorkOrder.RepairTask.DuplicateInRequest",
                    description: $"RepairTask {repairTaskId} is duplicated in the request."
                );
            }

            var WorkOderId = Guid.NewGuid();

            var repairTaskIds = workOrderRepairTasks.Select(wr => wr.Id).ToList();

            

            var repairTasksModelsDb = await context.RepairTasks
                                                   .Where(r => repairTaskIds.Contains(r.Id))
                                                   .ToListAsync(ct);

            var repairTasksIdsDb = repairTasksModelsDb.Select(r => r.Id).ToList();

            var workOderRepairTasksModels = new List<WorkOrderRepairTasks>();

            foreach (var workOrderRepairTask in workOrderRepairTasks)
            {
                var repairTaskId = workOrderRepairTask.Id;

                if (!repairTasksIdsDb.Contains(repairTaskId))
                {
                    return Error.NotFound(
                        code: "RepairTask_NotFound",
                        description: $"RepairTask With Id {repairTaskId} was Not Found"
                    );
                }
            }

            foreach(var repairTask in repairTasksModelsDb) 
            {
                var workOrderRepairTaskModel = WorkOrderRepairTasks.Create(WorkOderId,
                                                                           repairTask.Id,
                                                                           repairTask.TimeEstimated,
                                                                           repairTask.CostEstimated);

                if (!workOrderRepairTaskModel.IsSuccess)
                {
                    return workOrderRepairTaskModel.Errors;
                }

                workOderRepairTasksModels.Add(workOrderRepairTaskModel.Value);
            }

            var workOrderResult = WorkOrder.Create(WorkOderId,
                                             vehicle.Id,
                                             customer.Id,
                                             employee.Id,
                                             request.StartTime,
                                             workOderRepairTasksModels);

            if (!workOrderResult.IsSuccess)
            {
                return workOrderResult.Errors;
            }

            var workOrder = workOrderResult.Value;

            var conflictVehicle = await context.WorkOrders
                .Where(wo => wo.VehicleId == vehicle.Id &&
                      (wo.State == State.InProgress || wo.State == State.Scheduled))
                .ToListAsync(ct);

            if (conflictVehicle.Count > 0)
            {
                return ApplicationErrors.VehicleHasWorkOrderConflict;
            }

            var isOverlappedEmployee = await context.WorkOrders
                .Where(wo => wo.EmployeeId == employee.Id &&
                                wo.StartTime < workOrder.StartTime.AddMinutes(workOrder.TotalTime) &&
                                wo.StartTime.AddMinutes(wo.TotalTime) > workOrder.StartTime)
                .ToListAsync(ct);

            if (isOverlappedEmployee.Count > 0)
            {
                return ApplicationErrors.EmployeeHasWorkOrderOverlap;
            }
            await context.WorkOrders.AddAsync(workOrder , ct);
            await context.SaveChangesAsync(ct);

            await cache.RemoveByTagAsync("work-orders");

            return workOrder.ToDto();
        }
    }
}