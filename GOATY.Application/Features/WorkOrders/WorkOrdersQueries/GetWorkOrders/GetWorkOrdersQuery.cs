using GOATY.Application.Common.Interfaces;
using GOATY.Application.Common.Models;
using GOATY.Application.Features.WorkOrders.DTOs;
using GOATY.Domain.Common.Results;
using GOATY.Domain.WorkOrders.Enums;

namespace GOATY.Application.Features.WorkOrders.WorkOrdersQueries.GetWorkOrders
{
    public sealed record class GetWorkOrdersQuery(
        int Page,
        int PageSize,
        string? SearchTerm,
        string SortColumn = "createdAt",
        string SortDirection = "desc",
        State? State = null,
        Guid? VehicleId = null,
        Guid? LaborId = null,
        DateTime? StartDateFrom = null,
        DateTime? StartDateTo = null,
        DateTime? EndDateFrom = null,
        DateTime? EndDateTo = null,
        Bay? Bay = null)
        : ICachedQuery<Result<PaginatedList<WorkOrderDto>>>
    {
        public string CacheKey =>
            $"work-orders:p={Page}:ps={PageSize}" +
            $":q={SearchTerm ?? "-"}" +
            $":sort={SortColumn}:{SortDirection}" +
            $":state={State?.ToString() ?? "-"}" +
            $":veh={VehicleId?.ToString() ?? "-"}" +
            $":lab={LaborId?.ToString() ?? "-"}" +
            $":sdfrom={StartDateFrom?.ToString("yyyyMMdd") ?? "-"}" +
            $":sdto={StartDateTo?.ToString("yyyyMMdd") ?? "-"}" +
            $":edfrom={EndDateFrom?.ToString("yyyyMMdd") ?? "-"}" +
            $":edto={EndDateTo?.ToString("yyyyMMdd") ?? "-"}" +
            $":spot={Bay?.ToString() ?? "-"}";

        public string[] Tags => ["work-orders"];

        public TimeSpan Expiration => TimeSpan.FromMinutes(10);
    }
}
