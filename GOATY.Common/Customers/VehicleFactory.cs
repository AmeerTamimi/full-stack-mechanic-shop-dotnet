using GOATY.Domain.Common.Results;
using GOATY.Domain.Customers.Vehicles;

namespace GOATY.Tests.Common.Customers
{
    public static class VehicleFactory
    {
        public static Result<Vehicle> Create(Guid? customerId = null,
                                             string? brand = null,
                                             string? model = null,
                                             int? year = null,
                                             string? licensePlate = null)
        {
            return Vehicle.Create(
                customerId ?? Guid.NewGuid(),
                brand ?? "Toyota",
                model ?? "Corolla",
                year ?? DateTime.UtcNow.Year,
                licensePlate ?? "ABC-123");
        }
    }
}