using GOATY.Domain.Parts;

namespace GOATY.Application.DTOs
{
    public class PartDto
    {
        public decimal Cost { get; set; }
        public string? Name { get; set; }
        public int Quantity { get; set; }
    }
}
