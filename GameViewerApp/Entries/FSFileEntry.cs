using System.IO;
using System.Threading.Tasks;

namespace GameViewerApp.Entries
{
    internal class FSFileEntry : IGameDataFileEntry
    {
        private FileInfo m_fileInfo;

        public FSFileEntry(string path)
        {
            m_fileInfo = new FileInfo(path);
        }

        public string Name => m_fileInfo.Name;

        public long Size => m_fileInfo.Length;

        public Stream GetStream()
        {
            return m_fileInfo.OpenRead();
        }

        public async Task ExportAsync(Stream stream)
        {
            using (var f = m_fileInfo.OpenRead())
            {
                await f.CopyToAsync(stream);
            }
        }
    }
}