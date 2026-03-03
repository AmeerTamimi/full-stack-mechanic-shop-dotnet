using GOATY.Domain.Common.Results;

namespace GOATY.Domain.WorkOrders
{
    public sealed class WorkOrderErrors
    {
        public static readonly Error InvalidId = Error.Validation(
            code: "WorkOrder.Id.Invalid",
            description: "WorkOrder id must not be empty."
        );

        public static readonly Error InvalidVehicleId = Error.Validation(
            code: "WorkOrder.VehicleId.Invalid",
            description: "WorkOrder vehicle id must not be empty."
        );

        public static readonly Error InvalidCustomerId = Error.Validation(
            code: "WorkOrder.CustomerId.Invalid",
            description: "WorkOrder customer id must not be empty."
        );

        public static readonly Error InvalidTechnicianId = Error.Validation(
            code: "WorkOrder.TechnicianId.Invalid",
            description: "WorkOrder technician id must not be empty."
        );

        public static readonly Error InvalidStartTime = Error.Validation(
            code: "WorkOrder.StartTime.Invalid",
            description: "Invalid work order start time."
        );

        public static readonly Error InvalidRepairTasks = Error.Validation(
            code: "WorkOrder.RepairTasks.Invalid",
            description: "WorkOrder must contain at least one repair task."
        );

        public static readonly Error InvalidWorkOrderId = Error.Validation(
            code: "WorkOrderRepairTask.InvalidWorkOrderId",
            description: "WorkOrderId is required."
        );

        public static readonly Error InvalidRepairTaskId = Error.Validation(
            code: "WorkOrderRepairTask.InvalidRepairTaskId",
            description: "RepairTaskId is required."
        );

        public static readonly Error InvalidRepairTaskTime = Error.Validation(
            code: "WorkOrderRepairTask.InvalidTime",
            description: "Time must be a valid positive TimeStamps value."
        );

        public static readonly Error InvalidRepairTaskCost = Error.Validation(
            code: "WorkOrderRepairTask.InvalidCost",
            description: "Cost must be greater than 0."
        );
    }
}