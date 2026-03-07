using GOATY.Application.Common.Interfaces;
using GOATY.Application.Features.Billing.DTOs;
using GOATY.Domain.Common.Results;

namespace GOATY.Application.Features.Billing.Queries.GetInvoiceById
{
    public sealed record class GetInvoiceByIdQuery(Guid Id)
        : ICachedQuery<Result<InvoiceDto>>
    {
        public string CacheKey => $"invoice_{Id}";

        public string[] Tags => ["invoices"];

        public TimeSpan Expiration => TimeSpan.FromMinutes(10);
    }
}
