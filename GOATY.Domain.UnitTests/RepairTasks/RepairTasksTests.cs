using GOATY.Domain.Common.Constans;
using GOATY.Domain.Common.Enums;
using GOATY.Domain.Common.Results;
using GOATY.Domain.RepairTasks;

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
            var time = Enum.GetValues<TimeStamps>().First();

            var partId = Guid.NewGuid();
            var details = new List<RepairTaskDetails>
            {
                RepairTaskDetails.Create(id, partId, quantity: 2, unitPrice: 30m).Value
            };

            var cost = 10000m;
            var technicianCost = GOATYConstans.TechnicianBase;

            var result = RepairTask.Create(id, name, desc, time, cost, technicianCost, details);

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
            var time = Enum.GetValues<TimeStamps>().First();

            var details = new List<RepairTaskDetails>
            {
                RepairTaskDetails.Create(Guid.NewGuid(), Guid.NewGuid(), quantity: 1, unitPrice: 10m).Value
            };

            var cost = 999m;
            var technicianCost = GOATYConstans.TechnicianBase;

            var result = RepairTask.Create(id, name, desc, time, cost, technicianCost, details);

            var actual = result.Error;
            var expected = RepairTaskErrors.InvalidId;

            Assert.False(result.IsSuccess);
            Assert.Equal(actual, expected);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(" ")]
        public void Create_WithInvalidName_ShouldFail(string name)
        {
            var id = Guid.NewGuid();
            var desc = "Replace battery terminals";
            var time = Enum.GetValues<TimeStamps>().First();

            var details = new List<RepairTaskDetails>
            {
                RepairTaskDetails.Create(id, Guid.NewGuid(), quantity: 1, unitPrice: 10m).Value
            };

            var cost = 999m;
            var technicianCost = GOATYConstans.TechnicianBase;

            var result = RepairTask.Create(id, name, desc, time, cost, technicianCost, details);

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
            var time = Enum.GetValues<TimeStamps>().First();

            var details = new List<RepairTaskDetails>
            {
                RepairTaskDetails.Create(id, Guid.NewGuid(), quantity: 1, unitPrice: 10m).Value
            };

            var cost = 999m;
            var technicianCost = GOATYConstans.TechnicianBase;

            var result = RepairTask.Create(id, name, desc, time, cost, technicianCost, details);

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
            var time = Enum.GetValues<TimeStamps>().First();
            var cost = 999m;
            var technicianCost = GOATYConstans.TechnicianBase;

            var result = RepairTask.Create(id, name, desc, time, cost, technicianCost, repairTaskDetails: null!);

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
            var time = Enum.GetValues<TimeStamps>().First();
            var cost = 999m;
            var technicianCost = GOATYConstans.TechnicianBase;

            var result = RepairTask.Create(id, name, desc, time, cost, technicianCost, new List<RepairTaskDetails>());

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
            var time = Enum.GetValues<TimeStamps>().First();

            var details = new List<RepairTaskDetails>
            {
                RepairTaskDetails.Create(id, Guid.NewGuid(), quantity: 2, unitPrice: 30m).Value
            };

            var subtotal = details.Sum(d => d.Quantity * d.UnitPrice);
            var cost = subtotal - 10m;
            var technicianCost = GOATYConstans.TechnicianBase;

            var result = RepairTask.Create(id, name, desc, time, cost, technicianCost, details);

            var actual = result.Error;
            var expected = RepairTaskErrors.InvalidCostEstimated(subtotal);

            Assert.False(result.IsSuccess);
            Assert.Equivalent(actual, expected);
        }

        [Fact]
        public void Create_WithInvalidTimeEstimated_ShouldFail()
        {
            var id = Guid.NewGuid();
            var name = "Battery Repair";
            var desc = "Replace battery terminals";
            var time = (TimeStamps)999;

            var details = new List<RepairTaskDetails>
            {
                RepairTaskDetails.Create(id, Guid.NewGuid(), quantity: 1, unitPrice: 10m).Value
            };

            var cost = 999m;
            var technicianCost = GOATYConstans.TechnicianBase;

            var result = RepairTask.Create(id, name, desc, time, cost, technicianCost, details);

            var actual = result.Error;
            var expected = RepairTaskErrors.InvalidTimeEstimated;

            Assert.False(result.IsSuccess);
            Assert.Equal(actual, expected);
        }

        [Fact]
        public void Create_WithInvalidTechnicianCost_ShouldFail()
        {
            var id = Guid.NewGuid();
            var name = "Battery Repair";
            var desc = "Replace battery terminals";
            var time = Enum.GetValues<TimeStamps>().First();

            var details = new List<RepairTaskDetails>
            {   
                RepairTaskDetails.Create(id, Guid.NewGuid(), quantity: 1, unitPrice: 10m).Value
            };

            var cost = 999m;
            var technicianCost = GOATYConstans.TechnicianBase - 1;

            var result = RepairTask.Create(id, name, desc, time, cost, technicianCost, details);

            var actual = result.Error;
            var expected = RepairTaskErrors.InvalidTechnicianCost;

            Assert.False(result.IsSuccess);
            Assert.Equal(actual, expected);
        }

        [Fact]
        public void Update_WithValidData_ShouldSucceed()
        {
            var id = Guid.NewGuid();
            var initialTime = Enum.GetValues<TimeStamps>().First();
            var initialDetails = new List<RepairTaskDetails>
            {
                RepairTaskDetails.Create(id, Guid.NewGuid(), quantity: 1, unitPrice: 10m).Value
            };
            var repairTask = RepairTask.Create(id, "Initial", "Initial Desc", initialTime, 999999m, GOATYConstans.TechnicianBase, initialDetails).Value;

            var newName = "New Name";
            var newDesc = "New Desc";
            var newTime = Enum.GetValues<TimeStamps>().Last();
            var newDetails = new List<RepairTaskDetails>
            {
                RepairTaskDetails.Create(id, Guid.NewGuid(), quantity: 3, unitPrice: 50m).Value
            };
            var newCost = 999999m;
            var newTechnicianCost = GOATYConstans.TechnicianBase;

            var updateResult = repairTask.Update(newName, newDesc, newTime, newCost, newTechnicianCost);
            var updateActual = updateResult.Value;
            var updateExpected = Result.Updated;

            Assert.True(updateResult.IsSuccess);
            Assert.Equal(updateActual, updateExpected);

            var upsertResult = repairTask.UpsertRepairTaskDetails(newDetails);
            var upsertActual = upsertResult.Value;
            var upsertExpected = Result.Updated;

            Assert.True(upsertResult.IsSuccess);
            Assert.Equal(upsertActual, upsertExpected);

            var actualRepairTask = repairTask;

            Assert.Equal(id, actualRepairTask.Id);
            Assert.Equal(newName, actualRepairTask.Name);
            Assert.Equal(newDesc, actualRepairTask.Description);
            Assert.Equal(newTime, actualRepairTask.TimeEstimated);
            Assert.Equal(newCost, actualRepairTask.CostEstimated);
            Assert.Equal(newTechnicianCost, actualRepairTask.TechnicianCost);
            Assert.Equivalent(newDetails, actualRepairTask.RepairTaskDetails);
        }

        [Fact]
        public void Update_WithInvalidName_ShouldFail()
        {
            var id = Guid.NewGuid();
            var time = Enum.GetValues<TimeStamps>().First();
            var details = new List<RepairTaskDetails>
            {
                RepairTaskDetails.Create(id, Guid.NewGuid(), quantity: 1, unitPrice: 10m).Value
            };

            var repairTask = RepairTask.Create(id, "Initial", "Initial Desc", time, 999999m, GOATYConstans.TechnicianBase, details).Value;

            var updateResult = repairTask.Update("", "New Desc", time, -1, GOATYConstans.TechnicianBase);
            var updateActual = updateResult.Error;
            var updateExpected = RepairTaskErrors.InvalidName;

            Assert.False(updateResult.IsSuccess);
            Assert.Equal(updateActual, updateExpected);

            var upsertResult = repairTask.UpsertRepairTaskDetails(details);
            var upsertActual = upsertResult.Value;
            var upsertExpected = Result.Updated;

            Assert.True(upsertResult.IsSuccess);
            Assert.Equal(upsertActual, upsertExpected);
        }

        [Fact]
        public void Update_WithInvalidDescription_ShouldFail()
        {
            var id = Guid.NewGuid();
            var time = Enum.GetValues<TimeStamps>().First();
            var details = new List<RepairTaskDetails>
            {
                RepairTaskDetails.Create(id, Guid.NewGuid(), quantity: 1, unitPrice: 10m).Value
            };
            var repairTask = RepairTask.Create(id, "Initial", "Initial Desc", time, 999999m, GOATYConstans.TechnicianBase, details).Value;

            var updateResult = repairTask.Update("New Name", null!, time, -1, GOATYConstans.TechnicianBase);
            var updateActual = updateResult.Error;
            var updateExpected = RepairTaskErrors.InvalidDescription;

            Assert.False(updateResult.IsSuccess);
            Assert.Equal(updateActual, updateExpected);

            var upsertResult = repairTask.UpsertRepairTaskDetails(details);
            var upsertActual = upsertResult.Value;
            var upsertExpected = Result.Updated;

            Assert.True(upsertResult.IsSuccess);
            Assert.Equal(upsertActual, upsertExpected);
        }

        [Fact]
        public void Update_WithNullRepairTaskDetails_ShouldFail()
        {
            var id = Guid.NewGuid();
            var time = Enum.GetValues<TimeStamps>().First();
            var details = new List<RepairTaskDetails>
            {
                RepairTaskDetails.Create(id, Guid.NewGuid(), quantity: 1, unitPrice: 10m).Value
            };
            var repairTask = RepairTask.Create(id, "Initial", "Initial Desc", time, 999999m, GOATYConstans.TechnicianBase, details).Value;

            var updateResult = repairTask.Update("New Name", "New Desc", time, 999999m, GOATYConstans.TechnicianBase);
            var updateActual = updateResult.Value;
            var updateExpected = Result.Updated;

            Assert.True(updateResult.IsSuccess);
            Assert.Equal(updateActual, updateExpected);

            var upsertResult = repairTask.UpsertRepairTaskDetails(null!);
            var upsertActual = upsertResult.Error;
            var upsertExpected = RepairTaskErrors.InvalidRepairTask;

            Assert.False(upsertResult.IsSuccess);
            Assert.Equal(upsertActual, upsertExpected);
        }

        [Fact]
        public void Update_WithInvalidCostEstimated_ShouldFail()
        {
            var id = Guid.NewGuid();
            var time = Enum.GetValues<TimeStamps>().First();
            var details = new List<RepairTaskDetails>
            {
                RepairTaskDetails.Create(id, Guid.NewGuid(), quantity: 2, unitPrice: 30m).Value
            };
            var repairTask = RepairTask.Create(id, "Initial", "Initial Desc", time, 999999m, GOATYConstans.TechnicianBase, details).Value;

            var subtotal = details.Sum(d => d.Quantity * d.UnitPrice);

            var updateResult = repairTask.Update("New Name", "New Desc", time, subtotal - 1, GOATYConstans.TechnicianBase);
            var updateActual = updateResult.Error;
            var updateExpected = RepairTaskErrors.InvalidCostEstimated(subtotal);

            Assert.False(updateResult.IsSuccess);
            Assert.Equivalent(updateActual, updateExpected);

            var upsertResult = repairTask.UpsertRepairTaskDetails(details);
            var upsertActual = upsertResult.Value;
            var upsertExpected = Result.Updated;

            Assert.True(upsertResult.IsSuccess);
            Assert.Equal(upsertActual, upsertExpected);
        }

        [Fact]
        public void Update_WithInvalidTechnicianCost_ShouldFail()
        {
            var id = Guid.NewGuid();
            var time = Enum.GetValues<TimeStamps>().First();
            var details = new List<RepairTaskDetails>
            {
                RepairTaskDetails.Create(id, Guid.NewGuid(), quantity: 1, unitPrice: 10m).Value
            };
            var repairTask = RepairTask.Create(id, "Initial", "Initial Desc", time, 999999m, GOATYConstans.TechnicianBase, details).Value;

            var updateResult = repairTask.Update("New Name", "New Desc", time, 999999m, GOATYConstans.TechnicianBase - 1);
            var updateActual = updateResult.Error;
            var updateExpected = RepairTaskErrors.InvalidTechnicianCost;

            Assert.False(updateResult.IsSuccess);
            Assert.Equal(updateActual, updateExpected);

            var upsertResult = repairTask.UpsertRepairTaskDetails(details);
            var upsertActual = upsertResult.Value;
            var upsertExpected = Result.Updated;

            Assert.True(upsertResult.IsSuccess);
            Assert.Equal(upsertActual, upsertExpected);
        }

        [Fact]
        public void Update_WithInvalidTimeEstimated_ShouldFail()
        {
            var id = Guid.NewGuid();
            var time = Enum.GetValues<TimeStamps>().First();
            var details = new List<RepairTaskDetails>
            {
                RepairTaskDetails.Create(id, Guid.NewGuid(), quantity: 1, unitPrice: 10m).Value
            };
            var repairTask = RepairTask.Create(id, "Initial", "Initial Desc", time, 999999m, GOATYConstans.TechnicianBase, details).Value;

            var invalidTime = (TimeStamps)999;

            var updateResult = repairTask.Update("New Name", "New Desc", invalidTime, 999999m, GOATYConstans.TechnicianBase);
            var updateActual = updateResult.Error;
            var updateExpected = RepairTaskErrors.InvalidTimeEstimated;

            Assert.False(updateResult.IsSuccess);
            Assert.Equal(updateActual, updateExpected);

            var upsertResult = repairTask.UpsertRepairTaskDetails(details);
            var upsertActual = upsertResult.Value;
            var upsertExpected = Result.Updated;

            Assert.True(upsertResult.IsSuccess);
            Assert.Equal(upsertActual, upsertExpected);
        }
    }
}