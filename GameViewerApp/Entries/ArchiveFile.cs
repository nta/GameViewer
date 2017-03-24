using RageLib.GTA5.ArchiveWrappers;
using System.Collections.Generic;
using System.IO;

namespace GameViewerApp.Entries
{
    public class ArchiveFile
    {
        private static Dictionary<string, ArchiveDirectoryEntry> ms_archiveCache = new Dictionary<string, ArchiveDirectoryEntry>();

        public static ArchiveDirectoryEntry GetFromCache(string path)
        {
            if (ms_archiveCache.TryGetValue(path, out var value))
            {
                return value;
            }

            var entry = new ArchiveDirectoryEntry(RageArchiveWrapper7.Open(File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read), Path.GetFileName(path)).Root, Path.GetFileName(path));
            ms_archiveCache.Add(path, entry);

            return entry;
        }
    }
}