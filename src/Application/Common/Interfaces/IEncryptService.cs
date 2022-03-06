using System.IO;
using System.Security.Cryptography;

namespace Application.Common.Interfaces
{
    public interface IEncryptService
    {
        CryptoStream EncryptStream(Stream responseStream);
        Stream DecryptStream(Stream cipherStream);
        string EncryptString(string plainText);
        string DecryptString(string cipherText);

    }
}
