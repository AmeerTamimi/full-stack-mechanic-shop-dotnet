using GOATY.Application.Common.Interfaces;
using GOATY.Application.Features.RepairTasks.DTOs;
using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Features.RepairTasks.RepairTaskQueries.GetRepairTaskById
{
    public record class GetRepairTaskByIdQuery(Guid Id) : ICachedQuery<Result<RepairTaskDto>>
    {
        public string CacheKey => $"repair-tasks_{Id}";

        public string[] Tags => ["repair-tasks"];

        public TimeSpan Expiration => TimeSpan.FromMinutes(10);
    }
}
