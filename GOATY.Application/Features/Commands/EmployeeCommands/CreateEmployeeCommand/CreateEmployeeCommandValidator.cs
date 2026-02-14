using FluentValidation;

namespace GOATY.Application.Features.Commands.EmployeeCommands.CreateEmployeeCommand
{
    public sealed class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
    {
        public CreateEmployeeCommandValidator()
        {
            RuleFor(p => p.FirstName)
                .NotEmpty().WithMessage("Employee First Name Cant Be Empty")
                .MaximumLength(100).WithMessage("Employee First Name Must Be <= 100 Charachters Long");

            RuleFor(p => p.LastName)
                .NotEmpty().WithMessage("Employee Last Name Cant Be Empty")
                .MaximumLength(100).WithMessage("Employee Last Name Must Be <= 100 Charachters Long");

            RuleFor(p => p.Role)
                .IsInEnum()
                .WithMessage("Invalid Employee Role");
        }
    }
}
