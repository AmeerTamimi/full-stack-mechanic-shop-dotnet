using GOATY.Domain.Common.Enums;
using GOATY.Domain.Common.Results;
using GOATY.Domain.RepairTasks;

namespace GOATY.Domain.WorkOrders
{
    public sealed class WorkOrderRepairTasks
    {
        public Guid WorkOrderId { get; private set; }
        public Guid RepairTaskId { get; private set; }
        public int Quantity { get; private set; }
        public TimeStamps Time { get; private set; }
        public decimal Cost { get; private set; }

        public WorkOrder WorkOrder { get; private set; } = null!;
        public RepairTask RepairTask { get; private set; } = null!;

        private WorkOrderRepairTasks() { }

        private WorkOrderRepairTasks(Guid workOrderId,
                                     Guid repairTaskId,
                                     TimeStamps time,
                                     decimal cost,
                                     int quantity)
        {
            WorkOrderId = workOrderId;
            RepairTaskId = repairTaskId;
            Time = time;
            Cost = cost;
            Quantity = quantity;
        }

        public static Result<WorkOrderRepairTasks> Create(Guid workOrderId,
                                                          Guid repairTaskId,
                                                          TimeStamps time,
                                                          decimal cost,
                                                          int quantity)
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

            if (quantity <= 0)
            {
                return WorkOrderErrors.InvalidQuantity;
            }

            return new WorkOrderRepairTasks(workOrderId, repairTaskId, time, cost , quantity);
        }

        public Result<Updated> Update(TimeStamps time, decimal cost , int quantity)
        {
            if (!Enum.IsDefined(typeof(TimeStamps), time))
            {
                return WorkOrderErrors.InvalidRepairTaskTime;
            }

            if (cost <= 0)
            {
                return WorkOrderErrors.InvalidRepairTaskCost;
            }

            if (quantity <= 0)
            {
                return WorkOrderErrors.InvalidQuantity;
            }

            Time = time;
            Cost = cost;
            Quantity = quantity;

            return Result.Updated;
        }
    }
}