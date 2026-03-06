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
    }
}
