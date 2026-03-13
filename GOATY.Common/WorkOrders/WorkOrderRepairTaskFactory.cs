using GOATY.Domain.Common.Enums;
using GOATY.Domain.Common.Results;
using GOATY.Domain.WorkOrders;

namespace GOATY.Tests.Common.WorkOrders
{
    public static class WorkOrderRepairTaskFactory
    {
        public static Result<WorkOrderRepairTasks> Create(Guid? workOrderId = null,
                                                          Guid? repairTaskId = null,
                                                          TimeStamps? time = null,
                                                          decimal? cost = null,
                                                          int? quantity = null)
        {
            return WorkOrderRepairTasks.Create(
                    workOrderId ?? Guid.NewGuid(),
                    repairTaskId ?? Guid.NewGuid(),
                    time ?? TimeStamps.Min15,
                    cost ?? 50m,
                    quantity ?? 1);
        }
    }
}
