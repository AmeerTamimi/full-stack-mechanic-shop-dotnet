using FluentValidation;

namespace GOATY.Application.Features.WorkOrders.WorkOrdersCommands.AssignTechnician
{
    public sealed class AssignTechnicianCommandValidator : AbstractValidator<AssignTechnicianCommand>
    {
        public AssignTechnicianCommandValidator()
        {
            RuleFor(x => x.WorkOrderId)
                .NotEmpty().WithMessage("WorkOrderId is required.");

            RuleFor(x => x.EmployeeId)
                .NotEmpty().WithMessage("EmployeeId is required.");
        }
    }
}
