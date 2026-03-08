using GOATY.Application.Features.Billing.DTOs;
using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Features.Billing.Queries.GeneratePdf
{
    public sealed record class GeneratePdfQuery(Guid InvoiceId)
        : IRequest<Result<InvoicePdfDto>>;
}
