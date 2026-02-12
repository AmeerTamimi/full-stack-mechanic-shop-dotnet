using FluentValidation;
using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Behaviours
{
    public sealed class ValidationBehaviour<TRequest, TResponse>(IValidator<TRequest>? validator) :
        IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken ct)
        {
            // i wanna check if there are any errors
            // errors are coming from the validators on this assembly for this specific TRequest
            // if there errors , i will return Result<error> , else nothing (just pass it)
            
            if(validator is null)
            {
                return await next(ct);
            }

            var validationResult = await validator.ValidateAsync(request);

            if (validationResult.IsValid)
            {
                return await next(ct);
            }

            var errors = validationResult.Errors
                .ConvertAll(error => Error.Validation(
                    code: error.PropertyName,
                    description: error.ErrorMessage));

            return (dynamic) errors;
        }
    }
}
