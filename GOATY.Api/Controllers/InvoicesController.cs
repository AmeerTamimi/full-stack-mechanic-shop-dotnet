using GOATY.Application.Features.Billing.Commands.IssueInvoice;
using GOATY.Application.Features.Billing.Commands.RefundInvoice;
using GOATY.Application.Features.Billing.Commands.SettleInvoice;
using GOATY.Application.Features.Billing.Queries.GeneratePdf;
using GOATY.Application.Features.Billing.Queries.GetInvoiceById;
using GOATY.Application.Features.Billing.Queries.GetInvoices;
using GOATY.Contracts.Requests;
using GOATY.Domain.Employees.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GOATY.Api.Controllers
{
    [Authorize(Roles = nameof(Role.Manager))]
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController(IMediator mediator) : ApiController
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("GetInvoices")]
        [EndpointSummary("Retrieves a paginated list of invoices.")]
        [EndpointDescription("Returns a paginated list of invoices in the system. Only users with the Manager role are authorized to access this resource.")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetInvoices([FromQuery] PaginationRequest request)
        {
            var result = await mediator.Send(new GetInvoicesQuery(
                request.Page, request.PageSize));

            return result.Match(
                response => Ok(response),
                Problem
            );
        }

        [HttpGet("{invoiceId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("GetInvoiceById")]
        [EndpointSummary("Retrieves an invoice by its ID.")]
        [EndpointDescription("Returns detailed information about the specified invoice if it exists.")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetInvoiceById(Guid invoiceId)
        {
            var result = await mediator.Send(new GetInvoiceByIdQuery(invoiceId));

            return result.Match(
                response => Ok(response),
                Problem
            );
        }

        [HttpGet("pdf/{invoiceId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("GetInvoicePdf")]
        [EndpointSummary("Generates the PDF representation of an invoice.")]
        [EndpointDescription("Returns the PDF data for the specified invoice if it exists.")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetInvoicePdf(Guid invoiceId)
        {
            var result = await mediator.Send(new GeneratePdfQuery(invoiceId));

            return result.Match(
                response => Ok(response),
                Problem
            );
        }

        [HttpPost("{workOrderId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("CreateInvoice")]
        [EndpointSummary("Creates an invoice for a work order.")]
        [EndpointDescription("Issues a new invoice for the specified work order. Only users with the Manager role are authorized to perform this operation.")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> CreateInvoice(Guid workOrderId)
        {
            var result = await mediator.Send(new CreateInvoiceCommand(workOrderId));

            return result.Match(
                response => Ok(response),
                Problem
            );
        }

        [HttpPut("{invoiceId:guid}/settle")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("SettleInvoice")]
        [EndpointSummary("Marks an invoice as settled.")]
        [EndpointDescription("Updates the specified invoice to a settled state. Only users with the Manager role are authorized to perform this operation.")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> SettleInvoice(Guid invoiceId)
        {
            var result = await mediator.Send(new SettleInvoiceCommand(invoiceId));

            return result.Match(
                response => NoContent(),
                Problem
            );
        }

        [HttpPut("{invoiceId:guid}/refund")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("RefundInvoice")]
        [EndpointSummary("Marks an invoice as refunded.")]
        [EndpointDescription("Updates the specified invoice to a refunded state. Only users with the Manager role are authorized to perform this operation.")]
        [MapToApiVersion("1.0")]
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