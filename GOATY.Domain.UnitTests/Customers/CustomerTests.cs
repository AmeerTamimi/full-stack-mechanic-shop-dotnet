using GOATY.Domain.Common.Results;
using GOATY.Domain.Customers;
using GOATY.Domain.Customers.Vehicles;
using System.Reflection;

namespace GOATY.Domain.UnitTests.Customers
{
    public sealed class CustomerTests
    {
        private static Customer CreateValidCustomer(Guid id, List<Vehicle>? vehicles = null)
        {
            vehicles ??= [];

            return Customer.Create(
                id,
                firstName: "Ameer",
                lastName: "Tamimi",
                phone: "0591234567",
                email: "ameer@test.com",
                address: "Ramallah",
                vehicles: vehicles
            ).Value;
        }

        [Fact]
        public void Create_WithValidData_ShouldSucceed()
        {
            var id = Guid.NewGuid();
            var vehicles = new List<Vehicle>
            {
                Vehicle.Create(id, "Toyota", "Corolla", 2020, "ABC-123").Value
            };

            var result = Customer.Create(
                id,
                "Ameer",
                "Tamimi",
                "0591234567",
                "ameer@test.com",
                "Ramallah",
                vehicles
            );

            var actual = result.Value;

            Assert.True(result.IsSuccess);
            Assert.Equal(id, actual.Id);
            Assert.Equal("Ameer", actual.FirstName);
            Assert.Equal("Tamimi", actual.LastName);
            Assert.Equal("Ameer Tamimi", actual.FullName);
            Assert.Equal("0591234567", actual.Phone);
            Assert.Equal("ameer@test.com", actual.Email);
            Assert.Equal("Ramallah", actual.Address);
            Assert.Equivalent(vehicles, actual.Vehicles);
        }

        [Fact]
        public void Create_WithInvalidFirstName_ShouldFail()
        {
            var result = Customer.Create(
                Guid.NewGuid(),
                firstName: "Ab",
                lastName: "Tamimi",
                phone: "0591234567",
                email: "ameer@test.com",
                address: "Ramallah",
                vehicles: []
            );

            var actual = result.Error;
            var expected = CustomerErrors.InvalidFirstName;

            Assert.False(result.IsSuccess);
            Assert.Equal(actual, expected);
        }

        [Fact]
        public void Create_WithInvalidLastName_ShouldFail()
        {
            var result = Customer.Create(
                Guid.NewGuid(),
                firstName: "Ameer",
                lastName: "t",
                phone: "0591234567",
                email: "ameer@test.com",
                address: "Ramallah",
                vehicles: []
            );

            var actual = result.Error;
            var expected = CustomerErrors.InvalidLastName;

            Assert.False(result.IsSuccess);
            Assert.Equal(actual, expected);
        }

        [Fact]
        public void Create_WithInvalidPhone_ShouldFail()
        {
            var result = Customer.Create(
                Guid.NewGuid(),
                firstName: "Ameer",
                lastName: "Tamimi",
                phone: "123",
                email: "ameer@test.com",
                address: "Ramallah",
                vehicles: []
            );

            var actual = result.Error;
            var expected = CustomerErrors.InvalidPhoneNumber;

            Assert.False(result.IsSuccess);
            Assert.Equal(actual, expected);
        }

        [Fact]
        public void Create_WithInvalidEmail_ShouldFail()
        {
            var result = Customer.Create(
                Guid.NewGuid(),
                firstName: "Ameer",
                lastName: "Tamimi",
                phone: "0591234567",
                email: "not-an-email",
                address: "Ramallah",
                vehicles: []
            );

            var actual = result.Error;
            var expected = CustomerErrors.InvalidEmail;

            Assert.False(result.IsSuccess);
            Assert.Equal(actual, expected);
        }

        [Fact]
        public void Create_WithInvalidAddress_ShouldFail()
        {
            var result = Customer.Create(
                Guid.NewGuid(),
                firstName: "Ameer",
                lastName: "Tamimi",
                phone: "0591234567",
                email: "ameer@test.com",
                address: "   ",
                vehicles: []
            );

            var actual = result.Error;
            var expected = CustomerErrors.InvalidAddress;

            Assert.False(result.IsSuccess);
            Assert.Equal(actual, expected);
        }

        [Fact]
        public void Create_WithNullVehicles_ShouldFail()
        {
            var result = Customer.Create(
                Guid.NewGuid(),
                firstName: "Ameer",
                lastName: "Tamimi",
                phone: "0591234567",
                email: "ameer@test.com",
                address: "Ramallah",
                vehicles: null!
            );

            var actual = result.Error;
            var expected = CustomerErrors.InvalidVehicles;

            Assert.False(result.IsSuccess);
            Assert.Equal(actual, expected);
        }

        [Fact]
        public void Update_WithValidData_ShouldSucceed()
        {
            var customer = CreateValidCustomer(Guid.NewGuid());

            var updateResult = customer.Update(
                firstName: "Ahmad",
                lastName: "Khaled",
                phone: "0561234567",
                email: "ahmad@test.com",
                address: "Jenin"
            );

            var updateActual = updateResult.Value;
            var updateExpected = Result.Updated;

            Assert.True(updateResult.IsSuccess);
            Assert.Equal(updateActual, updateExpected);

            Assert.Equal("Ahmad", customer.FirstName);
            Assert.Equal("Khaled", customer.LastName);
            Assert.Equal("Ahmad Khaled", customer.FullName);
            Assert.Equal("0561234567", customer.Phone);
            Assert.Equal("ahmad@test.com", customer.Email);
            Assert.Equal("Jenin", customer.Address);
        }

        [Fact]
        public void Update_WithInvalidEmail_ShouldFail()
        {
            var customer = CreateValidCustomer(Guid.NewGuid());

            var updateResult = customer.Update(
                firstName: "Ahmad",
                lastName: "Khaled",
                phone: "0561234567",
                email: "bad-email",
                address: "Jenin"
            );

            var updateActual = updateResult.Error;
            var updateExpected = CustomerErrors.InvalidEmail;

            Assert.False(updateResult.IsSuccess);
            Assert.Equal(updateActual, updateExpected);

            Assert.Equal("Ameer", customer.FirstName);
            Assert.Equal("Tamimi", customer.LastName);
            Assert.Equal("0591234567", customer.Phone);
            Assert.Equal("ameer@test.com", customer.Email);
            Assert.Equal("Ramallah", customer.Address);
        }

        [Fact]
        public void UpsertVehicles_WithNewVehicles_ShouldAddThem()
        {
            var id = Guid.NewGuid();
            var customer = CreateValidCustomer(id, vehicles: []);

            var incoming = new List<Vehicle>
            {
                Vehicle.Create(id, "Toyota", "Corolla", 2020, "ABC-123").Value,
                Vehicle.Create(id, "Honda", "Civic", 2022, "XYZ-999").Value
            };

            var result = customer.UpsertVehicles(incoming);

            Assert.True(result.IsSuccess);
            Assert.Equal(Result.Updated, result.Value);
            Assert.Equal(2, customer.Vehicles.Count());

            var plates = customer.Vehicles.Select(v => v.LicensePlate).ToList();
            Assert.Contains("ABC-123", plates);
            Assert.Contains("XYZ-999", plates);
        }

        [Fact]
        public void UpsertVehicles_ShouldRemoveVehiclesNotInIncoming()
        {
            var id = Guid.NewGuid();

            var v1 = Vehicle.Create(id, "Toyota", "Corolla", 2020, "ABC-123").Value;
            var v2 = Vehicle.Create(id, "Honda", "Civic", 2022, "XYZ-999").Value;

            var customer = CreateValidCustomer(id, vehicles: [v1, v2]);

            var incoming = new List<Vehicle> { v1 };

            var result = customer.UpsertVehicles(incoming);

            Assert.True(result.IsSuccess);
            Assert.Equal(Result.Updated, result.Value);
            Assert.Single(customer.Vehicles);
            Assert.Equal("ABC-123", customer.Vehicles.First().LicensePlate);
        }

        [Fact]
        public void UpsertVehicles_ShouldUpdateExistingVehicle_WhenIdsMatch()
        {
            var id = Guid.NewGuid();

            var existing = Vehicle.Create(id, "Toyota", "Corolla", 2020, "ABC-123").Value;
            var customer = CreateValidCustomer(id, vehicles: [existing]);

            var incomingDifferent = Vehicle.Create(id, "BMW", "M3", 2023, "NEW-777").Value;
            ForceSetId(incomingDifferent, existing.Id);

            var result = customer.UpsertVehicles([incomingDifferent]);

            Assert.True(result.IsSuccess);
            Assert.Equal(Result.Updated, result.Value);

            var updated = customer.Vehicles.Single();
            Assert.Equal(existing.Id, updated.Id);
            Assert.Equal("BMW", updated.Brand);
            Assert.Equal("M3", updated.Model);
            Assert.Equal(2023, updated.Year);
            Assert.Equal("NEW-777", updated.LicensePlate);
        }

        private static void ForceSetId(object entity, Guid id)
        {
            var baseType = entity.GetType().BaseType!;
            var idProp = baseType.GetProperty("Id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (idProp is not null)
            {
                idProp.SetValue(entity, id);
                return;
            }

            var idField = baseType.GetField("<Id>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
            if (idField is not null)
            {
                idField.SetValue(entity, id);
            }
        }
    }
}