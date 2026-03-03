using GOATY.Application.Common.Interfaces;
using GOATY.Application.Features.WorkOrders.DTOs;
using GOATY.Domain.Common.Results;

namespace GOATY.Application.Features.WorkOrders.WorkOrdersQueries.GetWorkOrderById
{
    public sealed record class GetWorkOrderByIdQuery(Guid Id) : ICachedQuery<Result<WorkOrderDto>>
    {
        public string CacheKey => $"work-orders_{Id}";

        public string[] Tags => ["work-orders"];

        public TimeSpan Expiration => TimeSpan.FromMinutes(10);
    }
}