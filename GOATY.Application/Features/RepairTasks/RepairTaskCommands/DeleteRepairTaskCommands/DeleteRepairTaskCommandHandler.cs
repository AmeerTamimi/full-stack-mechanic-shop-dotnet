using GOATY.Application.Features.Common.Interfaces;
using GOATY.Application.Features.RepairTasks.DTOs;
using GOATY.Application.Features.RepairTasks.Mapping;
using GOATY.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GOATY.Application.Features.RepairTasks.RepairTaskCommands.DeleteRepairTaskCommands
{
    public sealed class DeleteRepairTaskCommandHandler(
        IAppDbContext context,
        ILogger<DeleteRepairTaskCommandHandler> logger)
        : IRequestHandler<DeleteRepairTaskCommand, Result<RepairTaskDto>>
    {

        public async Task<Result<RepairTaskDto>> Handle(DeleteRepairTaskCommand request, CancellationToken ct)
        {
            var repairTask = await context.RepairTasks.SingleOrDefaultAsync(r => r.Id == request.Id,
                                                                            ct);

            if (repairTask is null)
            {
                return Error.NotFound(
                    code: "RepairTask_NotFound",
                    description: $"RepairTask With Id {request.Id} was Not Found"
                );
            }
            // soft delete for this (from prd) :
            // "Deleting a task template does not affect existing work orders that reference it"
            repairTask.IsDeleted = true;

            await context.SaveChangesAsync(ct);

            return repairTask.ToDto();
        }
    }
}

