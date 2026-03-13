using GOATY.Domain.Common.Enums;
using GOATY.Domain.Common.Results;
using GOATY.Domain.WorkOrders;
using GOATY.Domain.WorkOrders.Enums;
using GOATY.Tests.Common.WorkOrders;

namespace GOATY.Domain.UnitTests.WorkOrders
{
    public sealed class WorkOrderTests
    {
        [Fact]
        public void Create_WithValidData_ShouldSucceed()
        {
            var workOrder = WorkOrderFactory.Create();

            Assert.True(workOrder.IsSuccess);
        }
        [Fact]
        public void Create_WithInvalidId_ShouldFail()
        {
            var workOrder = WorkOrderFactory.Create(id:Guid.Empty);

            Assert.False(workOrder.IsSuccess);
            Assert.Equal(WorkOrderErrors.InvalidId, workOrder.Error);
        }

        [Fact]
        public void Create_WithInvalidVehicleId_ShouldFail()
        {
            var workOrder = WorkOrderFactory.Create(vehicleId: Guid.Empty);

            Assert.False(workOrder.IsSuccess);
            Assert.Equal(WorkOrderErrors.InvalidVehicleId, workOrder.Error);
        }

        [Fact]
        public void Create_WithInvalidCustomerId_ShouldFail()
        {
            var workOrder = WorkOrderFactory.Create(customerId: Guid.Empty);

            Assert.False(workOrder.IsSuccess);
            Assert.Equal(WorkOrderErrors.InvalidCustomerId, workOrder.Error);
        }

        [Fact]
        public void Create_WithInvalidEmployeeId_ShouldFail()
        {
            var workOrder = WorkOrderFactory.Create(employeeId: Guid.Empty);

            Assert.False(workOrder.IsSuccess);
            Assert.Equal(WorkOrderErrors.InvalidTechnicianId, workOrder.Error);
        }

        [Fact]
        public void Create_WithPastStartTime_ShouldFail()
        {
            var workOrder = WorkOrderFactory.Create(startTime: DateTime.UtcNow.AddMinutes(-5));

            Assert.False(workOrder.IsSuccess);
            Assert.Equal(WorkOrderErrors.InvalidStartTime, workOrder.Error);
        }
        [Theory]
        [InlineData(-1)]
        [InlineData(101)]
        public void Create_WithInvalidDiscount_ShouldFail(decimal discount)
        {
            var workOrder = WorkOrderFactory.Create(discount: discount);

            Assert.False(workOrder.IsSuccess);
            Assert.Equal(WorkOrderErrors.InvalidDiscount, workOrder.Error);
        }

        [Fact]
        public void Update_WithValidData_ShouldSucceed()
        {
            var workOrder = WorkOrderFactory.Create().Value;

            var updateResult = workOrder.Update(vehicleId: Guid.NewGuid(),
                                                customerId: Guid.NewGuid(),
                                                employeeId: Guid.NewGuid(),
                                                startTime: DateTime.UtcNow.AddMinutes(5),
                                                bay: Bay.A,
                                                discount: 50,
                                                repairTasks: [WorkOrderRepairTaskFactory.Create().Value]);

            Assert.True(updateResult.IsSuccess);
        }

        [Fact]
        public void UpdateVehicle_WithValidVehicleId_ShouldSucceed()
        {
            var workOrder = WorkOrderFactory.Create().Value;
            var newVehicleId = Guid.NewGuid();

            var result = workOrder.UpdateVehicle(newVehicleId);

            Assert.True(result.IsSuccess);
            Assert.Equal(Result.Updated, result.Value);
            Assert.Equal(newVehicleId, workOrder.VehicleId);
        }

        [Fact]
        public void UpdateVehicle_WithInvalidVehicleId_ShouldFail()
        {
            var workOrder = WorkOrderFactory.Create().Value;

            var result = workOrder.UpdateVehicle(Guid.Empty);

            Assert.False(result.IsSuccess);
            Assert.Equal(WorkOrderErrors.InvalidVehicleId, result.Error);
        }

        [Fact]
        public void UpdateVehicle_WhenWorkOrderIsNotEditable_ShouldFail()
        {
            var workOrder = WorkOrderFactory.Create().Value;
            workOrder.UpdateState(State.InProgress);

            var result = workOrder.UpdateVehicle(Guid.NewGuid());

            Assert.False(result.IsSuccess);
            Assert.Equal(WorkOrderErrors.NotEditable, result.Error);
        }

        [Fact]
        public void UpdateTechnician_WithValidTechnicianId_ShouldSucceed()
        {
            var workOrder = WorkOrderFactory.Create().Value;
            var newTechnicianId = Guid.NewGuid();

            var result = workOrder.UpdateTechnician(newTechnicianId);

            Assert.True(result.IsSuccess);
            Assert.Equal(Result.Updated, result.Value);
            Assert.Equal(newTechnicianId, workOrder.EmployeeId);
        }

        [Fact]
        public void UpdateTechnician_WithInvalidTechnicianId_ShouldFail()
        {
            var workOrder = WorkOrderFactory.Create().Value;

            var result = workOrder.UpdateTechnician(Guid.Empty);

            Assert.False(result.IsSuccess);
            Assert.Equal(WorkOrderErrors.InvalidTechnicianId, result.Error);
        }

        [Fact]
        public void UpdateTechnician_WhenWorkOrderIsNotEditable_ShouldFail()
        {
            var workOrder = WorkOrderFactory.Create().Value;
            workOrder.UpdateState(State.InProgress);

            var result = workOrder.UpdateTechnician(Guid.NewGuid());

            Assert.False(result.IsSuccess);
            Assert.Equal(WorkOrderErrors.NotEditable, result.Error);
        }

        [Fact]
        public void Relocate_WithValidData_ShouldSucceed()
        {
            var workOrder = WorkOrderFactory.Create().Value;
            var newBay = Bay.B;

            var newStartTime = DateTime.Now.AddMinutes(5);

            var result = workOrder.Relocate(newBay, newStartTime);

            Assert.True(result.IsSuccess);
            Assert.Equal(Result.Updated, result.Value);
            Assert.Equal(newBay, workOrder.Bay);
            Assert.Equal(newStartTime, workOrder.StartTime);
        }

        [Fact]
        public void Relocate_WhenWorkOrderIsNotEditable_ShouldFail()
        {
            var workOrder = WorkOrderFactory.Create().Value;
            workOrder.UpdateState(State.InProgress);

            var result = workOrder.Relocate(Bay.B, DateTime.Now.AddMinutes(-5));

            Assert.False(result.IsSuccess);
            Assert.Equal(WorkOrderErrors.NotEditable, result.Error);
        }

        [Fact]
        public void Relocate_WithInvalidStartTime_ShouldFail()
        {
            var workOrder = WorkOrderFactory.Create().Value;

            var result = workOrder.Relocate(Bay.B, DateTime.Now.AddMinutes(-5));

            Assert.False(result.IsSuccess);
            Assert.Equal(WorkOrderErrors.InvalidStartTime, result.Error);
        }

        [Fact]
        public void Relocate_WithInvalidBay_ShouldFail()
        {
            var workOrder = WorkOrderFactory.Create().Value;
            var invalidBay = (Bay)999;

            var result = workOrder.Relocate(invalidBay, DateTime.Now.AddMinutes(5));

            Assert.False(result.IsSuccess);
            Assert.Equal(WorkOrderErrors.InvalidBay, result.Error);
        }

        [Fact]
        public void UpdateState_FromScheduledToInProgress_ShouldSucceed()
        {
            var workOrder = WorkOrderFactory.Create().Value;

            var result = workOrder.UpdateState(State.InProgress);

            Assert.True(result.IsSuccess);
            Assert.Equal(Result.Updated, result.Value);
            Assert.Equal(State.InProgress, workOrder.State);
        }

        [Fact]
        public void UpdateState_FromScheduledToCompleted_ShouldFail()
        {
            var workOrder = WorkOrderFactory.Create().Value;

            var result = workOrder.UpdateState(State.Completed);

            Assert.False(result.IsSuccess);
            Assert.Equal(WorkOrderErrors.InvalidWorkOrderStateTransition, result.Error);
        }

        [Fact]
        public void UpdateState_FromInProgressToScheduled_ShouldFail()
        {
            var workOrder = WorkOrderFactory.Create().Value;
            workOrder.UpdateState(State.InProgress);

            var result = workOrder.UpdateState(State.Scheduled);

            Assert.False(result.IsSuccess);
            Assert.Equal(WorkOrderErrors.InvalidWorkOrderStateTransition, result.Error);
        }

        [Fact]
        public void UpdateState_FromInProgressToCompleted_ShouldSucceed()
        {
            var workOrder = WorkOrderFactory.Create().Value;
            workOrder.UpdateState(State.InProgress);

            var result = workOrder.UpdateState(State.Completed);

            Assert.True(result.IsSuccess);
            Assert.Equal(Result.Updated, result.Value);
            Assert.Equal(State.Completed, workOrder.State);
        }

        [Fact]
        public void UpdateState_AfterCompleted_ShouldFail()
        {
            var workOrder = WorkOrderFactory.Create().Value;
            workOrder.UpdateState(State.InProgress);
            workOrder.UpdateState(State.Completed);

            var result = workOrder.UpdateState(State.Cancelled);

            Assert.False(result.IsSuccess);
            Assert.Equal(WorkOrderErrors.InvalidWorkOrderStateTransition, result.Error);
        }

        [Fact]
        public void UpdateState_AfterCancelled_ShouldFail()
        {
            var workOrder = WorkOrderFactory.Create().Value;
            workOrder.UpdateState(State.Cancelled);

            var result = workOrder.UpdateState(State.InProgress);

            Assert.False(result.IsSuccess);
            Assert.Equal(WorkOrderErrors.InvalidWorkOrderStateTransition, result.Error);
        }

        [Fact]
        public void UpsertRepairTasks_WithNewRepairTask_ShouldSucceed()
        {
            var workOrder = WorkOrderFactory.Create().Value;

            var incoming = new List<WorkOrderRepairTasks>
        {
            WorkOrderRepairTaskFactory.Create(
                workOrderId: workOrder.Id,
                repairTaskId: Guid.NewGuid(),
                time: TimeStamps.Min10,
                cost: 100,
                quantity: 1).Value
        };

            var result = workOrder.UpsertRepairTasks(incoming);

            Assert.True(result.IsSuccess);
            Assert.Equal(Result.Updated, result.Value);
            Assert.Single(workOrder.WorkOrderRepairTasks);
        }

        [Fact]
        public void UpsertRepairTasks_WithInvalidIncomingRepairTask_ShouldFail()
        {
            var workOrder = WorkOrderFactory.Create().Value;

            var incoming = new List<WorkOrderRepairTasks>
            {
                WorkOrderRepairTaskFactory.Create(
                    workOrderId: workOrder.Id,
                    repairTaskId: Guid.NewGuid(),
                    time: 0,
                    cost: 100,
                    quantity: 1).Value
            };

            var result = workOrder.UpsertRepairTasks(incoming);

            Assert.False(result.IsSuccess);
            Assert.Equal(WorkOrderErrors.InvalidRepairTasks, result.Error);
        }
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
