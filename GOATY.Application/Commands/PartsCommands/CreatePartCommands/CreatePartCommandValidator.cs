using FluentValidation;
using GOATY.Application.DTOs;

namespace GOATY.Application.Commands.PartsCommands.CreatePartCommands
{
    public sealed class CreatePartCommandValidator : AbstractValidator<CreatePartCommand>
    {
        public CreatePartCommandValidator()
        {
            RuleFor(x => x.name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(80).WithMessage("Name can't be longer than 80 characters.");

            RuleFor(x => x.cost)
                .NotEmpty().WithMessage("Hello")
                .GreaterThan(0m).WithMessage("Cost must be greater than 0.")
                .LessThanOrEqualTo(1_000_000m).WithMessage("Cost is too large.");

            RuleFor(x => x.quantity)
                .GreaterThanOrEqualTo(0).WithMessage("Quantity can't be negative.")
                .LessThanOrEqualTo(10_000).WithMessage("Quantity is too large.");
        }
    }
}
