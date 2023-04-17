using System.Threading.Tasks;

namespace TextCrypt.service
{
    internal interface IEncryptionService
    {
        internal Task<string?> EncryptAsync(string text, string password);
        internal Task<string?> DecryptAsync(byte[] encryptedByteArray, string password);
    }
}
