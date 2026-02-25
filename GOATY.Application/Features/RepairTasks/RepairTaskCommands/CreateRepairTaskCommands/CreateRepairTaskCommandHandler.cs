using GOATY.Application.Features.Common.Interfaces;
using GOATY.Application.Features.RepairTasks.DTOs;
using GOATY.Application.Features.RepairTasks.Mapping;
using GOATY.Domain.Common.Results;
using GOATY.Domain.RepairsTask.Parts;
using GOATY.Domain.RepairTasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GOATY.Application.Features.RepairTasks.RepairTaskCommands.CreateRepairTaskCommands
{
    public sealed class CreateRepairTaskCommandHandler(
        IAppDbContext context,
        ILogger<CreateRepairTaskCommandHandler> logger)
        : IRequestHandler<CreateRepairTaskCommand, Result<RepairTaskDto>>
    {

        public async Task<Result<RepairTaskDto>> Handle(CreateRepairTaskCommand request, CancellationToken ct)
        {
            if(await context.RepairTasks.AnyAsync(r => r.Name == request.Name))
            {
                return Error.Conflict(code: "RepairTask.Name.Conflict",
                                      description: $"The Repair Task Name {request.Name} Already Exists.");
            }

            var repairTaskId = Guid.NewGuid();

            var repairTaskDetailsList = new List<RepairTaskDetails>();

            var parts = request.Parts;

            foreach(var part in parts)
            {
                var partId = part.Id;
                var partModel = await context.Parts.SingleOrDefaultAsync(p => p.Id == partId);

                if (partModel is null)
                {
                    return Error.NotFound( 
                        code: "Part_NotFound",
                        description: $"Part With Id {partId} was Not Found"
                    );
                }

                repairTaskDetailsList.Add(new RepairTaskDetails(repairTaskId,
                                                             partId,
                                                             part.Quantity,
                                                             partModel.Cost));
            }

            var repairTaskResult = RepairTask.Create(repairTaskId,
                                                     request.Name,
                                                     request.Description,
                                                     request.TimeEstimated,
                                                     request.CostEstimated,
                                                     repairTaskDetailsList);
            if (!repairTaskResult.IsSuccess)
            {
                return repairTaskResult.Errors;
            }

            var repairTask = repairTaskResult.Value;

            await context.RepairTasks.AddAsync(repairTask, ct);
            await context.SaveChangesAsync(ct);

            return repairTask.ToDto();
        }
    }
}
