using Azure.Core;
using GOATY.Application.Features.Identity.DTOs;
using GOATY.Application.Features.Identity.GenerateByRefreshTokenQueries;
using GOATY.Application.Features.Identity.GenerateTokenQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GOATY.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController(IMediator mediator) : ApiController

    {
        [HttpPost("token/generate")]
        [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Generates an access and refresh token for a valid user.")]
        [EndpointDescription("Authenticates a user using provided credentials and returns a JWT token pair.")]
        [EndpointName("GenerateToken")]
        public async Task<IActionResult> GenerateToken([FromBody] GenerateTokenQuery request)
        {
            var result = await mediator.Send(new GenerateTokenQuery(request.Email, request.Password));

            return result.Match(
                response => Ok(response),
                Problem
            );
        }

        [HttpPost("token/refresh-token")]
        [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Refreshes access token using a valid refresh token.")]
        [EndpointDescription("Exchanges an expired access token and a valid refresh token for a new token pair.")]
        [EndpointName("RefreshToken")]
        public async Task<IActionResult> GenerateTokenFromRefreshToken(GenerateByRefreshTokenQuery request)
        {
            var result = await mediator.Send(new GenerateByRefreshTokenQuery(request.Token));

            return result.Match(
                response => Ok(response),
                Problem
            );
        }
    }
}
