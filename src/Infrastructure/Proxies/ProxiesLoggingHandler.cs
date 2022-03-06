using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Proxies
{
    public class ProxiesLoggingHandler : DelegatingHandler
    {
        private readonly ILogger _logger;

        public ProxiesLoggingHandler(ILogger logger)
        {
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);
            var jsonResponse = JsonConvert.SerializeObject(await response.Content.ReadAsStringAsync());
            var jsonRequest = request.Method == HttpMethod.Get ? request.RequestUri.Query : JsonConvert.SerializeObject(await request.Content.ReadAsStringAsync());
            _logger.LogInformation("Api-Petition: Url: {Url}, Request: {Request}, Response: {Response}",
                request.RequestUri.ToString(), jsonRequest, jsonResponse);
            return response;
        }
    }
}
