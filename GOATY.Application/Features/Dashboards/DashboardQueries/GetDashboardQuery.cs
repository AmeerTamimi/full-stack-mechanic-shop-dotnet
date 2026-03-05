using GOATY.Application.Common.Interfaces;
using GOATY.Application.Features.Dashboards.DTOs;
using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Features.Dashboards.DashboardQueries
{
    public sealed record class GetDashboardQuery(
        DateOnly? Day,
        DateTimeOffset? StartDate,
        DateTimeOffset? EndDate)
        : ICachedQuery<Result<Dashboard>>
    {
        public string CacheKey =>
            $"dashboard:" +
            $"d={Day.ToString() ?? "-"}" +
            $"sd={StartDate.ToString() ?? "-"}" +
            $"ed={EndDate.ToString() ?? "-"}";

        public string[] Tags => ["dashboard"];

        public TimeSpan Expiration => TimeSpan.FromMinutes(15);
    }
}
