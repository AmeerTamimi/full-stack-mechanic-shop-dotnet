using GOATY.Domain.Common.Constans;
using GOATY.Domain.WorkOrders.Billing;
using GOATY.Tests.Common.WorkOrders.Invoices;

namespace GOATY.Domain.UnitTests.WorkOrders.Billing
{
    public sealed class InvoiceItemTests
    {
        [Fact]
        public void Create_WithValidRepairTaskSource_ShouldSucceed()
        {
            var result = InvoiceItemFactory.Create();

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void Create_WithInvalidId_ShouldFail()
        {
            var result = InvoiceItemFactory.Create(id: Guid.Empty);

            Assert.False(result.IsSuccess);
            Assert.Equal(InvoiceItemErrors.InvalidId, result.Error);
        }

        [Fact]
        public void Create_WithInvalidInvoiceId_ShouldFail()
        {
            var result = InvoiceItemFactory.Create(invoiceId: Guid.Empty);

            Assert.False(result.IsSuccess);
            Assert.Equal(InvoiceItemErrors.InvalidInvoiceId, result.Error);
        }

        [Fact]
        public void Create_WithInvalidTechnicianCost_ShouldFail()
        {
            var result = InvoiceItemFactory.Create(
                technicianCost: GOATYConstans.TechnicianBase - 1);

            Assert.False(result.IsSuccess);
            Assert.Equal(InvoiceItemErrors.InvalidTechnicianCost, result.Error);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Create_WithInvalidQuantity_ShouldFail(int quantity)
        {
            var result = InvoiceItemFactory.Create(quantity: quantity);

            Assert.False(result.IsSuccess);
            Assert.Equal(InvoiceItemErrors.InvalidQuantity, result.Error);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-50)]
        public void Create_WithInvalidUnitPrice_ShouldFail(decimal unitPrice)
        {
            var result = InvoiceItemFactory.Create(unitPrice: unitPrice);

            Assert.False(result.IsSuccess);
            Assert.Equal(InvoiceItemErrors.InvalidUnitPrice, result.Error);
        }

        [Fact]
        public void Create_WithConflictingSources_ShouldFail()
        {
            var result = InvoiceItemFactory.Create(
                repairTaskId: Guid.NewGuid(),
                partId: Guid.NewGuid());

            Assert.False(result.IsSuccess);
            Assert.Equal(InvoiceItemErrors.ConflictingSources, result.Error);
        }
    }
}