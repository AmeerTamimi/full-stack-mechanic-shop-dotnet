using GOATY.Application.Queries.PartsQueries.GetPartByIdQuery;
using GOATY.Application.Queries.PartsQueries.GetPartsQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GOATY.Api.Controllers
{
    [Route("api/parts")]
    [ApiController]
    public class PartsController(IMediator mediator) : ApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetParts()
        {
            var result = await mediator.Send(new GetPartsQuery());
            return result.Match<IActionResult>(
                response => Ok(response),
                errors => BadRequest(errors)
            );
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetPartById(Guid id)
        {
            var result = await mediator.Send(new GetPartByIdQuery(id));
            return result.Match<IActionResult>(
                response => Ok(response),
                Problem
                );
        }
    }
}
