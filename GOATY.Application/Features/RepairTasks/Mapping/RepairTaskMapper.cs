using GOATY.Application.Features.Parts.Mapping;
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
                Parts = model.RepairTaskDetails is null ? null : model.RepairTaskDetails.ToDtos()
            };
        }
        public static List<RepairTaskDto> ToDtos(this List<RepairTask> models)
        {
            return models.ConvertAll(m => m.ToDto());
        }
        public static RepairTaskDetailsDto ToDto(this RepairTaskDetails model)
        {
            return new RepairTaskDetailsDto
            {
                Part = model.Part is null ? null : model.Part.ToDto(),
                Quantity = model.Quantity,
                UnitPrice = model.UnitPrice
            };
        }
        public static List<RepairTaskDetailsDto> ToDtos(this List<RepairTaskDetails> models)
        {
            return models.ConvertAll(m => m.ToDto());
        }
    }
}
