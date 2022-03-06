using Application.Common.Interfaces;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Behaviours
{
    public class RequestResponseLogger<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse>
    {
        private readonly ILogger _logger;
        private readonly ICurrentUserService _currentUserService;

        public RequestResponseLogger(ILogger<TRequest> logger, ICurrentUserService currentUserService)
        {
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
        {
            var name = typeof(TRequest).Name;

            _logger.LogInformation("DB-Petition: Name: {Name}, Request: {Request}, Response: {Response}",
                name, JsonConvert.SerializeObject(request), JsonConvert.SerializeObject(response));

            return Task.CompletedTask;
        }
    }
}
