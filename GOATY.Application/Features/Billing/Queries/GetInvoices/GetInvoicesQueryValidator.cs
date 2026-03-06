using FluentValidation;

namespace GOATY.Application.Features.Billing.Queries.GetInvoices
{
    public sealed class GetInvoicesQueryValidator : AbstractValidator<GetInvoicesQuery>
    {
        public GetInvoicesQueryValidator()
        {
            RuleFor(x => x.Page)
                .NotEmpty().WithMessage("Page Cant be Empty")
                .GreaterThanOrEqualTo(1).WithMessage("Page must have a positive value.");

            RuleFor(x => x.PageSize)
                .NotEmpty().WithMessage("PageSize Cant be Empty")
                .GreaterThanOrEqualTo(1).WithMessage("PageSize must have a positive value.");

        }
    }
}
