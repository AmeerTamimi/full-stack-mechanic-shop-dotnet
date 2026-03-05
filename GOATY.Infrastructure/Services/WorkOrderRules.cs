using GOATY.Application.Common.Interfaces;
using GOATY.Domain.WorkOrders.Enums;
using Microsoft.EntityFrameworkCore;

namespace GOATY.Infrastructure.Services
{
    public sealed class WorkOrderRules : IWorkOrderRules
    {
        private readonly IAppDbContext _context;
        public WorkOrderRules(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsBayOccupied(Bay bay, DateTimeOffset startTime, DateTimeOffset endTime, CancellationToken ct)
        {
            return await _context.WorkOrders
                            .AnyAsync(wo => wo.Bay == bay &&
                                            wo.StartTime < endTime &&
                                            wo.EndTime > startTime,
                                            ct);
        }

        public async Task<bool> IsCustomerHasVehicle(Guid customerId, Guid vehicleId, CancellationToken ct)
        {
            return await _context.Vehicles
                        .AnyAsync(v => v.CustomerId == customerId &&
                                       v.Id == vehicleId,
                                       ct);
        }

        public async Task<bool> IsTechnicianOccupied(Guid employeeId, DateTimeOffset startTime, DateTimeOffset endTime, CancellationToken ct)
        {
            return await _context.WorkOrders
                            .AnyAsync(wo => wo.EmployeeId == employeeId &&
                                wo.StartTime < endTime &&
                                wo.EndTime > startTime, ct);
        }

        public async Task<bool> IsVehicleOccupied(Guid vehicleId, DateTimeOffset startTime, DateTimeOffset endTime, CancellationToken ct)
        {
            var conflictedVehicle = await _context.WorkOrders
                                            .AnyAsync(wo => wo.VehicleId == vehicleId &&
                                                wo.State == State.InProgress,
                                                ct);

            var overlappedVehicle = await _context.WorkOrders
                                            .AnyAsync(wo => wo.State == State.Scheduled &&
                                                wo.VehicleId == vehicleId &&
                                                endTime > wo.StartTime &&
                                                startTime < wo.EndTime,
                                                ct);

            return !conflictedVehicle && !overlappedVehicle;
        }
    }
}
