using GOATY.Application.Common;
using GOATY.Application.Common.Interfaces;
using GOATY.Domain.Common.Results;
using GOATY.Domain.WorkOrders.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace GOATY.Application.Features.WorkOrders.WorkOrdersCommands.UpdateWorkOrderState
{
    public sealed class UpdateWorkOrderStateCommandHandler(
        IAppDbContext context,
        ILogger<UpdateWorkOrderStateCommandHandler> logger,
        HybridCache cache)
        : IRequestHandler<UpdateWorkOrderStateCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(UpdateWorkOrderStateCommand request, CancellationToken ct)
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

            var newState = request.State;

            if(DateTime.Now < workOrder.StartTime)
            {
                return ApplicationErrors.CannotChangeWorkOrderStateBeforeStartTime;
            }

            var updateResult = workOrder.UpdateState(newState);

            if (!updateResult.IsSuccess)
            {
                return updateResult.Errors;
            }

            await context.SaveChangesAsync(ct);

            await cache.RemoveByTagAsync("work-orders");

            return Result.Updated;
        }
    }
}
