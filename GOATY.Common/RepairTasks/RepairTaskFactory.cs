using GOATY.Domain.Common.Constans;
using GOATY.Domain.Common.Enums;
using GOATY.Domain.Common.Results;
using GOATY.Domain.RepairTasks;

namespace GOATY.Tests.Common.RepairTasks
{
    public static class RepairTaskFactory
    {
        public static Result<RepairTask> Create(Guid? id = null,
                                        string? name = null,
                                        string? desc = null,
                                        TimeStamps? time = null,
                                        decimal? cost = null,
                                        decimal? technicianCost = null,
                                        List<RepairTaskDetails>? repairTaskDetails = null)
        {
            return RepairTask.Create(id ?? Guid.NewGuid(),
                name ?? "GoodName",
                desc ?? "GoodDesc",
                time ?? TimeStamps.Min30,
                cost ?? 50m,
                technicianCost ?? GOATYConstans.TechnicianBase,
                [RepairTaskDetailsFactory.Create().Value]);
        }
    }
}
