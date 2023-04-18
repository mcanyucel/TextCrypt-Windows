using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using TextCrypt.model;

namespace TextCrypt.service.implementation
{
    internal class WindowsFileService : IFileService
    {
        private const string recentFilesList = "recentFiles.json";

        async Task<List<RecentFileItem>> IFileService.GetRecentFilesAsync()
        {
            try
            {
                var recentFilesString = await File.ReadAllTextAsync(recentFilesList);
                return JsonSerializer.Deserialize<List<RecentFileItem>>(recentFilesString) ?? new List<RecentFileItem>();
            }
            catch { return new List<RecentFileItem>(); }

        }

        async Task<bool> IFileService.SaveRecentFilesAsync(List<RecentFileItem> recentFiles)
        {
            bool success = true;
            try
            {
                var jsonString = JsonSerializer.Serialize(recentFiles);
                await File.WriteAllTextAsync(recentFilesList, jsonString);
            }
            catch (Exception e) 
            { 
                Debug.Write(e.Message);
                success = false; 
            }
            return success;
        }

        async Task<byte[]> IFileService.ReadFileAsync(string path)
        {
            byte[] result;
            try
            {
                result = await File.ReadAllBytesAsync(path);
            }
            catch { result = Array.Empty<byte>(); }
            return result;
        }

        async Task<bool> IFileService.WriteFileAsync(string path, byte[] data)
        {
            var success = true;
            try
            {
                await File.WriteAllBytesAsync(path, data);
            }
            catch { success = false; }
            return success;
        }

        string IFileService.GetFileName(string path)
        {
            return Path.GetFileName(path);
        }
    }
}
