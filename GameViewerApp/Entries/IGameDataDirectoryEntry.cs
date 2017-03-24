using System.Collections.Generic;

namespace GameViewerApp.Entries
{
    public interface IGameDataDirectoryEntry : IGameDataEntry
    {
        IGameDataEntry GetEntry(string name);
        IEnumerable<IGameDataEntry> GetEntries();
    }
}