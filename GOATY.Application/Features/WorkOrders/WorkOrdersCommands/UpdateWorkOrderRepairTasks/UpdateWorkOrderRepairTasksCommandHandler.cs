using GOATY.Application.Common.Interfaces;
using GOATY.Application.Features.WorkOrders.WorkOrdersCommands.AssignTechnician;
using GOATY.Domain.Common.Results;
using GOATY.Domain.WorkOrders;
using GOATY.Domain.WorkOrders.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace GOATY.Application.Features.WorkOrders.WorkOrdersCommands.UpdateWorkOrderRepairTasks
{
    internal class UpdateWorkOrderRepairTasksCommandHandler(
        IAppDbContext context,
        ILogger<UpdateWorkOrderRepairTasksCommandHandler> logger,
        HybridCache cache,
        IWorkOrderRules _workOrderRules)
        : IRequestHandler<UpdateWorkOrderRepairTasksCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(UpdateWorkOrderRepairTasksCommand request, CancellationToken ct)
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
                    repairTask.CostEstimated,
                    1);

                if (!workOrderRepairTaskModel.IsSuccess)
                {
                    return workOrderRepairTaskModel.Errors;
                }

                workOrderRepairTasksModels.Add(workOrderRepairTaskModel.Value);
            }

            var upsertResult = workOrder.UpsertRepairTasks(workOrderRepairTasksModels);

            if (!upsertResult.IsSuccess)
            {
                return upsertResult.Errors;
            }

            await context.SaveChangesAsync(ct);

            await cache.RemoveByTagAsync("work-orders", ct);

            workOrder.AddDomainEvent(new WorkOrderCollectionModifiedDomainEvent());

            return Result.Updated;
        }
    }
}
