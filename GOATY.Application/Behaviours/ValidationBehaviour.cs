using FluentValidation;
using GOATY.Domain.Common.Results;
using MediatR;
using System.Threading;

namespace GOATY.Application.Behaviours
{
    public sealed class ValidationBehaviour<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) :
        IPipelineBehavior<TRequest, Result<TResponse>>
        where TRequest : notnull
    {

        public async Task<Result<TResponse>> Handle(
            TRequest request,
            RequestHandlerDelegate<Result<TResponse>> next,
            CancellationToken ct)
        {
            if (!validators.Any())
                return await next();

            var context = new ValidationContext<TRequest>(request);

            var results = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, ct)));

            var failures = results.SelectMany(r => r.Errors).Where(e => e is not null).ToList();

            if (failures.Count != 0)
                return Error.Validation(code:"Validation" , description:"Nigga");

            return await next();
        }
    }
}
