namespace GOATY.Application.Features.Schedule.DTOs
{
    public class ScheduleDto
    {
        public DateOnly Date { get; set; }
        public List<BayDto> Bays{ get; set; } = [];
    }
}
