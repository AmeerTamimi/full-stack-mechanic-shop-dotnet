using GOATY.Domain.WorkOrders.Enums;

namespace GOATY.Application.Features.Schedule.DTOs
{
    public sealed class BayDto
    {
        public Bay Bay { get; set; }
        public List<AvailabilityBayDto> Slots { get; set; } = [];
    }
}
