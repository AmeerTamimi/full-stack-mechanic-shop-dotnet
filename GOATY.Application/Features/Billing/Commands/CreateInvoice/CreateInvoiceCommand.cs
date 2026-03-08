using GOATY.Application.Features.Billing.DTOs;
using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Features.Billing.Commands.CreateInvoice
{
    public sealed  record class CreateInvoiceCommand(Guid WorkOrderId)
        : IRequest<Result<InvoiceDto>>;
}
