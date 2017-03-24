using System;
using System.Collections.Generic;
using System.Linq;

namespace GameViewerApp.Entries
{
    public class ConcatDirectoryEntry : IGameDataDirectoryEntry
    {
        private IEnumerable<IGameDataDirectoryEntry> m_directories;

        public ConcatDirectoryEntry(string name, IEnumerable<IGameDataDirectoryEntry> directories)
        {
            Name = name;

            m_directories = directories;
        }

        public void Add(IGameDataDirectoryEntry entry)
        {
            m_directories = m_directories.Append(entry);
        }

        public void Add(IEnumerable<IGameDataDirectoryEntry> entries)
        {
            m_directories = m_directories.Concat(entries);
        }

        public string Name { get; }

        public IEnumerable<IGameDataEntry> GetEntries() => 
            m_directories
                .SelectMany(a => a.GetEntries())
                .GroupBy(a => a.Name)
                .Select(a => a.First());

        public IGameDataEntry GetEntry(string name)
        {
            foreach (var dir in m_directories)
            {
                try
                {
                    var entry = dir.GetEntry(name);

                    if (entry != null)
                    {
                        return entry;
                    }
                }
                catch { }
            }

            return null;
        }
    }
}
