using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Features.WorkOrders.WorkOrdersCommands.UpdateVehicle
{
    public sealed record class UpdateWorkOrderVehicleCommand(
        Guid WorkOrderId,
        Guid VehicleId)
        : IRequest<Result<Updated>>;
}
