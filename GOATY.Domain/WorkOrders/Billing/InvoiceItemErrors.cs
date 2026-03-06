using GOATY.Domain.Common.Results;

namespace GOATY.Domain.WorkOrders.Billing
{
    public static class InvoiceItemErrors
    {
        public static readonly Error InvalidId = Error.Validation(
            code: "InvoiceItem.Id.Invalid",
            description: "Invoice item id must not be empty."
        );

        public static readonly Error InvalidInvoiceId = Error.Validation(
            code: "InvoiceItem.InvocieId.Invalid",
            description: "Invoice id must not be empty."
        );
        
        public static readonly Error InvalidTechnicianCost = Error.Validation(
            code: "InvoiceItem.TechnicianCost.Invalid",
            description: "Invoice item technician cost must be greater than or equal to 50."
        );

        public static readonly Error InvalidQuantity = Error.Validation(
            code: "InvoiceItem.Quantity.Invalid",
            description: "Invoice item quantity must be greater than 0."
        );

        public static readonly Error InvalidUnitPrice = Error.Validation(
            code: "InvoiceItem.UnitPrice.Invalid",
            description: "Invoice item unit price must be greater than or equal to 0."
        );

        public static readonly Error InvalidTotal = Error.Validation(
            code: "InvoiceItem.Total.Invalid",
            description: "Invoice item total must be greater than or equal to 0."
        );

        public static readonly Error MissingSource = Error.Validation(
            code: "InvoiceItem.Source.Missing",
            description: "Invoice item must reference either a repair task or a part."
        );

        public static readonly Error ConflictingSources = Error.Conflict(
            code: "InvoiceItem.Source.Conflict",
            description: "Invoice item cant reference both a repair task and a part ."
        );
        
        public static readonly Error InvalidRepairTaskId = Error.Validation(
            code: "InvoiceItem.RepairTaskId.Invalid",
            description: "Repair task id is invalid."
        );

        public static readonly Error InvalidPartId = Error.Validation(
            code: "InvoiceItem.PartId.Invalid",
            description: "Part id is invalid."
        );
    }
}