using GOATY.Domain.Common.Results;
using GOATY.Domain.RepairTasks;

namespace GOATY.Tests.Common.RepairTasks
{
    public static class RepairTaskDetailsFactory
    {
        public static Result<RepairTaskDetails> Create(Guid? repairTaskId = null,
                                                       Guid? partId = null,
                                                       int? quantity = null,
                                                       decimal? unitPrice = null)
        {
            return RepairTaskDetails.Create(
                    repairTaskId ?? Guid.NewGuid(),
                    partId ?? Guid.NewGuid(),
                    quantity ?? 1,
                    unitPrice ?? 100m);
        }
    }
}
