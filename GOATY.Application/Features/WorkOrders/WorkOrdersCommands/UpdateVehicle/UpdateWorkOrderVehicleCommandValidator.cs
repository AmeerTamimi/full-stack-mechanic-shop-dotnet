using FluentValidation;

namespace GOATY.Application.Features.WorkOrders.WorkOrdersCommands.UpdateVehicle
{
    public sealed class UpdateWorkOrderVehicleCommandValidator : AbstractValidator<UpdateWorkOrderVehicleCommand>
    {
        public UpdateWorkOrderVehicleCommandValidator()
        {
            RuleFor(x => x.WorkOrderId)
                .NotEmpty().WithMessage("WorkOrderId is required.");

            RuleFor(x => x.VehicleId)
                .NotEmpty().WithMessage("VehicleId is required.");
        }
    }
}
