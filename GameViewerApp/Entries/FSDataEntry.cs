using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GameViewerApp.Entries
{
    public class FSDataEntry : IGameDataDirectoryEntry
    {
        private string m_path;

        public string Name => Path.GetFileName(m_path);

        public FSDataEntry(string path)
        {
            m_path = path;
        }

        public IGameDataEntry GetEntry(string name)
        {
            var combinedPath = Path.Combine(m_path, name);
            
            if (Directory.Exists(combinedPath))
            {
                return new FSDataEntry(combinedPath);
            }
            else
            {
                if (File.Exists(combinedPath))
                {
                    if (Path.GetExtension(combinedPath).EndsWith("rpf"))
                    {
                        return ArchiveFile.GetFromCache(combinedPath);
                    }

                    return new FSFileEntry(combinedPath);
                }
                else
                {
                    return null;
                }
            }
        }

        public IEnumerable<IGameDataEntry> GetEntries()
        {
            var dirs = Directory.GetFiles(m_path);
            var files = Directory.GetDirectories(m_path);

            return dirs.Concat(files).Select(a => a.Replace(m_path, "").Substring(1)).Select(GetEntry);
        }
    }
}