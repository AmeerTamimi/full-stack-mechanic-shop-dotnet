using GOATY.Application.Common.Interfaces;
using GOATY.Application.Features.WorkOrders.DTOs;
using GOATY.Domain.Common.Results;

namespace GOATY.Application.Features.WorkOrders.WorkOrdersQueries.GetWorkOrders
{
    public sealed record class GetWorkOrdersQuery : ICachedQuery<Result<List<WorkOrderDto>>>
    {
        public string CacheKey => "work-orders";

        public string[] Tags => ["work-orders"];

        public TimeSpan Expiration => TimeSpan.FromMinutes(10);
    }
}
