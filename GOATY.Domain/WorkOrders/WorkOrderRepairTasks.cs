using System;
using GOATY.Domain.Common.Enums;
using GOATY.Domain.Common.Results;
using GOATY.Domain.RepairTasks;

namespace GOATY.Domain.WorkOrders
{
    public sealed class WorkOrderRepairTasks
    {
        public Guid WorkOrderId { get; private set; }
        public Guid RepairTaskId { get; private set; }
        public TimeStamps Time { get; private set; }
        public decimal Cost { get; private set; }

        public WorkOrder WorkOrder { get; private set; } = null!;
        public RepairTask RepairTask { get; private set; } = null!;

        private WorkOrderRepairTasks() { }

        private WorkOrderRepairTasks(Guid workOrderId,
                                     Guid repairTaskId,
                                     TimeStamps time,
                                     decimal cost)
        {
            WorkOrderId = workOrderId;
            RepairTaskId = repairTaskId;
            Time = time;
            Cost = cost;
        }

        public static Result<WorkOrderRepairTasks> Create(Guid workOrderId,
                                                          Guid repairTaskId,
                                                          TimeStamps time,
                                                          decimal cost)
        {
            if (Guid.Empty == workOrderId)
            {
                return WorkOrderErrors.InvalidWorkOrderId;
            }

            if (Guid.Empty == repairTaskId)
            {
                return WorkOrderErrors.InvalidRepairTaskId;
            }

            if (!Enum.IsDefined(typeof(TimeStamps), time))
            {
                return WorkOrderErrors.InvalidRepairTaskTime;
            }

            if (cost <= 0)
            {
                return WorkOrderErrors.InvalidRepairTaskCost;
            }

            return new WorkOrderRepairTasks(workOrderId, repairTaskId, time, cost);
        }

        public Result<Updated> Update(TimeStamps time, decimal cost)
        {
            if (!Enum.IsDefined(typeof(TimeStamps), time))
            {
                return WorkOrderErrors.InvalidRepairTaskTime;
            }

            if (cost <= 0)
            {
                return WorkOrderErrors.InvalidRepairTaskCost;
            }

            Time = time;
            Cost = cost;

            return Result.Updated;
        }
    }
}