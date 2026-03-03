using FluentValidation;

namespace GOATY.Application.Features.WorkOrders.WorkOrdersCommands.CreateWorkOrder
{
    public sealed class CreateWorkOrderCommandValidator : AbstractValidator<CreateWorkOrderCommand>
    {
        public CreateWorkOrderCommandValidator()
        {

            RuleFor(x => x.VehicleId)
                .NotEmpty().WithMessage("VehicleId is required.");

            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("CustomerId is required.");

            RuleFor(x => x.EmployeeId)
                .NotEmpty().WithMessage("EmployeeId is required.");

            RuleFor(x => x.StartTime)
                .NotEmpty().WithMessage("StartTime is required.");

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
