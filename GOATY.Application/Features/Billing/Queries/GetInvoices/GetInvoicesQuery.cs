using GOATY.Application.Common.Interfaces;
using GOATY.Application.Common.Models;
using GOATY.Application.Features.Billing.DTOs;
using GOATY.Domain.Common.Results;

namespace GOATY.Application.Features.Billing.Queries.GetInvoices
{
    public sealed record class GetInvoicesQuery(int Page, int PageSize)
        : ICachedQuery<Result<PaginatedList<InvoiceDto>>>
    {
        public string CacheKey =>
            $"Invoices:" +
            $"p={Page}" +
            $"ps={PageSize}";

        public string[] Tags => ["invoices"];

        public TimeSpan Expiration => TimeSpan.FromMinutes(10);
    }
}
