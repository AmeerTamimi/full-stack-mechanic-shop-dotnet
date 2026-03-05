using GOATY.Domain.Common.Results;
using GOATY.Domain.Parts;

namespace GOATY.Domain.UnitTests.Parts
{
    public sealed class PartTests
    {
        [Fact]   // Method    Scenario      Outcome
        public void Create_WithValidData_ShouldSucceed()
        {
            // Arrange
            var id = Guid.NewGuid();
            var name = "PartName";
            var cost = 1000m;
            var quantity = 10;

            // Act
            var actual = Part.Create(id, name, cost, quantity);

            // Assert
            Assert.True(actual.IsSuccess);
            Assert.Equal(id, actual.Value.Id);
            Assert.Equal(name, actual.Value.Name);
            Assert.Equal(cost, actual.Value.Cost);
            Assert.Equal(quantity, actual.Value.Quantity);
        }

        [Fact]
        public void Create_WithInvalidName_ShouldFail()
        {
            var id = Guid.NewGuid();
            string name = "";
            var cost = 1000m;
            var quantity = 10;

            var actual = Part.Create(id, name, cost, quantity);

            var expected = PartErrors.NameInvalidError;

            Assert.False(actual.IsSuccess);
            Assert.Equal(expected, actual.Error);
        }

        [Fact]
        public void Create_WithInvalidCost_ShouldFail()
        {
            var id = Guid.NewGuid();
            string name = "Good Name";
            var cost = 0;
            var quantity = 10;

            var actual = Part.Create(id, name, cost, quantity);

            var expected = PartErrors.CostInvalidError;

            Assert.False(actual.IsSuccess);
            Assert.Equal(expected, actual.Error);
        }

        [Fact]
        public void Create_WithInvalidQuantity_ShouldFail()
        {
            var id = Guid.NewGuid();
            string name = "Good Name";
            var cost = 100m;
            var quantity = -1;

            var actual = Part.Create(id, name, cost, quantity);

            var expected = PartErrors.QuantityInvalidError;

            Assert.False(actual.IsSuccess);
            Assert.Equal(expected, actual.Error);
        }

        [Fact]
        public void Update_WithValidData_ShouldSucced()
        {
            var id = Guid.NewGuid();
            var created = Part.Create(id, "Old Name", 100m, 10);
            var part = created.Value;

            var result = part.Update("New Name", 50m, 5);

            Assert.True(result.IsSuccess);
            Assert.Equal("New Name", part.Name);
            Assert.Equal(50m, part.Cost);
            Assert.Equal(5, part.Quantity);
        }

        [Fact]
        public void Update_WithInvalidName_ShouldFail()
        {
            var id = Guid.NewGuid();
            var created = Part.Create(id, "Old Name", 100m, 10);
            var part = created.Value;

            var result = part.Update("", 50m, 5);

            Assert.False(result.IsSuccess);
        }

        [Fact]
        public void Update_WithInvalidCost_ShouldFail()
        {
            var id = Guid.NewGuid();
            var created = Part.Create(id, "Old Name", 100m, 10);
            var part = created.Value;

            var result = part.Update("New Name", -50m, 5);

            Assert.False(result.IsSuccess);
        }

        [Fact]
        public void Update_WithInvalidQuantity_ShouldFail()
        {
            var id = Guid.NewGuid();
            var created = Part.Create(id, "Old Name", 100m, 10);
            var part = created.Value;

            var result = part.Update("New Name", 50m, -5);

            Assert.False(result.IsSuccess);
        }
    }
}