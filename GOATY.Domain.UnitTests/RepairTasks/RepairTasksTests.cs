using GOATY.Domain.Common.Constans;
using GOATY.Domain.RepairTasks;
using GOATY.Domain.RepairTasks.Enums;

namespace GOATY.Domain.UnitTests.RepairTasks
{
    public sealed class RepairTaskTests
    {
        [Fact]
        public void Create_WithValidData_ShouldSucceed()
        {
            var id = Guid.NewGuid();
            var name = "Battery Repair";
            var desc = "Replace battery terminals";
            var time = Enum.GetValues<TimeEstimations>().First();

            var partId = Guid.NewGuid();
            var details = new List<RepairTaskDetails>
            {
                RepairTaskDetails.Create(id, partId, quantity: 2, unitPrice: 30m).Value
            };

            var cost = 10000m;

            var result = RepairTask.Create(id, name, desc, time, cost, details);

            var actual = result.Value;

            Assert.True(result.IsSuccess);
            Assert.Equal(id, actual.Id);
            Assert.Equal(name, actual.Name);
            Assert.Equal(desc, actual.Description);
            Assert.Equal(time, actual.TimeEstimated);
            Assert.Equal(cost, actual.CostEstimated);
            Assert.Equal(details, actual.RepairTaskDetails);
        }

        [Fact]
        public void Create_WithInvalidId_ShouldFail()
        {
            var id = Guid.Empty;
            var name = "Battery Repair";
            var desc = "Replace battery terminals";
            var time = Enum.GetValues<TimeEstimations>().First();

            var details = new List<RepairTaskDetails>
            {
                RepairTaskDetails.Create(Guid.NewGuid(), Guid.NewGuid(), quantity: 1, unitPrice: 10m).Value
            };

            var cost = 999m;

            var result = RepairTask.Create(id, name, desc, time, cost, details);

            var actual = result.Error;
            var expected = RepairTaskErrors.InvalidId;

            Assert.False(result.IsSuccess);
            Assert.Equal(actual, expected);
        }

        [Fact]
        public void Create_WithInvalidName_ShouldFail()
        {
            var id = Guid.NewGuid();
            var name = "   ";
            var desc = "Replace battery terminals";
            var time = Enum.GetValues<TimeEstimations>().First();

            var details = new List<RepairTaskDetails>
            {
                RepairTaskDetails.Create(id, Guid.NewGuid(), quantity: 1, unitPrice: 10m).Value
            };

            var cost = 999m;

            var result = RepairTask.Create(id, name, desc, time, cost, details);

            var actual = result.Error;
            var expected = RepairTaskErrors.InvalidName;

            Assert.False(result.IsSuccess);
            Assert.Equal(actual, expected);
        }

        [Fact]
        public void Create_WithInvalidDescription_ShouldFail()
        {
            var id = Guid.NewGuid();
            var name = "Battery Repair";
            var desc = "";
            var time = Enum.GetValues<TimeEstimations>().First();

            var details = new List<RepairTaskDetails>
            {
                RepairTaskDetails.Create(id, Guid.NewGuid(), quantity: 1, unitPrice: 10m).Value
            };

            var cost = 999m;

            var result = RepairTask.Create(id, name, desc, time, cost, details);

            var actual = result.Error;
            var expected = RepairTaskErrors.InvalidDescription;

            Assert.False(result.IsSuccess);
            Assert.Equal(actual, expected);
        }

        [Fact]
        public void Create_WithNullRepairTaskDetails_ShouldFail()
        {
            var id = Guid.NewGuid();
            var name = "Battery Repair";
            var desc = "Replace battery terminals";
            var time = Enum.GetValues<TimeEstimations>().First();
            var cost = 999m;

            var result = RepairTask.Create(id, name, desc, time, cost, repairTaskDetails: null!);

            var actual = result.Error;
            var expected = RepairTaskErrors.InvalidRepairTask;

            Assert.False(result.IsSuccess);
            Assert.Equal(actual, expected);
        }

        [Fact]
        public void Create_WithEmptyRepairTaskDetails_ShouldFail()
        {
            var id = Guid.NewGuid();
            var name = "Battery Repair";
            var desc = "Replace battery terminals";
            var time = Enum.GetValues<TimeEstimations>().First();
            var cost = 999m;

            var result = RepairTask.Create(id, name, desc, time, cost, new List<RepairTaskDetails>());

            var actual = result.Error;
            var expected = RepairTaskErrors.InvalidRepairTask;

            Assert.False(result.IsSuccess);
            Assert.Equal(actual, expected);
        }

        [Fact]
        public void Create_WithInvalidCostEstimated_ShouldFail()
        {
            var id = Guid.NewGuid();
            var name = "Battery Repair";
            var desc = "Replace battery terminals";
            var time = Enum.GetValues<TimeEstimations>().First();

            var details = new List<RepairTaskDetails>
            {
                RepairTaskDetails.Create(id, Guid.NewGuid(), quantity: 2, unitPrice: 30m).Value
            };

            var subtotal = details.Sum(d => d.Quantity * d.UnitPrice);
            var totalCost = subtotal + (subtotal * GOATYConstans.TaxRate) + GOATYConstans.TechnicianBase;

            var cost = totalCost - 10m; // less than needed

            var result = RepairTask.Create(id, name, desc, time, cost, details);

            var actual = result.Error;
            var expected = RepairTaskErrors.InvalidCostEstimated(totalCost);

            Assert.False(result.IsSuccess);
            Assert.Equivalent(actual, expected);
        }

        [Fact]
        public void Create_WithInvalidTimeEstimated_ShouldFail()
        {
            var id = Guid.NewGuid();
            var name = "Battery Repair";
            var desc = "Replace battery terminals";
            var time = (TimeEstimations)999;

            var details = new List<RepairTaskDetails>
            {
                RepairTaskDetails.Create(id, Guid.NewGuid(), quantity: 1, unitPrice: 10m).Value
            };

            var cost = 999m;

            var result = RepairTask.Create(id, name, desc, time, cost, details);

            var actual = result.Error;
            var expected = RepairTaskErrors.InvalidTimeEstimated;

            Assert.False(result.IsSuccess);
            Assert.Equal(actual, expected);
        }

        [Fact]
        public void Update_WithValidData_ShouldSucceed()
        {
            var id = Guid.NewGuid();
            var initialTime = Enum.GetValues<TimeEstimations>().First();
            var initialDetails = new List<RepairTaskDetails>
            {
                RepairTaskDetails.Create(id, Guid.NewGuid(), quantity: 1, unitPrice: 10m).Value
            };
            var repairTask = RepairTask.Create(id, "Initial", "Initial Desc", initialTime, 999999m, initialDetails).Value;

            var newName = "New Name";
            var newDesc = "New Desc";
            var newTime = Enum.GetValues<TimeEstimations>().Last();
            var newDetails = new List<RepairTaskDetails>
            {
                RepairTaskDetails.Create(id, Guid.NewGuid(), quantity: 3, unitPrice: 50m).Value
            };
            var newCost = 999999m;

            var result = RepairTask.Update(repairTask, newName, newDesc, newTime, newCost, newDetails);

            var actual = repairTask;

            Assert.True(result.IsSuccess);
            Assert.Equal(id, actual.Id);
            Assert.Equal(newName, actual.Name);
            Assert.Equal(newDesc, actual.Description);
            Assert.Equal(newTime, actual.TimeEstimated);
            Assert.Equal(newCost, actual.CostEstimated);
            Assert.Equal(newDetails, actual.RepairTaskDetails);
        }

        [Fact]
        public void Update_WithInvalidName_ShouldFail()
        {
            var id = Guid.NewGuid();
            var time = Enum.GetValues<TimeEstimations>().First();
            var details = new List<RepairTaskDetails>
            {
                RepairTaskDetails.Create(id, Guid.NewGuid(), quantity: 1, unitPrice: 10m).Value
            };
            var repairTask = RepairTask.Create(id, "Initial", "Initial Desc", time, 999999m, details).Value;

            var result = RepairTask.Update(repairTask, name: "", desc: "New Desc", time, cost: 999999m, details);

            var actual = result.Error;
            var expected = RepairTaskErrors.InvalidName;

            Assert.False(result.IsSuccess);
            Assert.Equal(actual, expected);
        }

        [Fact]
        public void Update_WithInvalidDescription_ShouldFail()
        {
            var id = Guid.NewGuid();
            var time = Enum.GetValues<TimeEstimations>().First();
            var details = new List<RepairTaskDetails>
            {
                RepairTaskDetails.Create(id, Guid.NewGuid(), quantity: 1, unitPrice: 10m).Value
            };
            var repairTask = RepairTask.Create(id, "Initial", "Initial Desc", time, 999999m, details).Value;

            var result = RepairTask.Update(repairTask, name: "New Name", desc: "   ", time, cost: 999999m, details);

            var actual = result.Error;
            var expected = RepairTaskErrors.InvalidDescription;

            Assert.False(result.IsSuccess);
            Assert.Equal(actual, expected);
        }

        [Fact]
        public void Update_WithNullRepairTaskDetails_ShouldFail()
        {
            var id = Guid.NewGuid();
            var time = Enum.GetValues<TimeEstimations>().First();
            var details = new List<RepairTaskDetails>
            {
                RepairTaskDetails.Create(id, Guid.NewGuid(), quantity: 1, unitPrice: 10m).Value
            };
            var repairTask = RepairTask.Create(id, "Initial", "Initial Desc", time, 999999m, details).Value;

            var result = RepairTask.Update(repairTask, name: "New Name", desc: "New Desc", time, cost: 999999m, repairTaskDetails: null!);

            var actual = result.Error;
            var expected = RepairTaskErrors.InvalidRepairTask;

            Assert.False(result.IsSuccess);
            Assert.Equal(actual, expected);
        }

        [Fact]
        public void Update_WithInvalidCostEstimated_ShouldFail()
        {
            var id = Guid.NewGuid();
            var time = Enum.GetValues<TimeEstimations>().First();
            var details = new List<RepairTaskDetails>
            {
                RepairTaskDetails.Create(id, Guid.NewGuid(), quantity: 2, unitPrice: 30m).Value
            };
            var repairTask = RepairTask.Create(id, "Initial", "Initial Desc", time, 999999m, details).Value;

            var subtotal = details.Sum(d => d.Quantity * d.UnitPrice);
            var totalCost = subtotal + (subtotal * GOATYConstans.TaxRate) + GOATYConstans.TechnicianBase;

            var result = RepairTask.Update(repairTask, name: "New Name", desc: "New Desc", time, cost: totalCost - 10m, details);

            var actual = result.Error;
            var expected = RepairTaskErrors.InvalidCostEstimated(totalCost);

            Assert.False(result.IsSuccess);
            Assert.Equivalent(actual, expected);
        }

        [Fact]
        public void Update_WithInvalidTimeEstimated_ShouldFail()
        {
            var id = Guid.NewGuid();
            var time = Enum.GetValues<TimeEstimations>().First();
            var details = new List<RepairTaskDetails>
            {
                RepairTaskDetails.Create(id, Guid.NewGuid(), quantity: 1, unitPrice: 10m).Value
            };
            var repairTask = RepairTask.Create(id, "Initial", "Initial Desc", time, 999999m, details).Value;

            var invalidTime = (TimeEstimations)999;

            var result = RepairTask.Update(repairTask, name: "New Name", desc: "New Desc", invalidTime, cost: 999999m, details);

            var actual = result.Error;
            var expected = RepairTaskErrors.InvalidTimeEstimated;

            Assert.False(result.IsSuccess);
            Assert.Equal(actual, expected);
        }
    }
}