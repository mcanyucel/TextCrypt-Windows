using System;
using System.Threading.Tasks;

namespace TextCrypt.service
{
    internal interface IFileService
    {
        internal Task<byte[]> ReadFileAsync(String path);
        internal Task<bool> WriteFileAsync(String path, byte[] data);
    }
}
