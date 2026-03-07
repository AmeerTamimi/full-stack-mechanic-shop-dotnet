using GOATY.Application.Common.Interfaces;
using GOATY.Application.Common.Models;
using GOATY.Application.Features.RepairTasks.DTOs;
using GOATY.Application.Features.RepairTasks.Mapping;
using GOATY.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GOATY.Application.Features.RepairTasks.RepairTaskQueries.GetRepairTasksQuery
{
    public sealed class RepairTaskQueryHandler(IAppDbContext context)
        : IRequestHandler<GetRepairTaskQuery, Result<PaginatedList<RepairTaskDto>>>
    {
        public async Task<Result<PaginatedList<RepairTaskDto>>> Handle(GetRepairTaskQuery request, CancellationToken ct)
        {
            var repairTasksQuery = context.RepairTasks
                                          .AsNoTracking()
                                          .Include(r => r.RepairTaskDetails)
                                          .Where(r => r.IsDeleted == false)
                                          .AsQueryable();

            var count = await repairTasksQuery.CountAsync(ct);

            var page = Math.Max(1, request.Page);
            var pageSize = Math.Clamp(request.PageSize, 1, 100);

            var repairTasks = await repairTasksQuery
                                           .Skip((page - 1) * pageSize)
                                           .Take(pageSize)
                                           .ToListAsync(ct);

            return new PaginatedList<RepairTaskDto>
            {
                Items = repairTasks.ToDtos(),
                Page = page,
                PageSize = pageSize,
                TotalItems = count,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize)
            };
        }
    }
}