using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Proxies
{
    public abstract class BaseProxy
    {
        private ILogger<BaseProxy> _Logger;
        private BaseProxySettings _Settings;
        protected HttpClient _Client;
        protected HttpResponseMessage _Response;
        protected string _Token;
        protected string _AppName;

        public BaseProxy(ILogger<BaseProxy> logger,
            IOptions<BaseProxySettings> settings,
            HttpClient client)
        {
            _Logger = logger;
            _Settings = settings.Value;
            _Client = client;
            _Client.BaseAddress = new System.Uri(_Settings.BaseUrl);
            _Response = new HttpResponseMessage(System.Net.HttpStatusCode.NotAcceptable);
            _AppName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
        }

        protected async Task<HttpResponseMessage> PostAsync(string endpoint, string jsonParams)
        {
            try
            {
                var content = new StringContent(jsonParams, Encoding.UTF8, "application/json");
                
                _Response = await _Client.PostAsync($"{endpoint}", content);
                return _Response;
            }
            catch (HttpRequestException ex)
            {
                _Logger.LogError($"Error en {_AppName}. Servicio {_Settings.BaseUrl}{endpoint} caído. Error: { ex.Message }");
                _Response = new HttpResponseMessage(System.Net.HttpStatusCode.Conflict);
                return _Response;
            }
        }

        protected async Task<HttpResponseMessage> GetAsync(string endpoint, string queryStringParams)
        {
            try
            {
                _Response = await _Client.GetAsync($"{endpoint}?{queryStringParams}");
                return _Response;
            }
            catch (HttpRequestException ex)
            {
                _Logger.LogError($"Error en {_AppName}. Servicio {_Settings.BaseUrl}{endpoint} caído. Error: { ex.Message }");
                _Response = new HttpResponseMessage(System.Net.HttpStatusCode.Conflict);
                return _Response;
            }
        }

        protected async Task<HttpResponseMessage> GetAsync(string endpoint)
        {
            try
            {
                _Response = await _Client.GetAsync($"{endpoint}");
                return _Response;
            }
            catch (HttpRequestException ex)
            {
                _Logger.LogError($"Error en {_AppName}. Servicio {_Settings.BaseUrl}{endpoint} caído. Error: { ex.Message }");
                _Response = new HttpResponseMessage(System.Net.HttpStatusCode.Conflict);
                return _Response;
            }
        }
    }
}
