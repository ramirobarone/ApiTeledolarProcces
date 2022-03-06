using Application.Common.Interfaces;
using Domain.Entities.Teledolar.SingleDomiciliation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Proxies.Teledolar
{
    public class SingleDomiciliationProxy : BaseTeledolar, IProxySingleDomiciliation
    {
        private readonly IWebHostEnvironment webHostEnvironment;

        private IOptions<TeledolarSettings> Settings { get; }
        public SingleDomiciliationProxy(ILogger<BaseProxy> logger, IOptions<TeledolarSettings> settings, HttpClient client, IWebHostEnvironment webHostEnvironment) : base(logger, settings, client, webHostEnvironment)
        {
            Settings = settings;
            this.webHostEnvironment = webHostEnvironment;
        }

        public async Task<ResultDomiciliation> ExecuteSingleDomiciliation(ExecuteTransaction executeTransaction)
        {
            var contentJson = JsonSerializer.Serialize(executeTransaction);

            Authorize(contentJson);

            string uri = Settings.Value.SingleDomiciliation;

            await PostAsync(uri, contentJson);

            var Jsonresponse = await _Response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ResultDomiciliation>(Jsonresponse);
        }
       
    }
}
