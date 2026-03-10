using GOATY.Application.Features.Dashboards.DashboardQueries;
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
    public class DashboardController(IMediator mediator) : ApiController
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("GetDashboard")]
        [EndpointSummary("Retrieves dashboard data for a specific day.")]
        [EndpointDescription("Returns dashboard metrics for the specified day and time zone. If no day is provided, today's UTC date is used. Only users with the Manager role are authorized to access this resource.")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetDashboard([FromQuery] DashboardRequest request)
        {
            var result = await mediator.Send(new GetDashboardQuery(
                request.Day == default ? DateOnly.FromDateTime(DateTime.UtcNow) : request.Day,
                TimeZoneInfo.FindSystemTimeZoneById(request.TimeZone)));

            return result.Match(
                response => Ok(response),
                Problem
            );
        }
    }
}