using GOATY.Application.Features.Common.Interfaces;
using GOATY.Application.Features.RepairTasks.DTOs;
using GOATY.Application.Features.RepairTasks.Mapping;
using GOATY.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GOATY.Application.Features.RepairTasks.RepairTaskQueries.GetRepairTasksQuery
{
    public sealed class RepairTaskQueryHandler(IAppDbContext context)
        : IRequestHandler<GetRepairTaskQuery, Result<List<RepairTaskDto>>>
    {
        public async Task<Result<List<RepairTaskDto>>> Handle(GetRepairTaskQuery request, CancellationToken ct)
        {
            var repairTasks = await context.RepairTasks
                                           .AsNoTracking()
                                           .Include(r => r.RepairTaskDetails)
                                           .Where(r => r.IsDeleted == false)
                                           .ToListAsync(ct);

            return repairTasks.ToDtos();
        }
    }
}
