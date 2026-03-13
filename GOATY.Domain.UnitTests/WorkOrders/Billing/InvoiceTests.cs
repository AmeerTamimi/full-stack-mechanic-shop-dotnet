using GOATY.Domain.Common.Results;
using GOATY.Domain.WorkOrders.Billing;
using GOATY.Tests.Common.WorkOrders.Invoices;

namespace GOATY.Domain.UnitTests.WorkOrders.Billing
{
    public sealed class InvoiceTests
    {
        [Fact]
        public void Create_WithValidData_ShouldSucceed()
        {
            var result = InvoiceFactory.Create();

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void Create_WithInvalidId_ShouldFail()
        {
            var result = InvoiceFactory.Create(id: Guid.Empty);

            Assert.False(result.IsSuccess);
            Assert.Equal(InvoiceErrors.InvalidId, result.Error);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(101)]
        public void Create_WithInvalidDiscount_ShouldFail(decimal discount)
        {
            var result = InvoiceFactory.Create(discount: discount);

            Assert.False(result.IsSuccess);
            Assert.Equal(InvoiceErrors.InvalidDiscount, result.Error);
        }

        [Fact]
        public void Create_WithInvalidWorkOrderId_ShouldFail()
        {
            var result = InvoiceFactory.Create(workOrderId: Guid.Empty);

            Assert.False(result.IsSuccess);
            Assert.Equal(InvoiceErrors.InvalidWorkOrderId, result.Error);
        }

        [Fact]
        public void Create_WithNullInvoiceItems_ShouldFail()
        {
            var result = Invoice.Create(
                Guid.NewGuid(),
                Guid.NewGuid(),
                10m,
                null!);

            Assert.False(result.IsSuccess);
            Assert.Equal(InvoiceErrors.InvalidInvoiceItems, result.Error);
        }

        [Fact]
        public void Create_WithEmptyInvoiceItems_ShouldFail()
        {
            var result = InvoiceFactory.Create(invoiceItems: new List<InvoiceItem>());

            Assert.False(result.IsSuccess);
            Assert.Equal(InvoiceErrors.InvalidInvoiceItems, result.Error);
        }

        [Fact]
        public void PayInvoice_FromNotPayed_ShouldSucceed()
        {
            var invoice = InvoiceFactory.Create().Value;

            var result = invoice.PayInvoice();

            Assert.True(result.IsSuccess);
            Assert.Equal(Result.Updated, result.Value);
            Assert.Equal(InvoiceStatus.Payed, invoice.Status);
            Assert.NotNull(invoice.PaidAt);
        }

        [Fact]
        public void PayInvoice_WhenAlreadyPayed_ShouldFail()
        {
            var invoice = InvoiceFactory.Create().Value;
            invoice.PayInvoice();

            var result = invoice.PayInvoice();

            Assert.False(result.IsSuccess);
            Assert.Equal(InvoiceErrors.InvalidStatusTransition, result.Error);
        }

        [Fact]
        public void RefundInvoice_FromPayed_ShouldSucceed()
        {
            var invoice = InvoiceFactory.Create().Value;
            invoice.PayInvoice();

            var result = invoice.RefundInvoice();

            Assert.True(result.IsSuccess);
            Assert.Equal(Result.Updated, result.Value);
            Assert.Equal(InvoiceStatus.Refunded, invoice.Status);
        }

        [Fact]
        public void RefundInvoice_FromNotPayed_ShouldFail()
        {
            var invoice = InvoiceFactory.Create().Value;

            var result = invoice.RefundInvoice();

            Assert.False(result.IsSuccess);
            Assert.Equal(InvoiceErrors.InvalidStatusTransition, result.Error);
        }

        [Fact]
        public void RefundInvoice_WhenAlreadyRefunded_ShouldFail()
        {
            var invoice = InvoiceFactory.Create().Value;
            invoice.PayInvoice();
            invoice.RefundInvoice();

            var result = invoice.RefundInvoice();

            Assert.False(result.IsSuccess);
            Assert.Equal(InvoiceErrors.InvoiceNotEditable, result.Error);
        }

        [Fact]
        public void PayInvoice_WhenAlreadyRefunded_ShouldFail()
        {
            var invoice = InvoiceFactory.Create().Value;
            invoice.PayInvoice();
            invoice.RefundInvoice();

            var result = invoice.PayInvoice();

            Assert.False(result.IsSuccess);
            Assert.Equal(InvoiceErrors.InvoiceNotEditable, result.Error);
        }
    }
}