using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.OpenSsl;

namespace Infrastructure.Proxies.Teledolar
{
    public class BaseTeledolar : BaseProxy {
        private ILogger<DomiciliacionesTeledolarProxy> _Logger;
        private TeledolarSettings _Settings;
        private readonly IWebHostEnvironment _WebHostEnvironment;

        public BaseTeledolar (ILogger<BaseProxy> logger, IOptions<TeledolarSettings> settings, HttpClient client, IWebHostEnvironment webHostEnvironment) : base (logger, settings, client) {
            _Settings = settings.Value;
            _WebHostEnvironment = webHostEnvironment;
        }
        public string GetConstSettings () => JsonConvert.SerializeObject (new { _Settings.channel_id, _Settings.currency_id, _Settings.device_id, _Settings.enterprise_service_id, _Settings.transaction_type_id });

        protected void Authorize (string json = "") {
            DateTimeOffset ExpirationTime = DateTime.UtcNow;
            long unixTimeStampInSeconds = ExpirationTime.ToUnixTimeSeconds ();
            string time = unixTimeStampInSeconds.ToString ();

            byte[] bytes = Encoding.UTF8.GetBytes ($"{time}|{json}");
            SHA256Managed hashstring = new();
            byte[] Sha256key = hashstring.ComputeHash (bytes);

#if DEBUG

#endif

            //var basePath = Path.GetFullPath (Path.Combine (_WebHostEnvironment.ContentRootPath, @"../"));
            // Encrypt
            string basePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            var encryptedWithPrivate = RsaEncryptWithPrivate (Sha256key, Path.Combine (basePath, _Settings.PrivateKeyPath));
            // Decrypt
            var decryptedWithPublic = RsaDecryptWithPublic (encryptedWithPrivate, Path.Combine (basePath, _Settings.PublicKeyPath));

            //Se convierte a Bytes para generar el Hexadecimal.
            byte[] BytePrivateKey = Convert.FromBase64String (encryptedWithPrivate);

            string HexPrivateKey = BitConverter.ToString (BytePrivateKey).Replace ("-", string.Empty);

            _Client.DefaultRequestHeaders.Accept.Add (
                new MediaTypeWithQualityHeaderValue ("application/json"));
            _Client.DefaultRequestHeaders.Add ("api-key", _Settings.SecretKey);
            _Client.DefaultRequestHeaders.Add ("signature", HexPrivateKey);
            _Client.DefaultRequestHeaders.Add ("signature-timestamp", time);
        }

        private string RsaEncryptWithPrivate (byte[] clearText, string privateKey) {
            var bytesToEncrypt = clearText;

            var encryptEngine = new Pkcs1Encoding (new RsaEngine ());

            using (var txtreader = System.IO.File.OpenText (privateKey)) {
                var keyPair = (AsymmetricCipherKeyPair) new PemReader (txtreader).ReadObject ();

                encryptEngine.Init (true, keyPair.Private);
            }

            var encrypted = Convert.ToBase64String (encryptEngine.ProcessBlock (bytesToEncrypt, 0, bytesToEncrypt.Length));
            return encrypted;
        }

        private string RsaDecryptWithPublic (string base64Input, string publicKey) {
            var bytesToDecrypt = Convert.FromBase64String (base64Input);

            var decryptEngine = new Pkcs1Encoding (new RsaEngine ());

            using (var txtreader = System.IO.File.OpenText (publicKey) /*new StringReader(publicKey)*/ ) {
                var keyParameter = (AsymmetricKeyParameter) new PemReader (txtreader).ReadObject ();

                decryptEngine.Init (false, keyParameter);
            }

            var decrypted = Convert.ToBase64String (decryptEngine.ProcessBlock (bytesToDecrypt, 0, bytesToDecrypt.Length));
            return decrypted;
        }
    }
}
