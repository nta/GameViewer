using RageLib.GTA5.Cryptography;
using RageLib.GTA5.Cryptography.Helpers;
using System;

using GameViewerApp.Entries;

namespace GameViewerApp
{
    public class GameData
    {
        private string m_gameDir;

        public GameData()
        {
            var gameDir = @"S:\Five\V";
            LoadGameState(gameDir);
        }

        private void LoadGameState(string gameDir)
        {
            GTA5Constants.LoadFromPath(@"S:\five\tdt");

            m_gameDir = gameDir;
        }

        public IGameDataEntry GetEntryAtPath(string path)
        {
            // resolve the path, portion by portion
            var parts = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            IGameDataDirectoryEntry entry = new FSDataEntry(m_gameDir);

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