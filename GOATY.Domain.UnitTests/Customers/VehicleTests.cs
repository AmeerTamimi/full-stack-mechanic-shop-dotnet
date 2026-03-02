using GOATY.Domain.Common.Results;
using GOATY.Domain.Customers.Vehicles;

namespace GOATY.Domain.UnitTests.Customers
{
    public sealed class VehicleTests
    {
        [Fact]
        public void Create_WithValidData_ShouldSucceed()
        {
            var customerId = Guid.NewGuid();
            var brand = "Toyota";
            var model = "Corolla";
            var year = DateTime.UtcNow.Year;
            var plate = "ABC-123";

            var result = Vehicle.Create(customerId, brand, model, year, plate);

            var actual = result.Value;

            Assert.True(result.IsSuccess);
            Assert.NotEqual(Guid.Empty, actual.Id);
            Assert.Equal(customerId, actual.CustomerId);
            Assert.Equal(brand, actual.Brand);
            Assert.Equal(model, actual.Model);
            Assert.Equal(year, actual.Year);
            Assert.Equal(plate, actual.LicensePlate);
            Assert.Equal($"{brand} | {model} | {year}", actual.VehicleInfo);
        }

        [Fact]
        public void Create_WithEmptyCustomerId_ShouldFail()
        {
            var result = Vehicle.Create(Guid.Empty, "Toyota", "Corolla", DateTime.UtcNow.Year, "ABC-123");

            var actual = result.Error;
            var expected = VehicleErrors.CustomerIdRequired;

            Assert.False(result.IsSuccess);
            Assert.Equal(actual, expected);
        }

        [Fact]
        public void Create_WithEmptyBrand_ShouldFail()
        {
            var result = Vehicle.Create(Guid.NewGuid(), "", "Corolla", DateTime.UtcNow.Year, "ABC-123");

            var actual = result.Error;
            var expected = VehicleErrors.BrandRequired;

            Assert.False(result.IsSuccess);
            Assert.Equal(actual, expected);
        }

        [Fact]
        public void Create_WithEmptyModel_ShouldFail()
        {
            var result = Vehicle.Create(Guid.NewGuid(), "Toyota", "   ", DateTime.UtcNow.Year, "ABC-123");

            var actual = result.Error;
            var expected = VehicleErrors.ModelRequired;

            Assert.False(result.IsSuccess);
            Assert.Equal(actual, expected);
        }

        [Fact]
        public void Create_WithEmptyLicensePlate_ShouldFail()
        {
            var result = Vehicle.Create(Guid.NewGuid(), "Toyota", "Corolla", DateTime.UtcNow.Year, "");

            var actual = result.Error;
            var expected = VehicleErrors.LicensePlateRequired;

            Assert.False(result.IsSuccess);
            Assert.Equal(actual, expected);
        }

        [Fact]
        public void Create_WithInvalidYear_ShouldFail()
        {
            var result = Vehicle.Create(Guid.NewGuid(), "Toyota", "Corolla", year: 1800, licensePlate: "ABC-123");

            var actual = result.Error;
            var expected = VehicleErrors.YearInvalid;

            Assert.False(result.IsSuccess);
            Assert.Equal(actual, expected);
        }

        [Fact]
        public void Update_WithValidData_ShouldSucceed()
        {
            var customerId = Guid.NewGuid();
            var vehicle = Vehicle.Create(customerId, "Toyota", "Corolla", 2020, "ABC-123").Value;

            var updateResult = vehicle.Update(customerId, "Honda", "Civic", 2022, "XYZ-999");

            var updateActual = updateResult.Value;
            var updateExpected = Result.Updated;

            Assert.True(updateResult.IsSuccess);
            Assert.Equal(updateActual, updateExpected);

            Assert.Equal(customerId, vehicle.CustomerId);
            Assert.Equal("Honda", vehicle.Brand);
            Assert.Equal("Civic", vehicle.Model);
            Assert.Equal(2022, vehicle.Year);
            Assert.Equal("XYZ-999", vehicle.LicensePlate);
        }

        [Fact]
        public void Update_WithInvalidYear_ShouldFail()
        {
            var customerId = Guid.NewGuid();
            var vehicle = Vehicle.Create(customerId, "Toyota", "Corolla", 2020, "ABC-123").Value;

            var updateResult = vehicle.Update(customerId, "Honda", "Civic", year: 9999, licensePlate: "XYZ-999");

            var updateActual = updateResult.Error;
            var updateExpected = VehicleErrors.YearInvalid;

            Assert.False(updateResult.IsSuccess);
            Assert.Equal(updateActual, updateExpected);

            // state should stay unchanged
            Assert.Equal("Toyota", vehicle.Brand);
            Assert.Equal("Corolla", vehicle.Model);
            Assert.Equal(2020, vehicle.Year);
            Assert.Equal("ABC-123", vehicle.LicensePlate);
        }
    }
}