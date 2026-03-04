using GOATY.Application.Common;
using GOATY.Application.Common.Interfaces;
using GOATY.Domain.Common.Results;
using GOATY.Domain.WorkOrders.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace GOATY.Application.Features.WorkOrders.WorkOrdersCommands.RelocateWorkOrder
{
    public sealed class RelocateWorkOrderCommandHandler(
        IAppDbContext context,
        ILogger<RelocateWorkOrderCommandHandler> logger,
        HybridCache cache,
        IWorkOrderRules _workOrderRules)
        : IRequestHandler<RelocateWorkOrderCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(RelocateWorkOrderCommand request, CancellationToken ct)
        {
            var workOrder = await context.WorkOrders
                .SingleOrDefaultAsync(wo => wo.Id == request.WorkOrderId, ct);

            if (workOrder is null)
            {
                return Error.NotFound(
                    code: "WorkOrder_NotFound",
                    description: $"WorkOrder With Id {request.WorkOrderId} was Not Found"
                );
            }

            if(await _workOrderRules.IsBayOccupied(request.NewBay , request.NewStartTime , workOrder.EndTime , ct))
            {
                return ApplicationErrors.BayIsOccupied;
            }

            var updateResult = workOrder.Relocate(request.NewBay, request.NewStartTime);

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
