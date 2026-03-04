using GOATY.Application.Common;
using GOATY.Application.Common.Interfaces;
using GOATY.Domain.Common.Results;
using GOATY.Domain.Customers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace GOATY.Application.Features.WorkOrders.WorkOrdersCommands.UpdateVehicle
{
    public sealed class UpdateVehicleCommandHandler(
        IAppDbContext context,
        ILogger<UpdateVehicleCommandHandler> logger,
        HybridCache cache,
        IWorkOrderRules _workOrderRules)
        : IRequestHandler<UpdateVehicleCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(UpdateVehicleCommand request, CancellationToken ct)
        {
            var workOrder = await context.WorkOrders
                .Include(wo => wo.WorkOrderRepairTasks)
                .SingleOrDefaultAsync(wo => wo.Id == request.WorkOrderId, ct);

            if (workOrder is null)
            {
                return Error.NotFound(
                    code: "WorkOrder_NotFound",
                    description: $"WorkOrder With Id {request.WorkOrderId} was Not Found"
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

            if (!await _workOrderRules.IsCustomerHasVehicle(workOrder.CustomerId, workOrder.VehicleId , ct))
            {
                return ApplicationErrors.CustomerDoesNotOwnVehicle;
            }

            if (await _workOrderRules.IsVehicleOccupied(request.VehicleId, workOrder.StartTime, workOrder.EndTime , ct))
            {
                return ApplicationErrors.VehicleHasSchedulingConflict;
            }

            var updateResult = workOrder.UpdateVehicle(request.VehicleId);

            if (!updateResult.IsSuccess)
            {
                return updateResult.Errors;
            }

            await context.SaveChangesAsync(ct);

            await cache.RemoveByTagAsync("work-orders", ct);

            return Result.Updated;
        }
    }
}
