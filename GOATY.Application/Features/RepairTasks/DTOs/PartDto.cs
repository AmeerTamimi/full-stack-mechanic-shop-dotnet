namespace GOATY.Application.Features.RepairTasks.DTOs
{
    public class PartDto
    {
        public Guid Id { get; set; }
        public decimal Cost { get; set; }
        public string? Name { get; set; }
        public int Quantity { get; set; }
    }
}
