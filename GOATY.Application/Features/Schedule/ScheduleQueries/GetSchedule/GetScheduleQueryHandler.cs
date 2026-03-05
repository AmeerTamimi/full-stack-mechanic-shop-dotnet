using GOATY.Application.Common;
using GOATY.Application.Common.Interfaces;
using GOATY.Application.Features.WorkOrders.DTOs;
using GOATY.Application.Features.WorkOrders.Mappers;
using GOATY.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GOATY.Application.Features.Schedule.ScheduleQueries.GetSchedule
{
    public sealed class GetScheduleQueryHandler(IAppDbContext context, ILogger<GetScheduleQueryHandler> logger)
        : IRequestHandler<GetScheduleQuery, Result<List<WorkOrderDto>>>

    {
        public async Task<Result<List<WorkOrderDto>>> Handle(GetScheduleQuery request, CancellationToken ct)
        {
            var day = request.Day == default
                        ? DateOnly.FromDateTime(DateTime.Today)
                        : request.Day;

            var start = day.ToDateTime(TimeOnly.MinValue);   
            var end = start.AddDays(1);

            var workOrders = await context.WorkOrders
                .Where(wo => wo.StartTime >= start && wo.StartTime < end)
                .ToListAsync(ct);

            if(workOrders.Count == 0)
            {
                return ApplicationErrors.NoWorkOrders;
            }

            return workOrders.ToDtos();
        }
    }
}
