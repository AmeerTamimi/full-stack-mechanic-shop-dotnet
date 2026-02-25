namespace GOATY.Application.Features.RepairTasks.DTOs
{
    public sealed class RepairTaskDto
    {
        private List<RepairTaskDetailsDto> repairTaskDetailsDtos;

        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal TimeEstimated { get; set; }
        public decimal CostEstimated { get; set; }
        public List<RepairTaskDetailsDto> RepairTasksDetailsDto { get; set; }
    }
}
