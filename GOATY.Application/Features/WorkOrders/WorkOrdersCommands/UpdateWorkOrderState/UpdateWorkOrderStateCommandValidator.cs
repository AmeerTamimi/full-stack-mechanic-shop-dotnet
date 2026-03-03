using FluentValidation;

namespace GOATY.Application.Features.WorkOrders.WorkOrdersCommands.UpdateWorkOrderState
{
    public sealed class UpdateWorkOrderStateCommandValidator : AbstractValidator<UpdateWorkOrderStateCommand>
    {
        public UpdateWorkOrderStateCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("WorkOrderId is required.");

            RuleFor(x => x.State)
                .IsInEnum().WithMessage("Invalid State.");
        }
    }
}
