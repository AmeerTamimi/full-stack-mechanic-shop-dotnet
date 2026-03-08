using FluentValidation;

namespace GOATY.Application.Features.Billing.Commands.SettleInvoice
{
    public sealed class SettleInvoiceCommandValidator : AbstractValidator<SettleInvoiceCommand>
    {
        public SettleInvoiceCommandValidator()
        {
            RuleFor(x => x.InvoiceId)
                .NotEmpty().WithMessage("Invoice Id Cant Be Empty.");
        }
    }
}
