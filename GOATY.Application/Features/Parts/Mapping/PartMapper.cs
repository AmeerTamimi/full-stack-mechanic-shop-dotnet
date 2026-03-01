using GOATY.Application.Features.Parts.DTOs;
using GOATY.Domain.RepairsTask.Parts;

namespace GOATY.Application.Features.Parts.Mapping
{
    public static class PartMapper
    {
        public static PartDto ToDto(this Part model)
        {
            return new PartDto
            {
                Id = model.Id,
                Cost = model.Cost,
                Name = model.Name,
                Quantity = model.Quantity
            };
        }
        public static List<PartDto> ToDtos(this List<Part> partModels)
        {
            return partModels.ConvertAll(part => part.ToDto());
        }
    }
}