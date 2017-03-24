using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace GameViewerApp.Entries
{
    public static class AliasedDataEntry
    {
        public static IGameDataEntry Create(string alias, IGameDataEntry entry)
        {
            switch (entry)
            {
                case IGameDataDirectoryEntry directoryEntry:
                    return new AliasedDirectoryEntry(alias, directoryEntry);

                case IGameDataFileEntry fileEntry:
                    return new AliasedFileEntry(alias, fileEntry);

                default:
                    throw new ArgumentException("Unsupported entry type.");
            }
        }

        private class AliasedEntry : IGameDataEntry
        {
            public string Name { get; }

            protected IGameDataEntry m_entry;

            public AliasedEntry(string alias, IGameDataEntry entry)
            {
                Name = alias;
                m_entry = entry;
            }
        }

        private class AliasedDirectoryEntry : AliasedEntry, IGameDataDirectoryEntry
        {
            public AliasedDirectoryEntry(string alias, IGameDataDirectoryEntry entry) : base(alias, entry)
            {
            }

            public IEnumerable<IGameDataEntry> GetEntries() => ((IGameDataDirectoryEntry)m_entry).GetEntries();

            public IGameDataEntry GetEntry(string name) => ((IGameDataDirectoryEntry)m_entry).GetEntry(name);
        }

        private class AliasedFileEntry : AliasedEntry, IGameDataFileEntry
        {
            public AliasedFileEntry(string alias, IGameDataFileEntry entry) : base(alias, entry)
            {
            }

            public long Size => ((IGameDataFileEntry)m_entry).Size;

            public Task ExportAsync(Stream stream) => ((IGameDataFileEntry)m_entry).ExportAsync(stream);

            public Stream GetStream() => ((IGameDataFileEntry)m_entry).GetStream();
        }
    }
}
