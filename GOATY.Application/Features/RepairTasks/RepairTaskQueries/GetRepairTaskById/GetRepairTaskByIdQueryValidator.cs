using FluentValidation;

namespace GOATY.Application.Features.RepairTasks.RepairTaskQueries.GetRepairTaskById
{
    public sealed class GetRepairTaskByIdQueryValidator : AbstractValidator<GetRepairTaskByIdQuery>
    {
        public GetRepairTaskByIdQueryValidator()
        {
            RuleFor(x => x.Id)
            .NotNull().WithMessage("Id is required.")
            .NotEqual(Guid.Empty).WithMessage("Id must not be empty.");
        }
    }
}
