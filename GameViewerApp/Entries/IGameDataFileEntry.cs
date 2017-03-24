using System.IO;
using System.Threading.Tasks;

namespace GameViewerApp.Entries
{
    interface IGameDataFileEntry : IGameDataEntry
    {
        long Size { get; }

        Stream GetStream();

        Task ExportAsync(Stream stream);
    }
}