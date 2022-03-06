namespace Infrastructure.Proxies
{
    public abstract class BaseProxySettings
    {
        public string BaseUrl { get; set; }
        public string AuthEndpoint { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string RegisterAda { get; set; }
    }
}
