using FluentValidation;

namespace GOATY.Application.Features.Billing.Commands.RefundInvoice
{
    public sealed class RefundInvoiceCommandValidator : AbstractValidator<RefundInvoiceCommand>
    {
        public RefundInvoiceCommandValidator()
        {
            RuleFor(x => x.InvoiceId)
                .NotEmpty().WithMessage("Invoice Id Cant Be Empty.");
        }
    }
}