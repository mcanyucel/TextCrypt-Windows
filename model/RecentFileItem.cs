using System;

namespace TextCrypt.model
{
    internal class RecentFileItem
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime LastOpened { get; set; }
        public bool IsStarred { get; set; }

        public RecentFileItem(string fileName, string filePath, DateTime lastOpened, bool isStarred)
        {
            FileName = fileName;
            FilePath = filePath;
            LastOpened = lastOpened;
            IsStarred = isStarred;
        }

        public override bool Equals(object? obj) => obj is RecentFileItem item && FilePath == item.FilePath;

        public override string ToString() => FileName;

        public override int GetHashCode() => HashCode.Combine(FilePath);
    }
}
