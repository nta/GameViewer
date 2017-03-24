using RageLib.Archives;
using RageLib.GTA5.ArchiveWrappers;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GameViewerApp.Entries
{
    public class ArchiveDirectoryEntry : IGameDataDirectoryEntry
    {
        private IArchiveDirectory m_directory;
        private string m_nameOverride;

        public string Name => m_nameOverride ?? m_directory.Name;

        public ArchiveDirectoryEntry(IArchiveDirectory directory, string nameOverride = null)
        {
            m_directory = directory;
            m_nameOverride = nameOverride;
        }

        public IGameDataEntry GetEntry(string name)
        {
            var file = m_directory.GetFile(name);

            if (file != null)
            {
                if (file.Name.EndsWith(".rpf"))
                {
                    var binaryFile = (IArchiveBinaryFile)file;

                    return new ArchiveDirectoryEntry(RageArchiveWrapper7.Open(binaryFile.GetStream(), binaryFile.Name).Root, binaryFile.Name);
                }

                return new ArchiveFileEntry(file);
            }

            var directory = m_directory.GetDirectory(name);

            if (directory != null)
            {
                return new ArchiveDirectoryEntry(directory);
            }

            throw new FileNotFoundException();
        }

        public IEnumerable<IGameDataEntry> GetEntries()
        {
            var dirs = m_directory.GetDirectories().Select(a => new ArchiveDirectoryEntry(a));
            var files = m_directory.GetFiles().Select(a => new ArchiveFileEntry(a));

            return dirs.Cast<IGameDataEntry>().Concat(files);
        }
    }
}