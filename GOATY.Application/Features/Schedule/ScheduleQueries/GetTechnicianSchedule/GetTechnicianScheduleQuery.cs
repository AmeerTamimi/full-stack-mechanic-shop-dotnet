using GOATY.Application.Common.Interfaces;
using GOATY.Application.Features.WorkOrders.DTOs;
using GOATY.Domain.Common.Results;

namespace GOATY.Application.Features.Schedule.ScheduleQueries.GetTechnicianSchedule
{
    public sealed record class GetTechnicianScheduleQuery(Guid EmployeeId , DateOnly Day = default)
        : ICachedQuery<Result<List<WorkOrderDto>>>
    {
        public string CacheKey => $"tech-schedule_{Day}";

        public string[] Tags => ["tech-schedule"];

        public TimeSpan Expiration => TimeSpan.FromMinutes(10);
    }
}
