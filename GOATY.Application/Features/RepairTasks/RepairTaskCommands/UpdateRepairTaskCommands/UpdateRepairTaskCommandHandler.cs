using GOATY.Application.Common.Interfaces;
using GOATY.Domain.Common.Results;
using GOATY.Domain.RepairTasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace GOATY.Application.Features.RepairTasks.RepairTaskCommands.UpdateRepairTaskCommands
{
    public sealed class UpdateRepairTaskCommandHandler(
        IAppDbContext context,
        ILogger<UpdateRepairTaskCommandHandler> logger,
        HybridCache cache)
        : IRequestHandler<UpdateRepairTaskCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(UpdateRepairTaskCommand request, CancellationToken ct)
        {
            var repairTask = await context.RepairTasks
                                          .Include(r => r.RepairTaskDetails)
                                          .SingleOrDefaultAsync(
                                           r => r.IsDeleted == false &&
                                           r.Id == request.Id,
                                           ct);

            if(repairTask is null)
            {
                return Error.NotFound(
                    code: "RepairTask_NotFound",
                    description: $"RepairTask With Id {request.Id} was Not Found"
                );
            }

            if (await context.RepairTasks.AnyAsync(r => (r.Name == request.Name && r.Id != request.Id) , ct))
            {
                return Error.Conflict(code: "RepairTask.Name.Conflict",
                                      description: $"The Repair Task Name {request.Name} Already Exists.");
            }

            var repairTaskDetailsList = new List<RepairTaskDetails>();

            var partIds = request.Parts.Select(p => p.Id);

            var partsInDb = await context.Parts
                                         .Where(p => partIds.Contains(p.Id))
                                         .ToListAsync(ct);

            var parts = request.Parts;

            foreach (var part in parts)
            {
                var partId = part.Id;

                var partModel = partsInDb.SingleOrDefault(p => p.Id == partId);

                if (partModel is null)
                {
                    return Error.NotFound(
                        code: "Part_NotFound",
                        description: $"Part With Id {partId} was Not Found"
                    );
                }

                var repairTaskDetails = RepairTaskDetails.Create(repairTask.Id,
                                                                 partId,
                                                                 part.Quantity,
                                                                 partModel.Cost);

                if (!repairTaskDetails.IsSuccess)
                {
                    return repairTaskDetails.Errors;
                }

                repairTaskDetailsList.Add(repairTaskDetails.Value);
            }

            var repairTaskUpdateResult = repairTask.Update(request.Name,
                                                           request.Description,
                                                           request.TimeEstimated,
                                                           request.CostEstimated,
                                                           request.TechnicianCost);

            if (!repairTaskUpdateResult.IsSuccess)
            {
                return repairTaskUpdateResult.Errors;
            }

            var repairTaskDetailsUpsertResult = repairTask.UpsertRepairTaskDetails(repairTaskDetailsList);

            if (!repairTaskDetailsUpsertResult.IsSuccess)
            {
                return repairTaskDetailsUpsertResult.Errors;
            }

            await context.SaveChangesAsync(ct);

            await cache.RemoveByTagAsync("repair-tasks" , ct);

            return Result.Updated;
        }
    }
}
