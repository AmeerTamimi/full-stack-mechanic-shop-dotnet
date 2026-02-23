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
        [Authorize]
        [HttpGet("who")]
        public ActionResult Who()
        {
            return Ok(new
            {
                isAuth = User.Identity?.IsAuthenticated,
                name = User.Identity?.Name,
                claims = User.Claims.Select(c => new { c.Type, c.Value })
            });
        }

        [HttpPost]
        public async Task<IActionResult> GenerateToken([FromBody] GenerateTokenQuery request)
        {
            var result = await mediator.Send(new GenerateTokenQuery(request.Email, request.Password));

            return result.Match(
                response => Ok(response),
                Problem
            );
        }
    }
}
