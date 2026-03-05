using FluentValidation;

namespace GOATY.Application.Features.Schedule.ScheduleQueries.GetTechnicianSchedule
{
    public sealed class GetTechnicianScheduleQueryValidator : AbstractValidator<GetTechnicianScheduleQuery>
    {
        public GetTechnicianScheduleQueryValidator()
        {
            RuleFor(x => x.Day)
                .NotEmpty()
                .WithMessage("Day is required.")
                .Must(d => d.Year is >= 2000 and <= 2100)
                .WithMessage("Day must be a valid calendar date.");
        }
    }
}
