using RageLib.Archives;
using System.IO;
using System.Threading.Tasks;

namespace GameViewerApp.Entries
{
    public class ArchiveFileEntry : IGameDataFileEntry
    {
        private IArchiveFile m_file;

        public ArchiveFileEntry(IArchiveFile file)
        {
            m_file = file;
        }

        public string Name => m_file.Name;
        public long Size => m_file.Size;

        public Stream GetStream()
        {
            var ms = new MemoryStream();
            m_file.Export(ms);
            ms.Position = 0;
            return ms;
        }

        public async Task ExportAsync(Stream stream)
        {
            await m_file.ExportAsync(stream);
        }
    }
}