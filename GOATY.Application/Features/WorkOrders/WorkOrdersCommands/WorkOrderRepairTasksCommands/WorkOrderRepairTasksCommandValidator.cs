using FluentValidation;

namespace GOATY.Application.Features.WorkOrders.WorkOrdersCommands.WorkOrderRepairTasksCommands
{
    public sealed class WorkOrderRepairTasksCommandValidator : AbstractValidator<WorkOrderRepairTasksCommand>
    {
        public WorkOrderRepairTasksCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("RepairTaskId is required.");
        }
    }
}
