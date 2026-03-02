using GOATY.Domain.Common.Results;

namespace GOATY.Domain.Customers
{
    public sealed class CustomerErrors
    {
        public static readonly Error InvalidId = Error.Validation(
            code: "Customer.Id.Invalid",
            description: "Customer id must not be empty."
        );

        public static readonly Error InvalidFirstName = Error.Validation(
            code: "Customer.FirstName.Invalid",
            description: "First name is required and must be between 5 and 50 characters."
        );

        public static readonly Error InvalidLastName = Error.Validation(
            code: "Customer.LastName.Invalid",
            description: "Last name is required and must be between 5 and 50 characters."
        );

        public static readonly Error InvalidPhoneNumber = Error.Validation(
            code: "Customer.Phone.Invalid",
            description: "Phone number must be one of these formats: 05[6|9]XXXXXXX, 9705[6|9]XXXXXXX, or 9725[6|9]XXXXXXX."
        );

        public static readonly Error InvalidEmail = Error.Validation(
            code: "Customer.Email.Invalid",
            description: "Email address is invalid."
        );

        public static readonly Error InvalidAddress = Error.Validation(
            code: "Customer.Address.Invalid",
            description: "Address must have a value."
        );

        public static readonly Error InvalidVehicles = Error.Validation(
            code: "Customer.Vehicles.Invalid",
            description: "Vehicles list must not be null or Empty."
        );
    }
}