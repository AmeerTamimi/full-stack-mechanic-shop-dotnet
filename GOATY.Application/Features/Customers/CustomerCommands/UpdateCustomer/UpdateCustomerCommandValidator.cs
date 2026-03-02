using FluentValidation;

namespace GOATY.Application.Features.Customers.CustomerCommands.UpdateCustomer
{
    public sealed class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
    {
        public UpdateCustomerCommandValidator()
        {
            RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name must be at most 50 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name must be at most 50 characters.");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^(?:0|970|972)5(6|9)\d{7}$")
                .WithMessage("Phone must be a valid Palestinian mobile number (e.g., 059XXXXXXXX or 97059XXXXXXXX or 97259XXXXXXXX).");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .MaximumLength(256).WithMessage("Email must be at most 256 characters.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required.")
                .MaximumLength(300).WithMessage("Address must be at most 300 characters.");

            RuleFor(x => x.Vehicles)
                .NotNull().WithMessage("Vehicles is required.")
                .NotEmpty().WithMessage("At least one vehicle is required.");

            RuleForEach(x => x.Vehicles)
                .SetValidator(new VehicleValidator());
        }
    }
}
