using GOATY.Application.Common.Interfaces;
using GOATY.Application.Features.Dashboards.DTOs;
using GOATY.Domain.Common.Results;
using GOATY.Domain.WorkOrders.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GOATY.Application.Features.Dashboards.DashboardQueries
{
    public sealed class GetDashboardQueryHandler(IAppDbContext context)
        : IRequestHandler<GetDashboardQuery, Result<Dashboard>>
    {
        public async Task<Result<Dashboard>> Handle(GetDashboardQuery request, CancellationToken ct)
        {
            var workOrdersQuery = context.WorkOrders
                                           .AsNoTracking()
                                           .Include(wo => wo.Customer)
                                           .Include(wo => wo.Vehicle)
                                           .Include(wo => wo.Employee)
                                           .Include(wo => wo.WorkOrderRepairTasks)
                                                .ThenInclude(wr => wr.RepairTask)
                                                    .ThenInclude(r => r.RepairTaskDetails)
                                                        .ThenInclude(rd => rd.Part)
                                           .AsQueryable();



            var localStart = request.Day.ToDateTime(TimeOnly.MinValue);
            var localEnd = localStart.AddDays(1);

            var utcStart = TimeZoneInfo.ConvertTime(localStart, request.TimeZone);
            var utcEnd = TimeZoneInfo.ConvertTime(localEnd, request.TimeZone);

            var workOrders = await workOrdersQuery.Where(wo => wo.StartTime >= utcStart &&
                                                               wo.EndTime <= utcEnd)
                                                  .ToListAsync(ct);

            var totalOrders = workOrders.Count;

            if(totalOrders == 0)
            {
                return new Dashboard
                {
                    Day = request.Day,
                    TotalOrders = 0,
                    TotalScheduled = 0,
                    TotalInProgress = 0,
                    TotalCompleted = 0,
                    TotalCancelled = 0,
                    TotalRevenue = 0,
                    TotalPartsCost = 0,
                    TotalTechniciansCost = 0,
                    NetProfit = 0,
                    UniqueVehicles = 0,
                    UniqueCustomers = 0,
                    ProfitMargin = 0,
                    CompletionRate = 0,
                    CancellationRate = 0,
                    AvgRevenuPerOrder = 0,
                    OrdersPerVehicle = 0,
                    PartsCostRatio = 0,
                    LaborCostRatio = 0
                };
            }

            var totalScheduled = workOrders.Where(wo => wo.State == State.Scheduled).Count();
            var totalInProgress = workOrders.Where(wo => wo.State == State.InProgress).Count();
            var totalCompleted = workOrders.Where(wo => wo.State == State.Completed).Count();
            var totalCancelled = workOrders.Where(wo => wo.State == State.Cancelled).Count();

            var totalRevenue = workOrders.Where(wo => wo.Invoice != null)
                                         .Sum(wo => wo.Invoice!.Total);

            var totalPartsCost = workOrders.Sum(wo => wo.TotalPartsCost);
            var totalTechniciansCost = workOrders.Sum(wo => wo.TotalTechniciansCost);

            var netProfit = totalRevenue - (totalPartsCost + totalTechniciansCost);

            var uniqueVehicles = workOrders.Select(wo => wo.VehicleId).Distinct().Count();
            var uniqueCustomers = workOrders.Select(wo => wo.CustomerId).Distinct().Count();

            var completionRate = (decimal)totalCompleted / totalOrders * 100;
            var cancellationRate = (decimal)totalCancelled / totalOrders * 100;

            var avgRevenuePerOrder = (decimal)totalRevenue / totalOrders * 100;
            var ordersPerVehicle = (decimal)totalOrders / uniqueVehicles * 100;

            var partsCostRatio = totalRevenue > 0 ? (decimal)totalPartsCost / totalRevenue * 100 : 0;
            var techniciansCostRatio = totalRevenue > 0 ? (decimal)totalTechniciansCost / totalRevenue * 100 : 0;

            var profitMargin = totalRevenue > 0 ? (decimal)netProfit / totalRevenue * 100 : 0;

            return new Dashboard
            {
                Day = request.Day,
                TotalOrders = totalOrders,
                TotalScheduled = totalScheduled,
                TotalInProgress = totalInProgress,
                TotalCompleted = totalCompleted,
                TotalCancelled = totalCancelled,
                TotalRevenue = totalRevenue,
                TotalPartsCost = totalPartsCost,
                TotalTechniciansCost = totalTechniciansCost,
                NetProfit = netProfit,
                UniqueVehicles = uniqueVehicles,
                UniqueCustomers = uniqueCustomers,
                ProfitMargin = profitMargin,
                CompletionRate = completionRate,
                CancellationRate = cancellationRate,
                AvgRevenuPerOrder = avgRevenuePerOrder,
                OrdersPerVehicle = ordersPerVehicle,
                PartsCostRatio = partsCostRatio,
                LaborCostRatio = techniciansCostRatio
            };
        }
    }
}
