using GOATY.Application.Common.Interfaces;
using GOATY.Application.Features.Billing.DTOs;
using GOATY.Application.Features.Billing.Mappers;
using GOATY.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GOATY.Application.Features.Billing.Queries.GetInvoiceById
{
    public sealed class GetInvoiceByIdQueryHandler(IAppDbContext context, ILogger<GetInvoiceByIdQueryHandler> logger)
        : IRequestHandler<GetInvoiceByIdQuery, Result<InvoiceDto>>
    {
        public async Task<Result<InvoiceDto>> Handle(GetInvoiceByIdQuery request, CancellationToken ct)
        {
            var invoice = await context.Invoices
                                       .AsNoTracking()
                                       .Include(i => i.InvoiceItems)
                                       .SingleOrDefaultAsync(i => i.Id == request.Id , ct);

            if (invoice is null)
            {
                return Error.NotFound( 
                    code: "IKnvoice_NotFound",
                    description: $"Invoice With Id {request.Id} was Not Found"
                );
            }

            return invoice.ToDto();
        }
    }
}
