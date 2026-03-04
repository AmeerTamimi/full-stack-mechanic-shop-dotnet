using GOATY.Domain.Common.Results;
using GOATY.Domain.WorkOrders.Enums;

namespace GOATY.Application.Common.Interfaces
{
    public interface IWorkOrderRules
    {
        Task<bool> IsTechnicianOccupied(Guid id, DateTime startTime, DateTime endTime, CancellationToken ct);
        Task<bool> IsBayOccupied(Bay bay, DateTime startTime, DateTime endTime, CancellationToken ct);
        Task<bool> IsVehicleOccupied(Guid id, DateTime startTime, DateTime endTime, CancellationToken ct);
        Task<bool> IsCustomerHasVehicle(Guid customerId, Guid vehicleId, CancellationToken ct); 
    }
}
