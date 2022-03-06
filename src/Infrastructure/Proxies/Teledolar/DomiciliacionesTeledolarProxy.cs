using Application.Common.Interfaces;
using Domain.Entities.Teledolar.CreateBatch;
using Domain.Entities.Teledolar.GetBatch;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace Infrastructure.Proxies.Teledolar
{
    public class DomiciliacionesTeledolarProxy : BaseTeledolar, ITeledolarProxy
    {
        private ILogger<DomiciliacionesTeledolarProxy> _Logger;
        private TeledolarSettings _Settings;
        private readonly IWebHostEnvironment _WebHostEnvironment;

        public DomiciliacionesTeledolarProxy(
            ILogger<DomiciliacionesTeledolarProxy> logger,
            IOptions<TeledolarSettings> settings,
            HttpClient client, IWebHostEnvironment WebHostEnvironment) : base(logger, settings, client, WebHostEnvironment)
        {
            _Logger = logger;
            _Settings = settings.Value;
            _WebHostEnvironment = WebHostEnvironment;
        }
        public async Task<BatchResponse> CreateBatch(BatchRequest request)
        {
            Authorize(JsonConvert.SerializeObject(request));
            return new BatchResponse();
        }
        public async Task<Response_Domiciliacion> GetBatchStatus(string batchId, int pageSize, int page)
        {
            Authorize();
            return new Response_Domiciliacion();
        }
    }
}

/*
/api/signed/v2/accounts/client_accounts/get_enterprise_service_client_account
/api/signed/v2/transactions/get_by_reference
/api/signed/v2/bank_accounts/147915/transactions?page_size=param1&page=param2&transaction_type_id=param3&transaction_state_id=param4&movement_type_id=param5&start_date=param6&end_date=param7
/api/signed/v2/accounts/client_accounts/bccr_info?number=param1&identification=param2&identification_type_id=param3
/api/signed/v2/accounts/client_accounts/unregister_ada_enterprise_service_client_account
/api/signed/v2/accounts/client_accounts/register_ada_enterprise_service_client_account
/api/signed/v2/transactions
/api/signed/v2/transactions/evaluate
584efe54-75bb-4774-91db-9c4e0f9dd063
~/Content/key/ZenziyaKeyPrivate.pem
/api/signed/v2/accounts/client_accounts/register
/api/signed/v2/transactions/create_batch
/api/signed/v2/transactions/batch/:batch_id?page_size=param1&page=param2
/api/signed/v2/accounts/client_accounts?client_account_type_id=param1&client_account_dtr_state_id=param2&identification=param3&identification_type_id=param4&number=param5
*/
