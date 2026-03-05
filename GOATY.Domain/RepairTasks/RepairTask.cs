using GOATY.Domain.Common;
using GOATY.Domain.Common.Constans;
using GOATY.Domain.Common.Enums;
using GOATY.Domain.Common.Results;
using GOATY.Domain.WorkOrders;

namespace GOATY.Domain.RepairTasks
{
    public sealed class RepairTask : AuditableEntity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public TimeStamps TimeEstimated { get; private set; }
        public decimal CostEstimated { get; private set; }
        public List<RepairTaskDetails> RepairTaskDetails { get; private set; } = [];
        public List<WorkOrderRepairTasks> WorkOrderRepairTasks { get; private set; } = [];


        public bool IsDeleted { get; set; }

        private RepairTask() { }
        private RepairTask(
            Guid id,
            string name,
            string desc,
            TimeStamps time,
            decimal cost,
            List<RepairTaskDetails> repairTaskDetails)
            : base(id)
        {
            Id = id;
            Name = name;
            Description = desc;
            TimeEstimated = time;
            CostEstimated = cost;
            RepairTaskDetails = repairTaskDetails;
        }

        public static Result<RepairTask> Create(Guid id,
                                        string name,
                                        string desc,
                                        TimeStamps time,
                                        decimal cost,
                                        List<RepairTaskDetails> repairTaskDetails)
        {
            if(Guid.Empty == id)
            {
                return RepairTaskErrors.InvalidId;
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                return RepairTaskErrors.InvalidName;
            }
            if (string.IsNullOrWhiteSpace(desc))
            {
                return RepairTaskErrors.InvalidDescription;
            }
            if (repairTaskDetails is null || repairTaskDetails.Count() <= 0)
            {
                return RepairTaskErrors.InvalidRepairTask;
            }

            var totalCost = CalculateTotalCost(repairTaskDetails);
            if (cost < totalCost)
            {
                return RepairTaskErrors.InvalidCostEstimated(totalCost);
            }

            if (!Enum.IsDefined(typeof(TimeStamps) , time))
            {
                return RepairTaskErrors.InvalidTimeEstimated;
            }

            if(repairTaskDetails is null)
            {
                return RepairTaskErrors.InvalidRepairTaskDetails;

            }
            return new RepairTask(id, name, desc, time, cost, repairTaskDetails);
        }

        public static Result<Updated> Update(RepairTask repairTask,
                                        string name,
                                        string desc,
                                        TimeStamps time,
                                        decimal cost)
        {

            if (string.IsNullOrWhiteSpace(name))
            {
                return RepairTaskErrors.InvalidName;
            }
            if (string.IsNullOrWhiteSpace(desc))
            {
                return RepairTaskErrors.InvalidDescription;
            }

            if (!Enum.IsDefined(typeof(TimeStamps) , time))
            {
                return RepairTaskErrors.InvalidTimeEstimated;
            }

            repairTask.Name = name;
            repairTask.Description = desc;
            repairTask.TimeEstimated = time;
            repairTask.CostEstimated = cost;

            return Result.Updated;
        }

        public Result<Updated> UpsertRepairTaskDetails(List<RepairTaskDetails> incoming)
        {
            if (incoming is null || incoming.Count() <= 0)
            {
                return RepairTaskErrors.InvalidRepairTask;
            }

            RepairTaskDetails.RemoveAll(existing => 
                !incoming.Any(rd => rd.RepairTaskId == Id &&
                                    rd.PartId == existing.PartId));

            foreach(var repairTaskDetail in incoming)
            {
                var existing = RepairTaskDetails.SingleOrDefault(rd => rd.RepairTaskId == Id &&
                                                                       rd.PartId == repairTaskDetail.PartId);
                
                if (existing is null)
                {
                    var addResult = RepairTasks.RepairTaskDetails.Create(Id,
                                                                         repairTaskDetail.PartId,
                                                                         repairTaskDetail.Quantity,
                                                                         repairTaskDetail.UnitPrice);

                    if (!addResult.IsSuccess)
                    {
                        return addResult.Errors;
                    }

                    RepairTaskDetails.Add(addResult.Value);
                }
                else
                {
                    var updatedResult = existing.Update(repairTaskDetail.Quantity , repairTaskDetail.UnitPrice);

                    if (!updatedResult.IsSuccess)
                    {
                        return updatedResult.Errors;
                    }
                }
            }

            var totalCost = CalculateTotalCost(incoming);
            if (CostEstimated < totalCost)
            {
                return RepairTaskErrors.InvalidCostEstimated(totalCost);
            }

            return Result.Updated;
        }

        private static decimal CalculateTotalCost(List<RepairTaskDetails> repairTaskDetails)
        {
            decimal total = 0;

            foreach (var r in repairTaskDetails)
            {
                total += r.Quantity * r.UnitPrice;
            }

            var taxes = total * GOATYConstans.TaxRate;
            var techBase = GOATYConstans.TechnicianBase;

            return total + taxes + techBase;
        }

    }
}
