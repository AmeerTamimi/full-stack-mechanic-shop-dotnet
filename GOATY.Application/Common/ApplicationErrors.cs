using GOATY.Domain.Common.Results;

namespace GOATY.Application.Common
{
    public static class ApplicationErrors
    {

        public static readonly Error VehicleHasSchedulingConflict =
                Error.Conflict(
                    code: "WorkOrder.Vehicle.SchedulingConflict",
                    description: "Vehicle already has a work order that overlaps with the requested time."
                );

        public static Error CustomerDoesNotOwnVehicle = 
                Error.Conflict(
                    code: "Customer.Vehicle.NotOwned",
                    description: $"The Customer does not own The Vehicle."
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

        public static readonly Error WorkOrderIsOccupied =
                Error.Conflict(
                    code: "WorkOrder.Occupied",
                    description: "Work order has technician already."
                );

        public static readonly Error TechnicianIsOccupied =
            Error.Conflict(
                code: "Technician.Occupied",
                description: "Technician has another work order that overlaps with the requested time."
            );

        public static readonly Error CannotChangeStateWhenClosed =
                Error.Conflict(
                    code: "WorkOrder.State.Closed",
                    description: "Cannot change work order state when it is Completed or Cancelled."
                );

        public static readonly Error BayIsOccupied =
            Error.Conflict(
                code: "Bay.Occupied",
                description: "Bay is occupied."
            );

        public static readonly Error NoWorkOrders =
            Error.NotFound(
                code: "WorkOrder.NotFound",
                description: "No work orders were found for the selected day."
            );
    }
}