using FluentValidation;

namespace GOATY.Application.Features.Billing.Queries.GeneratePdf
{
    public sealed class GeneratePdfQueryValidator : AbstractValidator<GeneratePdfQuery>
    {
        public GeneratePdfQueryValidator()
        {
            RuleFor(x => x.InvoiceId)
                .NotEmpty().WithMessage("Invoice Id Cant be Empty");
        }
    }
}