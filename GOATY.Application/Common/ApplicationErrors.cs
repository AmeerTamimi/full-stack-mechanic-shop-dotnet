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

        public static readonly Error WorkOrderAlreadyHasInvoice =
            Error.Conflict(
                code: "WorkOrder.Invoice.AlreadyExists",
                description: "Cannot create another invoice because this work order already has one."
            );

        public static readonly Error InvoiceAlreadyPayed =
            Error.Conflict(
                code: "Invoice.Pay.AlreadyPayed",
                description: "Cannot settle an invoice that is already payed."
            );

        public static readonly Error InvoiceIsRefunded =
            Error.Conflict(
                code: "Invoice.Pay.Refunded",
                description: "Cannot settle/refund an invoice that has been refunded."
            );

        public static readonly Error WorkOrderNotCompleted =
            Error.Conflict(
                code: "WorkOrder.NotCompleted",
                description: "Cannot settle the invoice because the work order is not completed yet."
            );

        public static readonly Error InvoiceIsNotPayed =
            Error.Conflict(
                code: "Invoice.Refund.NotPayed",
                description: "Cannot refund an invoice that has not been payed yet."
            );

    }
}