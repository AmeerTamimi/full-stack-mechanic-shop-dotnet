using GOATY.Application.Features.RepairTasks.DTOs;
using GOATY.Domain.RepairTasks;

namespace GOATY.Application.Features.RepairTasks.Mapping
{
    public static class RepairTaskMapper
    {
        public static RepairTaskDto ToDto(this RepairTask model)
        {
            return new RepairTaskDto
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                TimeEstimated = model.TimeEstimated,
                CostEstimated = model.CostEstimated,
            };
        }

        public static List<RepairTaskDto> ToDtos(this List<RepairTask> models)
        {
            return models.ConvertAll(m => m.ToDto());
        }
    }
}
