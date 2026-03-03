using GOATY.Domain.Common.Enums;
using GOATY.Domain.WorkOrders.Enums;

namespace GOATY.Contracts.Requests
{
    public sealed record class WorkOrderRequest(
            Guid VehicleId,
            Guid CustomerId,
            Guid EmployeeId,
            DateTime StartTime,
            Bay Bay,
            IReadOnlyList<WorkOrderRepairTaskRequest> WorkOrderRepairTasks);

    public sealed record class WorkOrderRepairTaskRequest(Guid RepairTaskId);
}
