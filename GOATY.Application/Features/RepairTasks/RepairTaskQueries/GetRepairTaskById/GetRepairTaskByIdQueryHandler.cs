using GOATY.Application.Common.Interfaces;
using GOATY.Application.Features.RepairTasks.DTOs;
using GOATY.Application.Features.RepairTasks.Mapping;
using GOATY.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GOATY.Application.Features.RepairTasks.RepairTaskQueries.GetRepairTaskById
{
    public sealed class GetRepairTaskByIdQueryHandler(IAppDbContext context)
        : IRequestHandler<GetRepairTaskByIdQuery, Result<RepairTaskDto>>
    {
        public async Task<Result<RepairTaskDto>> Handle(GetRepairTaskByIdQuery request, CancellationToken ct)
        {
            var repairTask = await context.RepairTasks
                                          .AsNoTracking()
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

            return repairTask.ToDto();
        }
    }
}
