using GOATY.Domain.Parts;

namespace GOATY.Application.Mapping.PartsMapping
{
    public class PartDto
    {
        public Guid Id { get; set; }
        public decimal Cost { get; set; }
        public string? Name { get; set; }
        public int Quantity { get; set; }

        private PartDto() { }

        public static PartDto ToDto(Part partModel)
        {
            return new PartDto
            {
                Id = partModel.Id,
                Cost = partModel.Cost,
                Name = partModel.Name,
                Quantity = partModel.Quantity
            };
        }

        public static List<PartDto> ToDtos(List<Part> partModels)
        {
            return partModels.ConvertAll(part => ToDto(part));
        }
    }
}
