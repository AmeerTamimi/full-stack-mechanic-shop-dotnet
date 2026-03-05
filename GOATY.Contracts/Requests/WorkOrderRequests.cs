using GOATY.Domain.Common.Enums;
using GOATY.Domain.WorkOrders.Enums;

namespace GOATY.Contracts.Requests
{
    public sealed record class CreateWorkOrderRequest(
            Guid VehicleId,
            Guid CustomerId,
            Guid EmployeeId,
            DateTime StartTime,
            Bay Bay,
            IReadOnlyList<WorkOrderRepairTaskRequest> WorkOrderRepairTasks);

    public sealed record class WorkOrderRepairTaskRequest(Guid RepairTaskId);

    public sealed record class AssignTechnicianRequest(Guid EmployeeId);
    public sealed record class RelocateWorkOrderRequest(Bay Bay , DateTime StartTime);
    public sealed record class UpdateWorkOrderVehicleRequest(Guid VehicleId);
    public sealed record class UpdateWorkOrderStateRequest(State State);
    public sealed record class UpdateWorkOrderRepairTasksRequest(IReadOnlyList<WorkOrderRepairTaskRequest> WorkOrderRepairTasks);
    public sealed record class GetScheduleRequest(DateOnly Day = default);
    public sealed record class GetTechnicianScheduleRequest(Guid EmployeeId , DateOnly Day = default);



}
