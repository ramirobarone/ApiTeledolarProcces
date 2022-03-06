using Application.Common.Interfaces;
using Domain.Entities.Teledolar.Account;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace Infrastructure.Proxies.Teledolar
{
    public class AccountVerificationTeledolar : BaseTeledolar, IServiceProxyGet
    {
        private readonly ILogger<BaseProxy> logger;
        private readonly IOptions<TeledolarSettings> settings;
        private readonly HttpClient client;
        private readonly IWebHostEnvironment webHostEnvironment;

        public AccountVerificationTeledolar(ILogger<BaseProxy> logger, IOptions<TeledolarSettings> settings, HttpClient client, IWebHostEnvironment webHostEnvironment) : base(logger, settings, client, webHostEnvironment)
        {
            this.logger = logger;
            this.settings = settings;
            this.client = client;
            this.webHostEnvironment = webHostEnvironment;
        }

      

        public async Task<ResponseAccountClient> Get(string iban, string identification, string typeIdentification)
        {
            Authorize();

            string parameters = string.Format(settings.Value.ValidateAccount, iban, identification, typeIdentification);

            var response = await _Client.GetAsync(parameters);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var json = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<ResponseAccountClient>(json);
            }
            return default;
        }
    }
}
