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
            var entries = new List<IGameDataEntry>();

            foreach (var dir in m_directories)
            {
                try
                {
                    var entry = dir.GetEntry(name);

                    if (entry != null)
                    {
                        entries.Add(entry);
                    }
                }
                catch { }
            }

            // early out for 0 entries
            if (entries.Count == 0)
            {
                return null;
            }

            // early out for a single entry
            if (entries.Count == 1)
            {
                return entries[0];
            }

            // if these aren't directories, fail
            if (!entries.All(a => a is IGameDataDirectoryEntry))
            {
                return null;
            }

            // return another merged entry
            return new ConcatDirectoryEntry(entries.First().Name, entries.Cast<IGameDataDirectoryEntry>());
        }
    }
}