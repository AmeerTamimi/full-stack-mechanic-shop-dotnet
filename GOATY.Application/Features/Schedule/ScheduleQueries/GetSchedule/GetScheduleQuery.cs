using GOATY.Application.Common.Interfaces;
using GOATY.Application.Features.WorkOrders.DTOs;
using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Features.Schedule.ScheduleQueries.GetSchedule
{
    public sealed record class GetScheduleQuery(DateOnly Day = default)
        : ICachedQuery<Result<List<WorkOrderDto>>>
    {
        public string CacheKey => $"schedules_{Day:yyyy-MM-dd}";

        public string[] Tags => ["schedules"];

        public TimeSpan Expiration => TimeSpan.FromMinutes(10);
    }
}
