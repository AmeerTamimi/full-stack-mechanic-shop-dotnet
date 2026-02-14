using GOATY.Application.Commands.PartsCommands.CreatePartCommands;
using GOATY.Application.Commands.PartsCommands.DeletePartCommands;
using GOATY.Application.Commands.PartsCommands.UpdatePartCommands;
using GOATY.Application.DTOs;
using GOATY.Application.Queries.PartsQueries.GetPartByIdQuery;
using GOATY.Application.Queries.PartsQueries.GetPartsQuery;
using GOATY.Contracts.Requests;
using GOATY.Domain.Parts;
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
                Problem
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

        [HttpPost]
        public async Task<IActionResult> AddPart([FromBody] PartRequest part)
        {
            var result = await mediator.Send(new CreatePartCommand(part.Name! , part.Cost , part.Quantity));

            return result.Match(
                response => Ok(response),
                Problem
                );
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdatePart(Guid id , [FromBody] PartRequest part)
        {
            var result = await mediator.Send(new UpdatePartCommand(id , part.Name!, part.Cost, part.Quantity));

            return result.Match(
                response => NoContent(),
                Problem
                );
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeletePart(Guid id)
        {
            var result = await mediator.Send(new DeletePartCommand(id));

            return result.Match(
                response => Ok(response),
                Problem
                );
        }
    }
}
