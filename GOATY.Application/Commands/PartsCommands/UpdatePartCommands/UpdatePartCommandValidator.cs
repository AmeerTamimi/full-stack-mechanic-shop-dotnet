using FluentValidation;
using GOATY.Application.DTOs;

namespace GOATY.Application.Commands.PartsCommands.UpdatePartCommands
{
    public sealed class UpdatePartCommandValidator : AbstractValidator<UpdatePartCommand>
    {
        public UpdatePartCommandValidator()
        {
            RuleFor(x => x.name)
                .NotEmpty().WithMessage("Name is required.")
                .MinimumLength(2).WithMessage("Name must be at least 2 chars.")
                .MaximumLength(100).WithMessage("Name must be at most 100 chars.");

            RuleFor(x => x.cost)
                .GreaterThan(0m).WithMessage("Cost must be > 0.")
                .LessThanOrEqualTo(1_000_000m).WithMessage("Cost is too large.");

            RuleFor(x => x.quantity)
                .GreaterThanOrEqualTo(0).WithMessage("Quantity cannot be negative.")
                .LessThanOrEqualTo(1_000_000).WithMessage("Quantity is too large.");

            RuleFor(x => x)
                .Must(x => !(x.cost == 0m && x.quantity > 0))
                .WithMessage("Cost cannot be 0 when quantity is positive.");
        }
    }
}
