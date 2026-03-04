using FluentValidation;
using GOATY.Application.Features.WorkOrders.WorkOrdersCommands.WorkOrderRepairTasksCommands;

namespace GOATY.Application.Features.WorkOrders.WorkOrdersCommands.UpdateWorkOrderRepairTasks
{
    public sealed class UpdateWorkOrderRepairTasksCommandValidator : AbstractValidator<UpdateWorkOrderRepairTasksCommand>
    {
        public UpdateWorkOrderRepairTasksCommandValidator()
        {
            RuleFor(x => x.WorkOrderId)
                .NotEmpty().WithMessage("WorkOrderId is required.");

            RuleFor(x => x.WorkOrderRepairTasks)
                .NotNull().WithMessage("WorkOrderRepairTasks is required.")
                .NotEmpty().WithMessage("WorkOrderRepairTasks must contain at least one item.");

            RuleForEach(x => x.WorkOrderRepairTasks)
                .SetValidator(new WorkOrderRepairTasksCommandValidator());

            RuleFor(x => x.WorkOrderRepairTasks)
                .Must(list => list.Select(t => t.Id).Distinct().Count() == list.Count)
                .WithMessage("WorkOrderRepairTasks contains duplicate RepairTaskId.");
        }
    }
}
