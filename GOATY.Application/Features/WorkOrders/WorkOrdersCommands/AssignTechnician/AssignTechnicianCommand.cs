using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Features.WorkOrders.WorkOrdersCommands.AssignTechnician
{
    public sealed record class AssignTechnicianCommand(
        Guid WorkOrderId ,
        Guid EmployeeId)
        : IRequest<Result<Updated>>;
}
