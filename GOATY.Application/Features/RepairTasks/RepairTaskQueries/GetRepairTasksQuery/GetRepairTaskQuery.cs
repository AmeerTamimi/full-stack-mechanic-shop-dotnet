using GOATY.Application.Common.Interfaces;
using GOATY.Application.Common.Models;
using GOATY.Application.Features.RepairTasks.DTOs;
using GOATY.Domain.Common.Results;

namespace GOATY.Application.Features.RepairTasks.RepairTaskQueries.GetRepairTasksQuery
{
    public sealed record class GetRepairTaskQuery(int Page, int PageSize)
        : ICachedQuery<Result<PaginatedList<RepairTaskDto>>>
    {
        public string CacheKey =>
            $"repair-tasks:" +
            $"p={Page}:" +
            $"ps={PageSize}";

        public string[] Tags => ["repair-tasks"];

        public TimeSpan Expiration => TimeSpan.FromMinutes(10);
    }
}