using GOATY.Application.Common;
using GOATY.Application.Common.Interfaces;
using GOATY.Domain.Common.Results;
using GOATY.Domain.WorkOrders.Billing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace GOATY.Application.Features.Billing.Commands.RefundInvoice
{
    public sealed class RefundInvoiceCommandHandler(
        IAppDbContext context,
        ILogger<RefundInvoiceCommandHandler> logger,
        HybridCache cache)
        : IRequestHandler<RefundInvoiceCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(RefundInvoiceCommand request, CancellationToken ct)
        {
            var invoice = await context.Invoices
                            .SingleOrDefaultAsync(i => i.Id == request.InvoiceId, ct);

            if (invoice is null)
            {
                return Error.NotFound(
                    code: "Invoice_NotFound",
                    description: $"Invoice With Id {request.InvoiceId} was Not Found"
                );
            }

            if (invoice.Status == InvoiceStatus.NotPayed)
            {
                return ApplicationErrors.InvoiceIsNotPayed;
            }

            if (invoice.Status == InvoiceStatus.Refunded)
            {
                return ApplicationErrors.InvoiceIsRefunded;
            }

            var updateResult = invoice.RefundInvoice();

            if (!updateResult.IsSuccess)
            {
                return updateResult.Errors;
            }

            await context.SaveChangesAsync(ct);

            await cache.RemoveByTagAsync("invocies", ct);

            return Result.Updated;
        }
    }
}