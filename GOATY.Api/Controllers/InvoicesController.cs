using GOATY.Application.Features.Billing.Commands.IssueInvoice;
using GOATY.Application.Features.Billing.Commands.RefundInvoice;
using GOATY.Application.Features.Billing.Commands.SettleInvoice;
using GOATY.Application.Features.Billing.Queries.GeneratePdf;
using GOATY.Application.Features.Billing.Queries.GetInvoiceById;
using GOATY.Application.Features.Billing.Queries.GetInvoices;
using GOATY.Contracts.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GOATY.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController(IMediator mediator) : ApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetInvoices(PaginationRequest request)
        {
            var result = await mediator.Send(new GetInvoicesQuery(
                                                request.Page, request.PageSize));

            return result.Match(
                response => Ok(response),
                Problem
                );
        }


        [HttpGet("{invoiceId:guid}")]
        public async Task<IActionResult> GetInvoiceById(Guid invoiceId)
        {
            var result = await mediator.Send(new GetInvoiceByIdQuery(invoiceId));

            return result.Match(
                response => Ok(response),
                Problem
            );
        }

        [HttpGet("pdf/{invoiceId:guid}")]
        public async Task<IActionResult> GetInvoicePdf(Guid invoiceId)
        {
            var result = await mediator.Send(new GeneratePdfQuery(invoiceId));

            return result.Match(
                response => Ok(response),
                Problem
                );
        }

        [HttpPost("{workOrderId:guid}")]
        public async Task<IActionResult> CreateInvoice(Guid workOrderId)
        {
            var result = await mediator.Send(new CreateInvoiceCommand(workOrderId));

            return result.Match(
                response => Ok(response),
                Problem
            );
        }

        [HttpPut("settle/{invoiceId:guid}")]
        public async Task<IActionResult> SettleInvoice(Guid invoiceId)
        {
            var result = await mediator.Send(new SettleInvoiceCommand(invoiceId));

            return result.Match(
                response => NoContent(),
                Problem
            );
        }

        [HttpPut("refund/{invoiceId:guid}")]
        public async Task<IActionResult> RefundInvoice(Guid invoiceId)
        {
            var result = await mediator.Send(new RefundInvoiceCommand(invoiceId));

            return result.Match(
                response => NoContent(),
                Problem
            );
        }
    }
}
