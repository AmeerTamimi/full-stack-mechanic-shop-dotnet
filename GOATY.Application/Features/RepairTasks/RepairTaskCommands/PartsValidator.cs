using FluentValidation;
using GOATY.Application.Features.RepairTasks.RepairTaskCommands.CreateRepairTaskCommands;

namespace GOATY.Application.Features.RepairTasks.RepairTaskCommands
{
    public sealed class PartsValidator : AbstractValidator<PartRequirements>
    {
        public PartsValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("PartId is required.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0.");
        }
    }
}
