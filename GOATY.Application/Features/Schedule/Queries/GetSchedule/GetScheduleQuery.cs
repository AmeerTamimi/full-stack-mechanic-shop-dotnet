using GOATY.Application.Common.Interfaces;
using GOATY.Application.Features.Schedule.DTOs;
using GOATY.Domain.Common.Results;

namespace GOATY.Application.Features.Schedule.Queries.GetSchedule
{
    public sealed record class GetScheduleQuery(
        DateOnly Day,
        TimeZoneInfo TimeZone,
        Guid? EmployeeID = null) 
        : ICachedQuery<Result<ScheduleDto>>

    {
        public string CacheKey => $"schedules_{Day}{EmployeeID}";

        public string[] Tags => [$"schedules"];

        public TimeSpan Expiration => TimeSpan.FromMinutes(10);
    }
}
