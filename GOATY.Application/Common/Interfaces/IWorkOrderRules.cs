using GOATY.Domain.Common.Results;
using GOATY.Domain.WorkOrders.Enums;

namespace GOATY.Application.Common.Interfaces
{
    public interface IWorkOrderRules
    {
        Task<bool> IsTechnicianOccupied(Guid id, DateTimeOffset startTime, DateTimeOffset endTime, CancellationToken ct);
        Task<bool> IsBayOccupied(Bay bay, DateTimeOffset startTime, DateTimeOffset endTime, CancellationToken ct);
        Task<bool> IsVehicleOccupied(Guid id, DateTimeOffset startTime, DateTimeOffset endTime, CancellationToken ct);
        Task<bool> IsCustomerHasVehicle(Guid customerId, Guid vehicleId, CancellationToken ct); 
    }
}
