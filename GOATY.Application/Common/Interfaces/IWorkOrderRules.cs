using GOATY.Domain.Common.Results;
using GOATY.Domain.WorkOrders.Enums;

namespace GOATY.Application.Common.Interfaces
{
    public interface IWorkOrderRules
    {
        bool IsWorkOrderOccupied(Guid id);
        bool IsTechnicianOccupied(Guid id , DateTime startTime , DateTime endTime);
        bool IsBayOccupied(Bay bay, DateTime startTime, DateTime endTime);
        bool IsVehicleOccupied(Guid id, DateTime startTime, DateTime endTime);
    }
}
