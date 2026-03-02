using FluentValidation;

namespace GOATY.Application.Features.Customers.CustomerCommands
{
    public sealed class VehicleValidator : AbstractValidator<VehicleRequirements>
    {
        public VehicleValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("CustomerId is required.");

            RuleFor(x => x.Brand)
                .NotEmpty().WithMessage("Brand is required.")
                .MaximumLength(50).WithMessage("Brand must be at most 50 characters.");

            RuleFor(x => x.Model)
                .NotEmpty().WithMessage("Model is required.")
                .MaximumLength(50).WithMessage("Model must be at most 50 characters.");

            RuleFor(x => x.Year)
                .InclusiveBetween(1900, DateTime.UtcNow.Year)
                .WithMessage($"Year must be between 1900 and {DateTime.UtcNow.Year}.");

            RuleFor(x => x.LicensePlate)
                .NotEmpty().WithMessage("License plate is required.")
                .MaximumLength(20).WithMessage("License plate must be at most 20 characters.");
        }
    }
}
