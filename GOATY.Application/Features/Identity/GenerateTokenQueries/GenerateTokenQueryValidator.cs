using FluentValidation;

namespace GOATY.Application.Features.Identity.GenerateTokenQueries
{
    public sealed class GenerateTokenQueryValidator : AbstractValidator<GenerateTokenQuery>
    {
        public GenerateTokenQueryValidator()
        {
            //RuleFor(x => x.Email)
            //    .NotEmpty().WithMessage("Email is required.")
            //    .EmailAddress().WithMessage("Email is not valid.")
            //    .MaximumLength(256).WithMessage("Email is too long.");

            //RuleFor(x => x.Password)
            //    .NotEmpty().WithMessage("Password is required.")
            //    .MinimumLength(6).WithMessage("Password must be at least 6 characters.")
            //    .MaximumLength(128).WithMessage("Password is too long."); 
        }
    }
}
