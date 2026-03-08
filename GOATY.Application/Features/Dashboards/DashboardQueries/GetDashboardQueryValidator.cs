using FluentValidation;

namespace GOATY.Application.Features.Dashboards.DashboardQueries
{
    public sealed class GetDashboardQueryValidator : AbstractValidator<GetDashboardQuery>
    {
        public GetDashboardQueryValidator()
        {
            RuleFor(x => x.Day)
            .NotEmpty()
            .WithMessage("day is required");

            RuleFor(x => x.TimeZone)
                .NotNull()
                .WithMessage("time zone is required");
        }
    }
}
