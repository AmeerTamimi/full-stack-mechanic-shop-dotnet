using GOATY.Domain.RepairTasks;

namespace GOATY.Domain.UnitTests.RepairTasks
{
    public sealed class RepairTaskDetailsTests
    {
        [Fact]
        public void Create_WithValidData_ShouldSucceed()
        {
            var repairTaskId = Guid.NewGuid();
            var partId = Guid.NewGuid();
            var quantity = 2;
            var unitPrice = 30m;

            var result = RepairTaskDetails.Create(repairTaskId, partId, quantity, unitPrice);

            var actual = result.Value;

            Assert.True(result.IsSuccess);
            Assert.Equal(repairTaskId, actual.RepairTaskId);
            Assert.Equal(partId, actual.PartId);
            Assert.Equal(quantity, actual.Quantity);
            Assert.Equal(unitPrice, actual.UnitPrice);
        }

        [Fact]
        public void Create_WithInvalidRepairTaskId_ShouldFail()
        {
            var repairTaskId = Guid.Empty;
            var partId = Guid.NewGuid();

            var result = RepairTaskDetails.Create(repairTaskId, partId, quantity: 1, unitPrice: 10m);

            var actual = result.Error;
            var expected = RepairTaskErrors.InvalidId;

            Assert.False(result.IsSuccess);
            Assert.Equal(actual, expected);
        }

        [Fact]
        public void Create_WithInvalidPartId_ShouldFail()
        {
            var repairTaskId = Guid.NewGuid();
            var partId = Guid.Empty;

            var result = RepairTaskDetails.Create(repairTaskId, partId, quantity: 1, unitPrice: 10m);

            var actual = result.Error;
            var expected = RepairTaskErrors.InvalidPartId;

            Assert.False(result.IsSuccess);
            Assert.Equal(actual, expected);
        }

        [Fact]
        public void Create_WithInvalidQuantity_ShouldFail()
        {
            var repairTaskId = Guid.NewGuid();
            var partId = Guid.NewGuid();
            var quantity = 0;

            var result = RepairTaskDetails.Create(repairTaskId, partId, quantity, unitPrice: 10m);

            var actual = result.Error;
            var expected = RepairTaskErrors.InvalidQuantity;

            Assert.False(result.IsSuccess);
            Assert.Equal(actual, expected);
        }

        [Fact]
        public void Create_WithInvalidUnitPrice_ShouldFail()
        {
            var repairTaskId = Guid.NewGuid();
            var partId = Guid.NewGuid();
            var unitPrice = 0m;

            var result = RepairTaskDetails.Create(repairTaskId, partId, quantity: 1, unitPrice);

            var actual = result.Error;
            var expected = RepairTaskErrors.InvalidUnitPrice;

            Assert.False(result.IsSuccess);
            Assert.Equal(actual, expected);
        }
    }
}