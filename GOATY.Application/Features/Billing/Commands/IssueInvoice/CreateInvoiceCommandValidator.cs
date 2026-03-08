using FluentValidation;

namespace GOATY.Application.Features.Billing.Commands.IssueInvoice
{
    public sealed class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
    {
        public CreateInvoiceCommandValidator()
        {
            RuleFor(x => x.WorkOrderId)
                .NotEmpty().WithMessage("Work Order Id cant be empty");
        }
    }
}
