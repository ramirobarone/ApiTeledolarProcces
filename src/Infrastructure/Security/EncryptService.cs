using Application.Common.Interfaces;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Security
{
    public class EncryptService : IEncryptService
    {
        public CryptoStream EncryptStream(Stream responseStream)
        {
            Aes aes = GetEncryptionAlgorithm();

            ToBase64Transform base64Transform = new ToBase64Transform();
            CryptoStream base64EncodedStream = new CryptoStream(responseStream, base64Transform, CryptoStreamMode.Write);
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            CryptoStream cryptoStream = new CryptoStream(base64EncodedStream, encryptor, CryptoStreamMode.Write);

            return cryptoStream;
        }
        public Stream DecryptStream(Stream cipherStream)
        {
            Aes aes = GetEncryptionAlgorithm();

            FromBase64Transform base64Transform = new FromBase64Transform(FromBase64TransformMode.IgnoreWhiteSpaces);
            CryptoStream base64DecodedStream = new CryptoStream(cipherStream, base64Transform, CryptoStreamMode.Read);
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            CryptoStream decryptedStream = new CryptoStream(base64DecodedStream, decryptor, CryptoStreamMode.Read);
            return decryptedStream;
        }

        public string EncryptString(string plainText)
        {
            Aes aes = GetEncryptionAlgorithm();
            byte[] inputBytes = UTF8Encoding.UTF8.GetBytes(plainText);

            byte[] result = null;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(aes.Key, aes.IV), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(inputBytes, 0, inputBytes.Length);
                    cryptoStream.FlushFinalBlock();

                    result = memoryStream.ToArray();
                }
            }

            return Convert.ToBase64String(result, 0, result.Length);
        }
        public string DecryptString(string cipherText)
        {
            Aes aes = GetEncryptionAlgorithm();
            byte[] buffer = Convert.FromBase64String(cipherText);
            string plaintext = string.Empty;
            using (MemoryStream memoryStream = new MemoryStream(buffer))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(aes.Key, aes.IV), CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(cryptoStream))
                    {
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
            return plaintext;
        }
        private Aes GetEncryptionAlgorithm()
        {
            Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(string.Empty);
            aes.IV = Encoding.UTF8.GetBytes(string.Empty);
            aes.Mode = CipherMode.CBC;
            //aes.Padding = PaddingMode.None;
            return aes;
        }
    }
}
