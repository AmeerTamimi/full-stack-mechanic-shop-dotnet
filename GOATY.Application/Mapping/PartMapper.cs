using GOATY.Application.DTOs;
using GOATY.Domain.Parts;

namespace GOATY.Application.Mapping
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
            return partModels.ConvertAll(part => ToDto(part));
        }
    }
}
