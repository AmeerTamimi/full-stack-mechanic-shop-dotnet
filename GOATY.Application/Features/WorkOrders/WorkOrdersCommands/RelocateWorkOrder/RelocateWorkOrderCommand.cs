using GOATY.Domain.Common.Results;
using GOATY.Domain.WorkOrders.Enums;
using MediatR;

namespace GOATY.Application.Features.WorkOrders.WorkOrdersCommands.RelocateWorkOrder
{
    public sealed record class RelocateWorkOrderCommand(
        Guid WorkOrderId,
        DateTime NewStartTime,
        Bay NewBay)
        : IRequest<Result<Updated>>;
}
