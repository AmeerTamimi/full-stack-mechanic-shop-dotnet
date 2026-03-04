using FluentValidation;

namespace GOATY.Application.Features.WorkOrders.WorkOrdersCommands.RelocateWorkOrder
{
    public sealed class RelocateWorkOrderCommandValidator : AbstractValidator<RelocateWorkOrderCommand>
    {
        public RelocateWorkOrderCommandValidator()
        {
            RuleFor(x => x.WorkOrderId)
                .NotEmpty().WithMessage("WorkOrderId is required.");

            RuleFor(x => x.NewStartTime)
                .NotEmpty().WithMessage("NewStartTime is required.");

            RuleFor(x => x.NewBay)
                .NotEmpty().WithMessage("NewBay is required")
                .IsInEnum().WithMessage("Invalid Bay");
        }
    }
}
