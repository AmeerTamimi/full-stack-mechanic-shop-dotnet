using GOATY.Application.Common;
using GOATY.Application.Common.Interfaces;
using GOATY.Domain.Common.Results;
using GOATY.Domain.RepairTasks;
using GOATY.Domain.WorkOrders;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace GOATY.Application.Features.WorkOrders.WorkOrdersCommands.UpdateWorkOrder
{
    public sealed class UpdateWorkOrderCommandHandler(
        IAppDbContext context,
        ILogger<UpdateWorkOrderCommandHandler> logger,
        HybridCache cache)
        : IRequestHandler<UpdateWorkOrderCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(UpdateWorkOrderCommand request, CancellationToken ct)
        {
            var workOrder = await context.WorkOrders
                .Include(wo => wo.WorkOrderRepairTasks)
                .SingleOrDefaultAsync(wo => wo.Id == request.Id, ct);

            if (workOrder is null)
            {
                return Error.NotFound(
                    code: "WorkOrder_NotFound",
                    description: $"WorkOrder With Id {request.Id} was Not Found"
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

            var vehicle = await context.Vehicles
                .SingleOrDefaultAsync(v => v.Id == request.VehicleId, ct);

            if (vehicle is null)
            {
                return Error.NotFound(
                    code: "Vehicle_NotFound",
                    description: $"Vehicle With Id {request.VehicleId} was Not Found"
                );
            }

            var isCustomerHasVehicle = await context.Vehicles
                .AnyAsync(v => v.CustomerId == customer.Id &&
                               v.Id == vehicle.Id, ct);

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

            var workOrderRepairTasksRequest = request.WorkOrderRepairTasks;

            var duplictedRepairTaskInRequest = workOrderRepairTasksRequest
                .GroupBy(g => g.Id)
                .Where(gr => gr.Count() > 1)
                .Select(gr => gr.Key)
                .ToList();

            if (duplictedRepairTaskInRequest.Count > 0)
            {
                var repairTaskId = duplictedRepairTaskInRequest[0];

                return Error.Conflict(
                    code: "WorkOrder.RepairTask.DuplicateInRequest",
                    description: $"RepairTask {repairTaskId} is duplicated in the request."
                );
            }

            var repairTaskIdsRequest = workOrderRepairTasksRequest
                .Select(wr => wr.Id)
                .ToList();

            var repairTasksDb = await context.RepairTasks
                .Where(r => repairTaskIdsRequest.Contains(r.Id))
                .ToListAsync(ct);

            foreach (var repairTaskId in repairTaskIdsRequest)
            {
                if (!repairTasksDb.Any(r => r.Id == repairTaskId))
                {
                    return Error.NotFound(
                        code: "RepairTask_NotFound",
                        description: $"RepairTask With Id {repairTaskId} was Not Found"
                    );
                }
            }

            var workOrderRepairTasksModels = new List<WorkOrderRepairTasks>();

            foreach (var repairTask in repairTasksDb)
            {
                var workOrderRepairTaskModel = WorkOrderRepairTasks.Create(
                    workOrder.Id,
                    repairTask.Id,
                    repairTask.TimeEstimated,
                    repairTask.CostEstimated);

                if (!workOrderRepairTaskModel.IsSuccess)
                {
                    return workOrderRepairTaskModel.Errors;
                }

                workOrderRepairTasksModels.Add(workOrderRepairTaskModel.Value);
            }

            var isConflictVehicle = await context.WorkOrders
                .AnyAsync(wo => wo.Id != workOrder.Id &&
                                wo.VehicleId == vehicle.Id &&
                                workOrder.EndTime > wo.StartTime &&
                                workOrder.StartTime < wo.StartTime.AddMinutes(wo.TotalTime),
                                ct);

            if (isConflictVehicle)
            {
                return ApplicationErrors.VehicleHasWorkOrderConflict;
            }

            var isOverlappedEmployee = await context.WorkOrders
                .AnyAsync(wo => wo.Id != workOrder.Id &&
                                wo.EmployeeId == employee.Id &&
                                workOrder.EndTime > wo.StartTime &&
                                workOrder.StartTime < wo.StartTime.AddMinutes(wo.TotalTime),
                                ct);

            if (isOverlappedEmployee)
            {
                return ApplicationErrors.EmployeeHasWorkOrderOverlap;
            }

            var isOverlappedBay = await context.WorkOrders
                .AnyAsync(wo => wo.Id != workOrder.Id &&
                                wo.Bay == request.Bay &&
                                workOrder.EndTime > wo.StartTime &&
                                workOrder.StartTime < wo.StartTime.AddMinutes(wo.TotalTime),
                                ct);

            if (isOverlappedBay)
            {
                return ApplicationErrors.BayHasWorkOrderOverlap;
            }

            var updateResult = workOrder.Update(request.VehicleId,
                                                request.CustomerId,
                                                request.EmployeeId,
                                                request.StartTime,
                                                request.Bay,
                                                workOrderRepairTasksModels);
            if (!updateResult.IsSuccess)
            {
                return updateResult.Errors;
            }

            var upsertResult = workOrder.UpsertRepairTasks(workOrderRepairTasksModels);

            if (!upsertResult.IsSuccess)
            {
                return upsertResult.Errors;
            }

            await context.SaveChangesAsync(ct);

            await cache.RemoveByTagAsync("work-orders");

            return Result.Updated;
        }
    }
}