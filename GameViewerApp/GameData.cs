using RageLib.GTA5.Cryptography;
using RageLib.GTA5.Cryptography.Helpers;
using System;

using GameViewerApp.Entries;
using System.Linq;

namespace GameViewerApp
{
    public class GameData
    {
        private VirtualDirectoryEntry m_root;

        public GameData()
        {
            m_root = new VirtualDirectoryEntry("");

            var gameDir = @"S:\Five\V";
            LoadGameState(gameDir);
        }

        private void LoadGameState(string gameDir)
        {
            GTA5Constants.LoadFromPath(@"S:\five\tdt");

            var localDir = new FSDataEntry(gameDir);
            m_root.AddEntry("local", localDir);

            m_root.AddEntry(new ConcatDirectoryEntry("common",
                new[]
                {
                    localDir.GetDirectoryEntry("common.rpf"),
                    localDir.GetDirectoryEntry("update")
                        .GetDirectoryEntry("update.rpf")
                            .GetDirectoryEntry("common")
                }
            ));

            m_root.AddEntry(new ConcatDirectoryEntry("platform",
                localDir.GetEntries()
                    .Where(a => a.Name.StartsWith("x64") && a is IGameDataDirectoryEntry)
                    .Cast<IGameDataDirectoryEntry>()
                .Append(localDir
                    .GetDirectoryEntry("update")
                        .GetDirectoryEntry("x64"))
                .Append(localDir
                    .GetDirectoryEntry("update")
                        .GetDirectoryEntry("update.rpf")
                            .GetDirectoryEntry("x64"))
                .Reverse()
            ));
        }

        public IGameDataEntry GetEntryAtPath(string path)
        {
            // resolve the path, portion by portion
            var parts = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            IGameDataDirectoryEntry entry = m_root;

            foreach (var part in parts)
            {
                if (part[0] == '.')
                {
                    continue;
                }

                var thisEntry = entry.GetEntry(part);

                if (thisEntry is IGameDataFileEntry file)
                {
                    return file;
                }


                entry = (thisEntry as IGameDataDirectoryEntry);
            }

            return entry;
        }
    }
}