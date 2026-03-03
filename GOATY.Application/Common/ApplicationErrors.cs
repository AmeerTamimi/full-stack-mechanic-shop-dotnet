using GOATY.Domain.Common.Results;

namespace GOATY.Application.Common
{
    public static class ApplicationErrors
    {
        public static Error VehicleHasWorkOrderConflict =
            Error.Conflict(
                code: "WorkOrder.Vehicle.Conflict",
                description: $"Vehicle already has a Scheduled/inProgress Work order."
            );

        public static readonly Error VehicleDoesNotExist = Error.NotFound(
            code: "Vehicle_NotFound",
            description: "Vehicle was not found."
        );

        public static readonly Error CustomerDoesNotExist = Error.NotFound(
            code: "Customer_NotFound",
            description: "Customer was not found."
        );

        public static readonly Error EmployeeDoesNotExist = Error.NotFound(
            code: "Employee_NotFound",
            description: "Employee was not found."
        );

        public static readonly Error WorkOrderDoesNotExist = Error.NotFound(
            code: "WorkOrder_NotFound",
            description: "WorkOrder was not found."
        );

        public static readonly Error RepairTaskDoesNotExist = Error.NotFound(
            code: "RepairTask_NotFound",
            description: "RepairTask was not found."
        );

        public static readonly Error PartDoesNotExist = Error.NotFound(
            code: "Part_NotFound",
            description: "Part was not found."
        );
    }
}