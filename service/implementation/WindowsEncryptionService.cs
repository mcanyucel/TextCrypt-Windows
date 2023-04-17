using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TextCrypt.service.implementation
{
    internal class WindowsEncryptionService : IEncryptionService
    {
        async Task<string?> IEncryptionService.DecryptAsync(byte[] encryptedByteArray, string password) =>

            await Task.Run(() =>
            {
                try
                {
                    var passwordSaltBytes = encryptedByteArray.Take(32);
                    byte[] passwordByteArray = Encoding.UTF8.GetBytes(password);
                    var saltedPasswordByteArray = passwordSaltBytes.Concat(passwordByteArray).ToArray();
                    var passwordHash = SHA256.HashData(saltedPasswordByteArray);
                    var iv = encryptedByteArray.Skip(32).Take(16).ToArray();
                    using Aes aes = Aes.Create();
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.IV = iv;
                    aes.Key = passwordHash;
                    using ICryptoTransform decryptor = aes.CreateDecryptor(passwordHash, iv);
                    var cipherTextBytes = encryptedByteArray.Skip(48).ToArray();
                    var plainTextBytes = decryptor.TransformFinalBlock(cipherTextBytes, 0, cipherTextBytes.Length);
                    return Encoding.UTF8.GetString(plainTextBytes);
                }
                catch (CryptographicException)
                {
                    return null;
                }
            });


        async Task<byte[]?> IEncryptionService.EncryptAsync(string text, string password) =>

            await Task.Run(() =>
            {
                try
                {
                    var messageBytes = Encoding.UTF8.GetBytes(text);
                    var passwordSaltBytes = RandomNumberGenerator.GetBytes(32);
                    byte[] passwordByteArray = Encoding.UTF8.GetBytes(password);
                    var saltedPasswordByteArray = passwordSaltBytes.Concat(passwordByteArray).ToArray();
                    var passwordHash = SHA256.HashData(saltedPasswordByteArray);
                    var iv = RandomNumberGenerator.GetBytes(16);
                    using Aes aes = Aes.Create();
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.IV = iv;
                    aes.Key = passwordHash;
                    using ICryptoTransform encryptor = aes.CreateEncryptor(passwordHash, iv);
                    var encryptedMessageBytes = encryptor.TransformFinalBlock(messageBytes, 0, messageBytes.Length);
                    return passwordSaltBytes.Concat(iv).Concat(encryptedMessageBytes).ToArray();
                }
                catch (CryptographicException)
                {
                    return null;
                }
            });

    }
}
