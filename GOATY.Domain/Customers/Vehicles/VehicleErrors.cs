using GOATY.Domain.Common.Results;

namespace GOATY.Domain.Customers.Vehicles
{
    public static class VehicleErrors
    {
        public static readonly Error CustomerIdRequired = Error.Validation(
            code: "Vehicle.CustomerId.Required",
            description: "Vehicle Customer Id is required."
        );

        public static readonly Error BrandRequired = Error.Validation(
            code: "Vehicle.Brand.Required",
            description: "Vehicle brand is required."
        );

        public static readonly Error ModelRequired = Error.Validation(
            code: "Vehicle.Model.Required",
            description: "Vehicle model is required."
        );

        public static readonly Error LicensePlateRequired = Error.Validation(
            code: "Vehicle.LicensePlate.Required",
            description: "Vehicle license plate is required."
        );

        public static Error YearInvalid = Error.Validation(
            code: "Vehicle.Year.Invalid",
            description: $"Vehicle year must be between {1900} and {DateTime.Now.Year}."
        );
    }
}