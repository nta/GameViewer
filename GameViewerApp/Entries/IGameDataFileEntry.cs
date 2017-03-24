using System.IO;
using System.Threading.Tasks;

namespace GameViewerApp.Entries
{
    public interface IGameDataFileEntry : IGameDataEntry
    {
        long Size { get; }

        Stream GetStream();

        Task ExportAsync(Stream stream);
    }
}