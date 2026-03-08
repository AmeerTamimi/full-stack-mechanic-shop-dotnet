using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Features.Billing.Commands.RefundInvoice
{
    public sealed record class RefundInvoiceCommand(Guid InvoiceId)
        : IRequest<Result<Updated>>;
}