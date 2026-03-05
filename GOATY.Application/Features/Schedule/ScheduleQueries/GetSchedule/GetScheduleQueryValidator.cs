using FluentValidation;

namespace GOATY.Application.Features.Schedule.ScheduleQueries.GetSchedule
{
    public sealed class GetScheduleQueryValidator : AbstractValidator<GetScheduleQuery>
    {
        public GetScheduleQueryValidator()
        {
            RuleFor(x => x.Day)
                .NotEmpty()
                .WithMessage("Day is required.")
                .Must(d => d.Year is >= 2000 and <= 2100)
                .WithMessage("Day must be a valid calendar date.");
        }
    }
}
