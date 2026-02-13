using GOATY.Domain.Common.Results;
using GOATY.Domain.Parts;

namespace GOATY.Domain.UnitTests.Parts
{
    public sealed class PartTests
    {
        
        [Fact]        //     Method      Scenario        Outcome
        public void CreatePart_ForPartObject_ReturnsPartResult()
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
        public void CreatePart_ForPartObjectWithDuplicatedName_ReturnsNameAlreadyExistsError()
        {
            var id = Guid.NewGuid();
            string name = "ReservedName";
            var cost = 1000m;
            var quantity = 10;

            var actual = Part.Create(id, name, cost, quantity);

            var expected = PartErrors.NameRequiredError;

            Assert.False(actual.IsSuccess);
            Assert.Equal(actual.Error, expected);
        }

        [Fact]
        public void CreatePart_ForPartObjectWithEmptyName_ReturnsNameRequiredError11()
        {
            var id = Guid.NewGuid();
            string name = null!;
            var cost = 1000m;
            var quantity = 10;

            var actual = Part.Create(id, name, cost, quantity);

            var expected = PartErrors.NameRequiredError;

            Assert.False(actual.IsSuccess);
            Assert.Equal(actual.Error, expected);
        }

        [Fact]
        public void CreatePart_ForPartObjectWithEmptyName_ReturnsNameRequiredError()
        {
            var id = Guid.NewGuid();
            string name = null!;
            var cost = 1000m;
            var quantity = 10;

            var actual = Part.Create(id, name, cost, quantity);

            var expected = PartErrors.NameRequiredError;

            Assert.False(actual.IsSuccess);
            Assert.Equal(actual.Error, expected);
        }
    }
}
