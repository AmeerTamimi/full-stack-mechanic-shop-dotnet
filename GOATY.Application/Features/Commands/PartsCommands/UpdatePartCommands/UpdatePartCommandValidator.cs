using FluentValidation;

namespace GOATY.Application.Features.Commands.PartsCommands.UpdatePartCommands
{
    public sealed class UpdatePartCommandValidator : AbstractValidator<UpdatePartCommand>
    {
        public UpdatePartCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Employee Id Cant Be Empty.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MinimumLength(2).WithMessage("Name must be at least 2 chars.")
                .MaximumLength(100).WithMessage("Name must be at most 100 chars.");

            RuleFor(x => x.Cost)
                .GreaterThan(0m).WithMessage("Cost must be > 0.")
                .LessThanOrEqualTo(1_000_000m).WithMessage("Cost is too large.");

            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(1).WithMessage("Quantity cannot be negative.")
                .LessThanOrEqualTo(1_000_000).WithMessage("Quantity is too large.");
        }
    }
}
