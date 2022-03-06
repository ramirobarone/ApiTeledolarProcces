using Microsoft.Extensions.Options;

namespace Infrastructure.Proxies.Teledolar
{
    public class GetConfigurations : TeledolarSettings
    {
        private readonly IOptions<TeledolarSettings> options;

        public GetConfigurations(IOptions<TeledolarSettings> options)
        {
            this.options = options;
            LoadDataInFields();
        }

        private void LoadDataInFields()
        {
            transaction_type_id = options.Value.transaction_type_id;
            channel_id = options.Value.channel_id;
            device_id = options.Value.device_id;
            enterprise_service_id = options.Value.enterprise_service_id;
            //transaction_type_id = transaction_type_id;
            currency_id = options.Value.currency_id;
        }
    }
}
