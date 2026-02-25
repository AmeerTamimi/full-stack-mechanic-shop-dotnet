using GOATY.Application.Features.Common.Interfaces;
using GOATY.Application.Features.RepairTasks.DTOs;
using GOATY.Application.Features.RepairTasks.Mapping;
using GOATY.Domain.Common.Results;
using GOATY.Domain.RepairsTask.Parts;
using GOATY.Domain.RepairTasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GOATY.Application.Features.RepairTasks.RepairTaskCommands.CreateReapairTaskCommands
{
    public sealed class CreateRepairTaskCommandHandler(
        IAppDbContext context,
        ILogger<CreateRepairTaskCommandHandler> logger)
        : IRequestHandler<CreateRepairTaskCommand, Result<RepairTaskDto>>
    {

        public async Task<Result<RepairTaskDto>> Handle(CreateRepairTaskCommand request, CancellationToken ct)
        {

            //foreach (var details in request.RepairTaskDetails)
            //{
            //    var part = await context.Parts.SingleOrDefaultAsync(p => p.Id == details.PartId,
            //                                                        ct);
            //    if (part is null)
            //    {
            //        return Error.NotFound( 
            //            code: "Part_NotFound",
            //            description: $"Part With Id {details.PartId} was Not Found"
            //        );
            //    }
            //}

            //var repairTask = RepairTask.Create(
            //                            Guid.NewGuid(),
            //                            request.Name,
            //                            request.Description,
            //                            request.TimeEstimated,
            //                            request.CostEstimated,
            //                            request.RepairTaskDetails);

            //if (!repairTask.IsSuccess)
            //{
            //    return repairTask.Errors;
            //}

            //await context.RepairTasks.AddAsync(repairTask.Value , ct);
            //await context.SaveChangesAsync(ct);

            //return repairTask.Value.ToDto();
            return null;
        }
    }
}
