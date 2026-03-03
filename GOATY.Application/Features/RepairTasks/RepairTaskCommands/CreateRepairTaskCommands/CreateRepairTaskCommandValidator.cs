using FluentValidation;

namespace GOATY.Application.Features.RepairTasks.RepairTaskCommands.CreateRepairTaskCommands
{
    public sealed class CreateRepairTaskCommandValidator : AbstractValidator<CreateRepairTaskCommand>
    {
        public CreateRepairTaskCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must be 100 characters or less.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(1000).WithMessage("Description must be 1000 characters or less.");

            RuleFor(x => x.TimeEstimated)
                .NotEmpty().WithMessage("Time Estimated Is Required.")
                .IsInEnum().WithMessage("Invalid Time Duration.");

            RuleFor(x => x.CostEstimated)
                .GreaterThanOrEqualTo(50).WithMessage("CostEstimated must be 50 or more.");

            RuleFor(x => x.Parts)
                .NotNull().WithMessage("Parts is required.")
                .NotEmpty().WithMessage("Parts must contain at least one item.");

            RuleForEach(x => x.Parts)
                .SetValidator(new PartsValidator());

            RuleFor(x => x.Parts)
                .Must(parts => parts.Select(p => p.Id).Distinct().Count() == parts.Count)
                .WithMessage("Parts contains duplicate PartId.");
        }
    }
}
