using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace GOATY.Application.Common.Behaviours
{
    public sealed class PerformanceBehaviour<TRequest, TResponse>(ILogger<TRequest> logger)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
        {
            var timer = Stopwatch.StartNew();

            timer.Start();

            var response = await next(ct);

            timer.Stop();

            var elapsedMilliseconds = timer.ElapsedMilliseconds;

            if(elapsedMilliseconds > 500)
            {
                var requestName = typeof(TRequest).Name;

                logger.LogWarning("Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@Request}",
                                  requestName,
                                  elapsedMilliseconds,
                                  request);
            }

            return response;
        }
    }
}
