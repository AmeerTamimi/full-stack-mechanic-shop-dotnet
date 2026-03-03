using GOATY.Application.Features.WorkOrders.DTOs;
using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Features.WorkOrders.WorkOrdersCommands.DeleteWorkOrder
{
    public sealed record class DeleteWorkOrderCommand(Guid Id) 
        : IRequest<Result<WorkOrderDto>>;
}
