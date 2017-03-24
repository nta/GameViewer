using System;
using System.Collections.Generic;

namespace GameViewerApp.Entries
{
    public class VirtualDirectoryEntry : IGameDataDirectoryEntry
    {
        private readonly Dictionary<string, IGameDataEntry> m_entries = new Dictionary<string, IGameDataEntry>();

        public VirtualDirectoryEntry(string name)
        {
            Name = name;
        }

        public void AddEntry(IGameDataEntry entry)
        {
            m_entries.Add(entry.Name, entry);
        }

        public void AddEntry(string alias, IGameDataEntry entry)
        {
            AddEntry(AliasedDataEntry.Create(alias, entry));
        }

        // IGameDataDirectoryEntry
        public string Name { get; }

        public IEnumerable<IGameDataEntry> GetEntries()
        {
            return m_entries.Values;
        }

        public IGameDataEntry GetEntry(string name)
        {
            if (m_entries.TryGetValue(name, out var value))
            {
                return value;
            }

            return null;
        }
    }
}
