using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TextCrypt.model;

namespace TextCrypt.service
{
    internal interface IFileService
    {
        internal Task<byte[]> ReadFileAsync(String path);
        internal Task<bool> WriteFileAsync(String path, byte[] data);

        internal Task<List<RecentFileItem>> GetRecentFilesAsync();

        internal Task<bool> SaveRecentFilesAsync(List<RecentFileItem> recentFiles);

        internal string GetFileName(string path);
    }
}
