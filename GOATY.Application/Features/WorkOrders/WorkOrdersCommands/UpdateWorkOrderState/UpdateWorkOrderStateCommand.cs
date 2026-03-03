using GOATY.Domain.Common.Results;
using GOATY.Domain.WorkOrders.Enums;
using MediatR;

namespace GOATY.Application.Features.WorkOrders.WorkOrdersCommands.UpdateWorkOrderState
{
    public sealed record class UpdateWorkOrderStateCommand(Guid Id , State State)
        : IRequest<Result<Updated>>;
}
