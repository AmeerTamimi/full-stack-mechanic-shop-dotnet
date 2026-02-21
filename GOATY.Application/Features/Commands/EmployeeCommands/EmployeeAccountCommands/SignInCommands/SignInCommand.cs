using MediatR;

namespace GOATY.Application.Features.Commands.UserCommands.SignInCommands
{
    public record class SignInCommand : IRequest<JwtToken> // reponse must be jwt token
}
