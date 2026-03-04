using FluentValidation;

namespace GOATY.Application.Features.WorkOrders.WorkOrdersCommands.UpdateVehicle
{
    public sealed class UpdateVehicleCommandValidator : AbstractValidator<UpdateVehicleCommand>
    {
        public UpdateVehicleCommandValidator()
        {
            RuleFor(x => x.WorkOrderId)
                .NotEmpty().WithMessage("WorkOrderId is required.");

            RuleFor(x => x.VehicleId)
                .NotEmpty().WithMessage("VehicleId is required.");
        }
    }
}
