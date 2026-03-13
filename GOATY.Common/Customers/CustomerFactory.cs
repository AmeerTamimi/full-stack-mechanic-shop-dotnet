using GOATY.Domain.Common.Results;
using GOATY.Domain.Customers;
using GOATY.Domain.Customers.Vehicles;

namespace GOATY.Tests.Common.Customers
{
    public static class CustomerFactory
    {
        public static Result<Customer> Create(Guid? id = null,
                                              string? firstName = null,
                                              string? lastName = null,
                                              string? phone = null,
                                              string? email = null,
                                              string? address = null,
                                              List<Vehicle>? vehicles = null)
        {
            return Customer.Create(
                id ?? Guid.NewGuid(),
                firstName ?? "Ameer",
                lastName ?? "Tamimi",
                phone ?? "0591234567",
                email ?? "a@test.com",
                address ?? "Ramallah",
                vehicles ?? [VehicleFactory.Create().Value]);
        }
    }
}