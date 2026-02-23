using Azure.Core;
using GOATY.Application.Features.Identity.GenerateFromRefreshTokenQueries;
using GOATY.Application.Features.Identity.GenerateTokenQueries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GOATY.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController(IMediator mediator) : ApiController

    {
        [HttpPost]
        public async Task<IActionResult> GenerateToken([FromBody] GenerateTokenQuery request)
        {
            var result = await mediator.Send(new GenerateTokenQuery(request.Email, request.Password));

            return result.Match(
                response => Ok(response),
                Problem
            );
        }

        [HttpPost("{refreshToken}")]
        public async Task<IActionResult> GenerateTokenFromRefreshToken(string refreshToken)
        {
            var result = await mediator.Send(new GenerateFromRefreshTokenQuery(refreshToken));

            return result.Match(
                response => Ok(response),
                Problem
            );
        }
    }
}
