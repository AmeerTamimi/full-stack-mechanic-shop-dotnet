using GOATY.Domain.Common;
using GOATY.Domain.Common.Results;
using GOATY.Domain.Common.Constans;
using System.Security.Cryptography;

namespace GOATY.Domain.RepairTasks
{
    public sealed class RepairTask : AuditableEntity
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal TimeEstimated { get; set; }
        public decimal CostEstimated { get; set; }
        public List<RepairTaskDetails> RepairTaskDetails { get; set; } = [];

        public bool IsDeleted { get; set; }

        private RepairTask() { }
        private RepairTask(
            Guid id,
            string name,
            string desc,
            decimal time,
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
                                        decimal time,
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

            if (time < 10)
            {
                return RepairTaskErrors.InvalidTimeEstimated;
            }

            return new RepairTask(id, name, desc, time, cost, repairTaskDetails);
        }
        public static Result<Updated> Update(RepairTask repairTask,
                                        string name,
                                        string desc,
                                        decimal time,
                                        decimal cost,
                                        List<RepairTaskDetails> repairTaskDetails)
        {
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

            if (time < 10)
            {
                return RepairTaskErrors.InvalidTimeEstimated;
            }
            repairTask.Name = name;
            repairTask.Description = desc;
            repairTask.TimeEstimated = time;
            repairTask.CostEstimated = cost;
            repairTask.RepairTaskDetails = repairTaskDetails;
            return Result.Updated;
        }

        private static decimal CalculateTotalCost(List<RepairTaskDetails> repairTaskDetails)
        {
            decimal total = 0;

            foreach(var r in repairTaskDetails)
            {
                total += r.Quantity * r.UnitPrice;
            }

            var taxes = total * GOATYConstans.TaxRate;
            var techBase = GOATYConstans.TechnicianBase;

            return total + taxes + techBase;
        }
    }
}
