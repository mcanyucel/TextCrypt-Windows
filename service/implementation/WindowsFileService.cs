using System;
using System.IO;
using System.Threading.Tasks;

namespace TextCrypt.service.implementation
{
    internal class WindowsFileService : IFileService
    {
        async Task<byte[]> IFileService.ReadFileAsync(string path)
        {
            return await File.ReadAllBytesAsync(path);
        }

        async Task<bool> IFileService.WriteFileAsync(string path, byte[] data)
        {
            var success = true;

            try
            {
                await File.WriteAllBytesAsync(path, data);
            }
            catch (Exception)
            {
                success = false;
            }

            return success;
        }
    }
}
