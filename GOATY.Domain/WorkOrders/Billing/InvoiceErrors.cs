using GOATY.Domain.Common.Results;

namespace GOATY.Domain.WorkOrders.Billing
{
    public static class InvoiceErrors
    {
        public static readonly Error InvalidId = Error.Validation(
            code: "Invoice.Id.Invalid",
            description: "Invoice id must not be empty."
        );

        public static readonly Error InvalidDiscount = Error.Validation(
            code: "Invoice.Discount.Invalid",
            description: "Invoice discount must be greater than or equal to 0 and less than or equal 100."
        );

        public static readonly Error InvalidIssuedDate = Error.Validation(
            code: "Invoice.IssuedAt.Invalid",
            description: "Invoice issued date cannot be in the future."
        );

        public static readonly Error InvalidPayDate = Error.Validation(
            code: "Invoice.PaidAt.Invalid",
            description: "Invoice paid date cannot be in the future."
        );

        public static readonly Error InvalidStatus = Error.Validation(
            code: "Invoice.Status.Invalid",
            description: "Invoice status is invalid."
        );

        public static readonly Error InvalidTax = Error.Validation(
            code: "Invoice.Tax.Invalid",
            description: "Invoice tax must be greater than or equal to 0."
        );

        public static readonly Error InvalidTotalPrice = Error.Validation(
            code: "Invoice.Total.Invalid",
            description: "Invoice total price must be greater than or equal to 0."
        );

        public static readonly Error InvalidWorkOrderId = Error.Validation(
            code: "Invoice.WorkOrderId.Invalid",
            description: "Work order id must not be empty."
        );

        public static readonly Error InvalidInvoiceItems = Error.Validation(
            code: "Invoice.Items.Invalid",
            description: "Invoice must contain at least one invoice item."
        );

        public static readonly Error InvoiceNotEditable = Error.Validation(
            code: "Invoice.Not.Editable",
            description: "Invoice Cant Be Edited"
        );
        public static readonly Error InvalidStatusTransition = Error.Validation(
            code: "Invoice.Transition.Invalid",
            description: "Invoice Status Valid Transition : NotPayed -> Payed , Payed -> Refunded"
        );

    }
    
}