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
        public decimal TechnicianCost { get; private set; }

        private readonly List<RepairTaskDetails> _repairTaskDetails = [];
        public IReadOnlyCollection<RepairTaskDetails> RepairTaskDetails => _repairTaskDetails;

        private readonly List<WorkOrderRepairTasks> _workOrderRepairTasks = [];
        public IReadOnlyCollection<WorkOrderRepairTasks> WorkOrderRepairTasks => _workOrderRepairTasks;
        public bool IsDeleted { get; set; }

        private RepairTask() { }
        private RepairTask(
            Guid id,
            string name,
            string desc,
            TimeStamps time,
            decimal cost,
            decimal technicianCost,
            List<RepairTaskDetails> repairTaskDetails)
            : base(id)
        {
            Id = id;
            Name = name;
            Description = desc;
            TimeEstimated = time;
            CostEstimated = cost;
            TechnicianCost = technicianCost;
            _repairTaskDetails = repairTaskDetails;
        }

        public static Result<RepairTask> Create(Guid id,
                                                string name,
                                                string desc,
                                                TimeStamps time,
                                                decimal cost,
                                                decimal technicianCost,
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

            var totalCost = repairTaskDetails.Sum(rd => rd.Quantity * rd.UnitPrice);
            if (cost < totalCost)
            {
                return RepairTaskErrors.InvalidCostEstimated(totalCost);
            }

            if(technicianCost < GOATYConstans.TechnicianBase)
            {
                return RepairTaskErrors.InvalidTechnicianCost;
            }

            if (!Enum.IsDefined(typeof(TimeStamps) , time))
            {
                return RepairTaskErrors.InvalidTimeEstimated;
            }

            if(repairTaskDetails is null)
            {
                return RepairTaskErrors.InvalidRepairTaskDetails;

            }
            return new RepairTask(id, name, desc, time, cost, technicianCost, repairTaskDetails);
        }

        public Result<Updated> Update(string name,
                                      string desc,
                                      TimeStamps time,
                                      decimal cost,
                                      decimal technicianCost)
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

            var totalCost = _repairTaskDetails.Sum(rd => rd.Quantity * rd.UnitPrice);
            if (cost < totalCost)
            {
                return RepairTaskErrors.InvalidCostEstimated(totalCost);
            }

            if (technicianCost < GOATYConstans.TechnicianBase)
            {
                return RepairTaskErrors.InvalidTechnicianCost;
            }

            Name = name;
            Description = desc;
            TimeEstimated = time;
            CostEstimated = cost;
            TechnicianCost = technicianCost;

            return Result.Updated;
        }

        public Result<Updated> UpsertRepairTaskDetails(List<RepairTaskDetails> incoming)
        {
            if (incoming is null || incoming.Count() <= 0)
            {
                return RepairTaskErrors.InvalidRepairTask;
            }

            _repairTaskDetails.RemoveAll(existing => 
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

                    _repairTaskDetails.Add(addResult.Value);
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

            var totalCost = incoming.Sum(i => i.Quantity * i.UnitPrice);
            if (CostEstimated < totalCost)
            {
                return RepairTaskErrors.InvalidCostEstimated(totalCost);
            }

            return Result.Updated;
        }
    }
}