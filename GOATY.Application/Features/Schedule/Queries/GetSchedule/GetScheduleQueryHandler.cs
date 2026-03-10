using GOATY.Application.Common.Interfaces;
using GOATY.Application.Features.Employees.Mapping;
using GOATY.Application.Features.Schedule.DTOs;
using GOATY.Domain.Common.Results;
using GOATY.Domain.Customers.Vehicles;
using GOATY.Domain.WorkOrders.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GOATY.Application.Features.Schedule.Queries.GetSchedule
{
    public sealed class GetScheduleQueryHandler(
        IAppDbContext context,
        ILogger<GetScheduleQueryHandler> logger,
        TimeProvider _datetime)
        : IRequestHandler<GetScheduleQuery, Result<ScheduleDto>>
    {
        public async Task<Result<ScheduleDto>> Handle(GetScheduleQuery request, CancellationToken ct)
        {
            var day = request.Day == default ? DateOnly.FromDateTime(DateTime.UtcNow) : request.Day;
            
            var localStart = day.ToDateTime(TimeOnly.MinValue);
            var localEnd = localStart.AddDays(1);

            var timeZone = request.TimeZone == default ? TimeZoneInfo.Local : request.TimeZone;

            var utcStart = TimeZoneInfo.ConvertTimeToUtc(localStart, timeZone);
            var utcEnd = TimeZoneInfo.ConvertTimeToUtc(localEnd, timeZone);


            var workOrders = await context.WorkOrders
                .Where(wo => wo.EndTime >= utcStart &&
                             wo.StartTime < utcEnd &&
                             (request.EmployeeID == null || wo.EmployeeId == request.EmployeeID))
                .Include(wo => wo.WorkOrderRepairTasks)
                .ToListAsync(ct);

            var now = TimeZoneInfo.ConvertTime(_datetime.GetUtcNow(), timeZone);

            var result = new ScheduleDto
            {
                Date = day,
                Bays = []
            };

            var baysInfo = new List<BayDto>();

            foreach(var bay in Enum.GetValues<Bay>())
            {
                var slotsInfo = new List<AvailabilityBayDto>();
                var currentTime = localStart;

                var workOrderForBay = workOrders.Where(wo => wo.Bay == bay)
                                                .OrderBy(wo => wo.StartTime)
                                                .ToList( );

                while(currentTime < utcEnd)
                {
                    var next = currentTime.AddMinutes(15); // slot time or chunk

                    var startUtc = TimeZoneInfo.ConvertTimeToUtc(currentTime,timeZone);
                    var endUtc = TimeZoneInfo.ConvertTimeToUtc(next, timeZone);

                    var workOrder = workOrderForBay.FirstOrDefault
                                    (wo => wo.EndTime > startUtc &&
                                           wo.StartTime < endUtc);

                    if (workOrder is not null)
                    {
                        if(!slotsInfo.Any(a => a.WorkOrderId == workOrder.Id))
                        {
                            slotsInfo.Add(new AvailabilityBayDto
                            {
                                WorkOrderId = workOrder.Id,
                                Bay = bay,
                                StartAt = currentTime,
                                EndAt = next,
                                Vehicle = FormatVehicleInfo(workOrder.Vehicle!),
                                Employee = workOrder.Employee!.ToDto(),
                                IsOccupied = true,
                                IsAvailable = false,
                                WorkOrderLocked = true,
                                State = workOrder.State,
                                WorkOrderRepairTasks = workOrder.WorkOrderRepairTasks.ToList()
                            });
                        }
                    }
                    else
                    {
                        slotsInfo.Add(new AvailabilityBayDto
                        {
                            Bay = bay,
                            StartAt = startUtc,
                            EndAt = endUtc,
                            IsOccupied = false,
                            IsAvailable = true
                        });
                    }
                    currentTime = next;
                }
                result.Bays.Add(new BayDto
                {
                    Bay = bay,
                    Slots = slotsInfo
                });
            }
            return result;
        }
        private static string? FormatVehicleInfo(Vehicle vehicle) =>
        vehicle != null ? $"{vehicle.Brand} | {vehicle.LicensePlate}" : null;
    }
}
