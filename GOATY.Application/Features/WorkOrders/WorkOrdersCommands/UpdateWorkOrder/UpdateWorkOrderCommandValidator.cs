using FluentValidation;
using GOATY.Application.Features.WorkOrders.WorkOrdersCommands.WorkOrderRepairTasksCommands;

namespace GOATY.Application.Features.WorkOrders.WorkOrdersCommands.UpdateWorkOrder
{
    public sealed class UpdateWorkOrderCommandValidator : AbstractValidator<UpdateWorkOrderCommand>
    {
        public UpdateWorkOrderCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("WorkOrderId is required.");
               
            RuleFor(x => x.VehicleId)
                .NotEmpty().WithMessage("VehicleId is required.");

            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("CustomerId is required.");

            RuleFor(x => x.EmployeeId)
                .NotEmpty().WithMessage("EmployeeId is required.");

            RuleFor(x => x.StartTime)
                .NotEmpty().WithMessage("StartTime is required.");

            RuleFor(x => x.Bay)
                .NotEmpty().WithMessage("Bay is required")
                .IsInEnum().WithMessage("Invalid Bay");

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
