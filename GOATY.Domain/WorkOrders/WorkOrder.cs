using GOATY.Domain.Common;
using GOATY.Domain.Common.Enums;
using GOATY.Domain.Common.Results;
using GOATY.Domain.Customers;
using GOATY.Domain.Customers.Vehicles;
using GOATY.Domain.Employees;
using GOATY.Domain.RepairTasks;
using GOATY.Domain.WorkOrders.Enums;

namespace GOATY.Domain.WorkOrders
{
    public sealed class WorkOrder : AuditableEntity
    {
        public State State { get; private set; }
        public int TotalTime { get; private set; }
        public decimal TotalCost { get; private set; }
        public DateTime StartTime { get; private set; }
        public Guid VehicleId { get; private set; }
        public Guid CustomerId { get; private set; }
        public Guid EmployeeId { get; private set; }
        public Vehicle? Vehicle { get; set; }
        public Customer? Customer { get; set; }
        public Employee? Employee { get; set; }

        private readonly List<WorkOrderRepairTasks> _repairTasks = [];
        public IEnumerable<WorkOrderRepairTasks> WorkOrderRepairTasks => _repairTasks;

        private WorkOrder() { }
        private WorkOrder(Guid id,
                          Guid vehicleId,
                          Guid customerId,
                          Guid employeeId,
                          int totalTime,
                          decimal totalCost,
                          DateTime startTime,
                          List<WorkOrderRepairTasks> repairTasks)
                          : base(id)
        {
            VehicleId = vehicleId;
            CustomerId = customerId;
            EmployeeId = employeeId;
            StartTime = startTime;
            State = State.Scheduled;
            TotalTime = totalTime;
            TotalCost = totalCost;
            _repairTasks = repairTasks;
        }

        public static Result<WorkOrder> Create(Guid id,
                                               Guid vehicleId,
                                               Guid customerId,
                                               Guid employeeId,
                                               DateTime startTime,
                                               List<WorkOrderRepairTasks> repairTasks)
        {
            if (Guid.Empty == id)
            {
                return WorkOrderErrors.InvalidId;
            }
            if (Guid.Empty == vehicleId)
            {
                return WorkOrderErrors.InvalidVehicleId;
            }
            if (Guid.Empty == customerId)
            {
                return WorkOrderErrors.InvalidCustomerId;
            }
            if (Guid.Empty == employeeId)
            {
                return WorkOrderErrors.InvalidTechnicianId;
            }
            if(DateTime.Now > startTime)
            {
                return WorkOrderErrors.InvalidStartTime;
            }
            if(repairTasks is null || repairTasks.Count == 0)
            {
                return WorkOrderErrors.InvalidRepairTasks;
            }

            var totalTime = CalculateTotalTime(repairTasks);
            var totalCost = CalculateTotalCost(repairTasks);

            return new WorkOrder(id, vehicleId, customerId, employeeId, totalTime, totalCost, startTime, repairTasks);
        }

        private static int CalculateTotalTime(List<WorkOrderRepairTasks> repairTasks)
        {
            var totalTime = 0;

            foreach(var task in repairTasks)
            {
                totalTime += (int)task.Time;
            }

            return totalTime;
        }
        private static decimal CalculateTotalCost(List<WorkOrderRepairTasks> repairTasks)
        {
            var totalCost = 0m;

            foreach (var task in repairTasks)
            {
                totalCost += task.Cost;
            }

            return totalCost;
        }
    }
}
