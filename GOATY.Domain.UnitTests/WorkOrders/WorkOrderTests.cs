using GOATY.Domain.Common;
using GOATY.Domain.Common.Results;
using GOATY.Domain.Customers;
using GOATY.Domain.Customers.Vehicles;
using GOATY.Domain.Employees;
using GOATY.Domain.WorkOrders.Billing;
using GOATY.Domain.WorkOrders.Enums;

namespace GOATY.Domain.UnitTests.WorkOrders
{
    public sealed class WorkOrderTests
    {
        //[Fact]
        //public void 
    }
}
//{
//    public sealed class WorkOrder : AuditableEntity
//    {
//        public State State { get; private set; }
//        public DateTimeOffset StartTime { get; private set; }
//        public DateTimeOffset EndTime { get; private set; }
//        public decimal Discount { get; set; }
//        public Bay Bay { get; private set; }
//        public Invoice? Invoice { get; set; }
//        public Guid VehicleId { get; private set; }
//        public Guid CustomerId { get; private set; }
//        public Guid? EmployeeId { get; private set; }
//        public Vehicle Vehicle { get; set; }
//        public Customer Customer { get; set; }
//        public Employee? Employee { get; set; }
//        public int TotalTime => _workOrderRepairTasks.Sum(wr => (int)wr.RepairTask.TimeEstimated);
//        public decimal TotalCost => _workOrderRepairTasks.Sum(wr => wr.RepairTask.CostEstimated);
//        public decimal TotalPartsCost => _workOrderRepairTasks.Select(wr => wr.RepairTask).SelectMany(r => r.RepairTaskDetails).Sum(rd => rd.UnitPrice);
//        public decimal TotalTechniciansCost => _workOrderRepairTasks.Select(wr => wr.RepairTask).Sum(r => r.TechnicianCost);

//        private readonly List<WorkOrderRepairTasks> _workOrderRepairTasks = [];
//        public IReadOnlyCollection<WorkOrderRepairTasks> WorkOrderRepairTasks => _workOrderRepairTasks;

//        public bool IsEditable => State == State.Scheduled;

//        private WorkOrder() { }
//        private WorkOrder(Guid id,
//                          Guid vehicleId,
//                          Guid customerId,
//                          Guid employeeId,
//                          Bay bay,
//                          DateTime startTime,
//                          decimal discount,
//                          List<WorkOrderRepairTasks> repairTasks)
//                          : base(id)
//        {
//            VehicleId = vehicleId;
//            CustomerId = customerId;
//            EmployeeId = employeeId;
//            StartTime = startTime;
//            EndTime = startTime.AddMinutes(TotalTime);
//            Bay = bay;
//            Discount = discount;
//            State = State.Scheduled;
//            _workOrderRepairTasks = repairTasks;
//        }

//        public static Result<WorkOrder> Create(Guid id,
//                                               Guid vehicleId,
//                                               Guid customerId,
//                                               Guid employeeId,
//                                               DateTime startTime,
//                                               Bay bay,
//                                               decimal discount,
//                                               List<WorkOrderRepairTasks> repairTasks)
//        {
//            if (Guid.Empty == id)
//            {
//                return WorkOrderErrors.InvalidId;
//            }
//            if (Guid.Empty == vehicleId)
//            {
//                return WorkOrderErrors.InvalidVehicleId;
//            }
//            if (Guid.Empty == customerId)
//            {
//                return WorkOrderErrors.InvalidCustomerId;
//            }
//            if (Guid.Empty == employeeId)
//            {
//                return WorkOrderErrors.InvalidTechnicianId;
//            }
//            if (DateTime.Now > startTime)
//            {
//                return WorkOrderErrors.InvalidStartTime;
//            }
//            if (discount < 0 || discount > 100)
//            {
//                return WorkOrderErrors.InvalidDiscount;
//            }
//            if (repairTasks is null || repairTasks.Count == 0)
//            {
//                return WorkOrderErrors.InvalidRepairTasks;
//            }

//            return new WorkOrder(id, vehicleId, customerId, employeeId, bay, startTime, discount, repairTasks);
//        }

//        public Result<Updated> Update(Guid vehicleId,
//                                      Guid customerId,
//                                      Guid employeeId,
//                                      DateTime startTime,
//                                      Bay bay,
//                                      decimal discount,
//                                      List<WorkOrderRepairTasks> repairTasks)
//        {
//            if (Guid.Empty == vehicleId)
//            {
//                return WorkOrderErrors.InvalidVehicleId;
//            }
//            if (Guid.Empty == customerId)
//            {
//                return WorkOrderErrors.InvalidCustomerId;
//            }
//            if (Guid.Empty == employeeId)
//            {
//                return WorkOrderErrors.InvalidTechnicianId;
//            }
//            if (DateTime.Now > startTime)
//            {
//                return WorkOrderErrors.InvalidStartTime;
//            }
//            if (discount < 0 || discount > 100)
//            {
//                return WorkOrderErrors.InvalidDiscount;
//            }
//            if (repairTasks is null || repairTasks.Count == 0)
//            {
//                return WorkOrderErrors.InvalidRepairTasks;
//            }

//            VehicleId = vehicleId;
//            CustomerId = customerId;
//            EmployeeId = employeeId;
//            StartTime = startTime;
//            EndTime = startTime.AddMinutes(TotalTime);
//            Bay = bay;
//            Discount = discount;

//            return Result.Updated;
//        }

//        public Result<Updated> UpsertRepairTasks(List<WorkOrderRepairTasks> incomings)
//        {
//            _workOrderRepairTasks.RemoveAll(exists => !incomings.Contains(exists));

//            foreach (var incoming in incomings)
//            {
//                if (!_workOrderRepairTasks.Contains(incoming))
//                {
//                    var createResult = WorkOrders.WorkOrderRepairTasks.Create(Id,
//                                                                              incoming.RepairTaskId,
//                                                                              incoming.Time,
//                                                                              incoming.Cost,
//                                                                              incoming.Quantity);
//                    if (!createResult.IsSuccess)
//                    {
//                        return createResult.Errors;
//                    }
//                    _workOrderRepairTasks.Add(createResult.Value);
//                }
//                else
//                {
//                    var updateResult = incoming.Update(incoming.Time, incoming.Cost, incoming.Quantity);

//                    if (!updateResult.IsSuccess)
//                    {
//                        return updateResult.Errors;
//                    }
//                }
//            }
//            return Result.Updated;
//        }

//        public Result<Updated> UpdateState(State newState)
//        {
//            if ((State == State.Scheduled && newState == State.Completed) ||
//                (State == State.InProgress && newState == State.Scheduled) ||
//                State == State.Completed ||
//                State == State.Cancelled)
//            {
//                return WorkOrderErrors.InvalidWorkOrderStateTransition;
//            }

//            State = newState;

//            return Result.Updated;
//        }

//        public Result<Updated> Relocate(Bay newBay, DateTime newStartTime)
//        {
//            if (!IsEditable)
//            {
//                return WorkOrderErrors.NotEditable;
//            }

//            if (DateTime.Now < newStartTime)
//            {
//                return WorkOrderErrors.InvalidStartTime;
//            }
//            if (!Enum.IsDefined(typeof(Bay), newBay))
//            {
//                return WorkOrderErrors.InvalidBay;
//            }

//            Bay = newBay;
//            StartTime = newStartTime;
//            EndTime = StartTime.AddMinutes(TotalTime);

//            return Result.Updated;
//        }
//        public Result<Updated> UpdateVehicle(Guid vehicleId)
//        {
//            if (!IsEditable)
//            {
//                return WorkOrderErrors.NotEditable;
//            }

//            if (vehicleId == Guid.Empty)
//            {
//                return WorkOrderErrors.InvalidTechnicianId;
//            }

//            VehicleId = vehicleId;

//            return Result.Updated;
//        }
//        public Result<Updated> UpdateTechnician(Guid technicianId)
//        {
//            if (!IsEditable)
//            {
//                return WorkOrderErrors.NotEditable;
//            }

//            if (technicianId == Guid.Empty)
//            {
//                return WorkOrderErrors.InvalidTechnicianId;
//            }

//            EmployeeId = technicianId;

//            return Result.Updated;
//        }
//    }
//}
