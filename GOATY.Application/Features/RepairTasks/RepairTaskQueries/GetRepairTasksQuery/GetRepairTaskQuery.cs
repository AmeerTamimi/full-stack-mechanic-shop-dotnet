using GOATY.Application.Common.Interfaces;
using GOATY.Application.Features.RepairTasks.DTOs;
using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Features.RepairTasks.RepairTaskQueries.GetRepairTasksQuery
{
    public record class GetRepairTaskQuery : ICachedQuery<Result<List<RepairTaskDto>>>
    {
        public string CacheKey => "repair-tasks";

        public string[] Tags => ["repair-tasks"];

        public TimeSpan Expiration => TimeSpan.FromMinutes(10);
    }
}
