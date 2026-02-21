using GOATY.Application.Features.Common;
using GOATY.Application.Jwt;
using GOATY.Domain.Common.Results;
using GOATY.Domain.Employees;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GOATY.Application.Features.Commands.EmployeeCommands.EmployeeAccountCommands.SignInCommands
{
    public sealed class SignInCommandHandler(IAppDbContext context) : IRequestHandler<SignInCommand, Result<JwtResponse>>
    {
        public async Task<Result<JwtResponse>> Handle(SignInCommand request, CancellationToken ct)
        {
            var employee = await context.Employees.SingleOrDefaultAsync(
                                                           emp => emp.Email == request.Email,
                                                           ct
                                                        );

            if(employee is null)
            {
                return EmployeeErrors.EmailNotRegisterd;
            }
        }
    }
}
