using GOATY.Application.Common.Interfaces;
using GOATY.Application.Features.Billing.DTOs;
using GOATY.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GOATY.Application.Features.Billing.Queries.GeneratePdf
{
    public sealed class GeneratePdfQueryHandler(
        IAppDbContext context,
        ILogger<GeneratePdfQueryHandler> logger,
        IInvoicePdfGenerator invoiceGenerator)
        : IRequestHandler<GeneratePdfQuery, Result<InvoicePdfDto>>
    {
        public async Task<Result<InvoicePdfDto>> Handle(GeneratePdfQuery request, CancellationToken ct)
        {
            var invoice = await context.Invoices
                                       .AsNoTracking()
                                       .Include(i => i.InvoiceItems)
                                       .SingleOrDefaultAsync(i => i.Id == request.InvoiceId, ct);

            if (invoice is null)
            {
                logger.LogWarning("Invoice not found. InvoiceId: {InvoiceId}", request.InvoiceId);
                return Error.NotFound(
                    code: "Invoice_NotFound",
                    description: $"Invoice With Id {request.InvoiceId} was Not Found"
                );
            }

            try
            {
                var content = invoiceGenerator.Generate(invoice);

                return new InvoicePdfDto
                {
                    Content = content,
                    FileName = $"Invoice_{request.InvoiceId}.pdf"
                };

            }catch(Exception ex)
            {
                logger.LogError(ex, "Failed to generate PDF for InvoiceId: {InvoiceId}", request.InvoiceId);
                return Error.Failure("An error occurred while generating the invoice PDF.");
            }
        }
    }
}
