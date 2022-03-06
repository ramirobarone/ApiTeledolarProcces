namespace Infrastructure.Proxies.Teledolar
{
    public class TeledolarSettings : BaseProxySettings
    {
        public const string AppSettingsSection = "TeledolarSettings";
        public string CreateBatchUrl { get; set; }
        public string GetBatchStatusUrl { get; set; }
        public int channel_id { get; set; }
        public string device_id { get; set; }
        public int enterprise_service_id { get; set; }
        public int transaction_type_id { get; set; }
        public int currency_id { get; set; }
        public string SecretKey { get; set; }
        public string PrivateKeyPath { get; set; }
        public string PublicKeyPath { get; set; }
        public string AdaRegistered { get; set; }
        public string SingleDomiciliation { get; set; }
        public string AccountNumberZenziya { get; set; }
        public string ValidateAccount { get; set; }
    }
}
