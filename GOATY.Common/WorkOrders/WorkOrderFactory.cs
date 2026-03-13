using GOATY.Domain.Common.Results;
using GOATY.Domain.WorkOrders;
using GOATY.Domain.WorkOrders.Enums;

namespace GOATY.Tests.Common.WorkOrders
{
    public static class WorkOrderFactory
    {
        public static Result<WorkOrder> Create(Guid? id = null,
                                               Guid? vehicleId = null,
                                               Guid? customerId = null,
                                               Guid? employeeId = null,
                                               Bay? bay = null,
                                               DateTime? startTime = null,
                                               decimal? discount = null,
                                               List<WorkOrderRepairTasks>? repairTasks = null)
        {
            return WorkOrder.Create(
                id ?? Guid.NewGuid(),
                vehicleId ?? Guid.NewGuid(),
                customerId ?? Guid.NewGuid(),
                employeeId ?? Guid.NewGuid(),
                startTime ?? DateTime.UtcNow.AddMinutes(5),
                bay ?? Bay.A,
                discount ?? 0m,
                repairTasks ?? [WorkOrderRepairTaskFactory.Create().Value]);
        }
    }
}
