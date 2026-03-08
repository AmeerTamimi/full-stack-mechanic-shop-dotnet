using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Features.Billing.Commands.SettleInvoice
{
    public sealed record class SettleInvoiceCommand(Guid InvoiceId)
        : IRequest<Result<Updated>>;
}
