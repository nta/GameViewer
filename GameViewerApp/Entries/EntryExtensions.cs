namespace GameViewerApp.Entries
{
    public static class EntryExtensions
    {
        public static IGameDataFileEntry GetFileEntry(this IGameDataDirectoryEntry entry, string name) =>
            (entry.GetEntry(name) as IGameDataFileEntry);

        public static IGameDataDirectoryEntry GetDirectoryEntry(this IGameDataDirectoryEntry entry, string name) =>
            (entry.GetEntry(name) as IGameDataDirectoryEntry);
    }
}
