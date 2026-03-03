using GOATY.Domain.Common.Results;

namespace GOATY.Application.Common
{
    public static class ApplicationErrors
    {
        public static Error VehicleHasWorkOrderConflict =
                Error.Conflict(
                    code: "WorkOrder.Vehicle.Conflict",
                    description: $"Vehicle already has an inProgress Work order."
                );

        public static readonly Error VehicleHasWorkOrderOverlap =
                Error.Conflict(
                    code: "WorkOrder.Vehicle.Overlap",
                    description: "Vehicle has another Scheduled work order that overlaps with the requested time."
                );

        public static Error CustomerDoesNotOwnVehicle = 
                Error.Conflict(
                    code: "Customer.Vehicle.NotOwned",
                    description: $"The Customer does not own The Vehicle."
                );

        public static readonly Error EmployeeHasWorkOrderOverlap =
                Error.Conflict(
                    code: "WorkOrder.Employee.Overlap",
                    description: "Employee has another work order that overlaps with the requested time."
                );

        public static readonly Error BayHasWorkOrderOverlap =
                Error.Conflict(
                    code: "WorkOrder.Bay.Overlap",
                    description: "Bay has another work order that overlaps with the requested time."
                );

        public static readonly Error CannotRemoveWorkOrderInProgress =
                Error.Conflict(
                    code: "WorkOrder.Remove.InProgress",
                    description: "Cannot remove a work order while it is InProgress."
                );

        public static readonly Error CannotChangeWorkOrderStateBeforeStartTime =
                Error.Validation(
                    code: "WorkOrder.State.StartTime.NotReached",
                    description: "Cannot change work order state before its StartTime."
                );
    }
}