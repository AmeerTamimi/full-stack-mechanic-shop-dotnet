using GOATY.Application.Common.Interfaces;
using GOATY.Application.Common.Models;
using GOATY.Application.Features.Billing.DTOs;
using GOATY.Application.Features.Billing.Mappers;
using GOATY.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GOATY.Application.Features.Billing.Queries.GetInvoices
{
    public sealed class GetInvoicesQueryHandler(IAppDbContext context, ILogger<GetInvoicesQueryHandler> logger)
        : IRequestHandler<GetInvoicesQuery, Result<PaginatedList<InvoiceDto>>>
    {
        public async Task<Result<PaginatedList<InvoiceDto>>> Handle(GetInvoicesQuery request, CancellationToken ct)
        {
            var invoicesQuery = context.Invoices
                                  .AsNoTracking()
                                  .Include(i => i.InvoiceItems)
                                  .AsQueryable();

            var count = await invoicesQuery.CountAsync(ct);

            var page = Math.Max(1, request.Page);
            var pageSize = Math.Clamp(request.PageSize, 1, 100);

            var invoices = await invoicesQuery
                                    .Skip((page - 1) * pageSize)
                                    .Take(pageSize)
                                    .Select(i => i)
                                    .ToListAsync(ct);

            return new PaginatedList<InvoiceDto>
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = count,
                Items = invoices.ToDtos()
            };
        }
    }
}
