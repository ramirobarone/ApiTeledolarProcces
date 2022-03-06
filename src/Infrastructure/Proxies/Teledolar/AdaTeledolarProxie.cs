using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Entities.Teledolar.Ada;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Proxies.Teledolar {
    public partial class AdaTeledolarProxie : BaseTeledolar, IAdaProxy {
        private ILogger<AdaTeledolarProxie> _Logger;
        private TeledolarSettings _Settings;
        private readonly IWebHostEnvironment WebHostEnvironment;
        public AdaTeledolarProxie (ILogger<AdaTeledolarProxie> logger, IOptions<TeledolarSettings> settings, HttpClient client, IWebHostEnvironment webHostEnvironment) : base (logger, settings, client, webHostEnvironment) {
            _Logger = logger;
            _Settings = settings.Value;
            WebHostEnvironment = webHostEnvironment;
        }

        public async Task<ResponseAda> RegisterAda (RequestAda ada) {
            string jsonRequest = JsonSerializer.Serialize (ada);

            Authorize (jsonRequest);

            string url = _Settings.RegisterAda;
            var content = new StringContent (jsonRequest);

            await PostAsync (url, jsonRequest);

            if (_Response.StatusCode == System.Net.HttpStatusCode.OK) {

            }

            var contentString = await _Response.Content.ReadAsStringAsync ();
            return JsonSerializer.Deserialize<ResponseAda> (contentString);
        }

        public async Task<ResponseAda> StatusAda (string Identification) {
            string url = _Settings.AdaRegistered;

            await GetAsync (url);

            var contentString = await _Response.Content.ReadAsStringAsync ();

            return JsonSerializer.Deserialize<ResponseAda> (contentString);
        }
    }
}
