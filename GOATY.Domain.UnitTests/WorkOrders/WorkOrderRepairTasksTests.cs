using GOATY.Domain.Common.Enums;
using GOATY.Domain.Common.Results;
using GOATY.Domain.WorkOrders;
using GOATY.Tests.Common.WorkOrders;

namespace GOATY.Domain.UnitTests.WorkOrders
{
    public sealed class WorkOrderRepairTasksTests
    {
        [Fact]
        public void Create_WithValidData_ShouldSucceed()
        {
            var result = WorkOrderRepairTaskFactory.Create();

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void Create_WithInvalidWorkOrderId_ShouldFail()
        {
            var result = WorkOrderRepairTaskFactory.Create(workOrderId: Guid.Empty);

            Assert.False(result.IsSuccess);
            Assert.Equal(WorkOrderErrors.InvalidWorkOrderId, result.Error);
        }

        [Fact]
        public void Create_WithInvalidRepairTaskId_ShouldFail()
        {
            var result = WorkOrderRepairTaskFactory.Create(repairTaskId: Guid.Empty);

            Assert.False(result.IsSuccess);
            Assert.Equal(WorkOrderErrors.InvalidRepairTaskId, result.Error);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(999)]
        public void Create_WithInvalidTime_ShouldFail(int rawTime)
        {
            var result = WorkOrderRepairTaskFactory.Create(time: (TimeStamps)rawTime);

            Assert.False(result.IsSuccess);
            Assert.Equal(WorkOrderErrors.InvalidRepairTaskTime, result.Error);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-50)]
        public void Create_WithInvalidCost_ShouldFail(decimal cost)
        {
            var result = WorkOrderRepairTaskFactory.Create(cost: cost);

            Assert.False(result.IsSuccess);
            Assert.Equal(WorkOrderErrors.InvalidRepairTaskCost, result.Error);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-5)]
        public void Create_WithInvalidQuantity_ShouldFail(int quantity)
        {
            var result = WorkOrderRepairTaskFactory.Create(quantity: quantity);

            Assert.False(result.IsSuccess);
            Assert.Equal(WorkOrderErrors.InvalidQuantity, result.Error);
        }

        [Fact]
        public void Update_WithValidData_ShouldSucceed()
        {
            var workOrderRepairTask = WorkOrderRepairTaskFactory.Create().Value;

            var result = workOrderRepairTask.Update(TimeStamps.Min30, 100m, 2);

            Assert.True(result.IsSuccess);
            Assert.Equal(Result.Updated, result.Value);
            Assert.Equal(TimeStamps.Min30, workOrderRepairTask.Time);
            Assert.Equal(100m, workOrderRepairTask.Cost);
            Assert.Equal(2, workOrderRepairTask.Quantity);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(999)]
        public void Update_WithInvalidTime_ShouldFail(int rawTime)
        {
            var workOrderRepairTask = WorkOrderRepairTaskFactory.Create().Value;

            var result = workOrderRepairTask.Update((TimeStamps)rawTime, 100m, 2);

            Assert.False(result.IsSuccess);
            Assert.Equal(WorkOrderErrors.InvalidRepairTaskTime, result.Error);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-50)]
        public void Update_WithInvalidCost_ShouldFail(decimal cost)
        {
            var workOrderRepairTask = WorkOrderRepairTaskFactory.Create().Value;

            var result = workOrderRepairTask.Update(TimeStamps.Min30, cost, 2);

            Assert.False(result.IsSuccess);
            Assert.Equal(WorkOrderErrors.InvalidRepairTaskCost, result.Error);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-5)]
        public void Update_WithInvalidQuantity_ShouldFail(int quantity)
        {
            var workOrderRepairTask = WorkOrderRepairTaskFactory.Create().Value;

            var result = workOrderRepairTask.Update(TimeStamps.Min30, 100m, quantity);

            Assert.False(result.IsSuccess);
            Assert.Equal(WorkOrderErrors.InvalidQuantity, result.Error);
        }
    }
}