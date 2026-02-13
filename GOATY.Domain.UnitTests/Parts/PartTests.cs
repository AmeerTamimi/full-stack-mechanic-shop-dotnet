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
            var actual = Part.Create(id, name, cost ,quantity);

            var expected = new Part
            {
                Id = id,
                Name = name,
                Cost = cost,
                Quantity = quantity
            };

            // Assert
            Assert.True(actual.IsSuccess);
            Assert.Equivalent(actual.Value, expected);
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
            Assert.Equal(actual.Error, expected);
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
            Assert.Equal(actual.Error, expected);
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
            Assert.Equal(actual.Error, expected);
        }

        [Fact]
        public void Update_WithValidData_ShouldSucced()
        {
            var part = new Part
            {
                Id = Guid.NewGuid(),
                Name = "Old Name",
                Cost = 100m,
                Quantity = 10
            };

            var result = Part.Update(part, "New Name", 50, 5);

            var expectedName = "New Name";
            var expectedCost = 50;
            var expectedQuantity = 5;

            Assert.Equal(part.Name, expectedName);
            Assert.Equal(part.Cost, expectedCost);
            Assert.Equal(part.Quantity, expectedQuantity);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void Update_WithInvalidName_ShouldFail()
        {
            var part = new Part
            {
                Id = Guid.NewGuid(),
                Name = "Old Name",
                Cost = 100m,
                Quantity = 10
            };

            var result = Part.Update(part, "", 50, 5);

            Assert.False(result.IsSuccess);
        }

        [Fact]
        public void Update_WithInvalidCost_ShouldFail()
        {
            var part = new Part
            {
                Id = Guid.NewGuid(),
                Name = "Old Name",
                Cost = 100m,
                Quantity = 10
            };

            var result = Part.Update(part, "New Name", -50, 5);

            Assert.False(result.IsSuccess);
        }

        [Fact]
        public void Update_WithInvalidQuantity_ShouldFail()
        {
            var part = new Part
            {
                Id = Guid.NewGuid(),
                Name = "Old Name",
                Cost = 100m,
                Quantity = 10
            };

            var result = Part.Update(part, "New Name", 50, -5);

            Assert.False(result.IsSuccess);
        }
    }
}
