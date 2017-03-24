using RageLib.Archives;
using RageLib.GTA5.ArchiveWrappers;
using RageLib.GTA5.Cryptography;
using RageLib.GTA5.Cryptography.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GameViewerApp
{
    internal class GameData
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

    interface IGameDataFileEntry : IGameDataEntry
    {
        long Size { get; }

        Stream GetStream();

        Task ExportAsync(Stream stream);
    }

    public interface IGameDataEntry
    {
        string Name { get; }
    }

    public interface IGameDataDirectoryEntry : IGameDataEntry
    {
        IGameDataEntry GetEntry(string name);
        IEnumerable<IGameDataEntry> GetEntries();
    }

    public class FSDataEntry : IGameDataDirectoryEntry
    {
        private string m_path;

        public string Name => Path.GetFileName(m_path);

        public FSDataEntry(string path)
        {
            m_path = path;
        }

        public IGameDataEntry GetEntry(string name)
        {
            var combinedPath = Path.Combine(m_path, name);
            
            if (Directory.Exists(combinedPath))
            {
                return new FSDataEntry(combinedPath);
            }
            else
            {
                if (Path.GetExtension(combinedPath).EndsWith("rpf"))
                {
                    return ArchiveFile.GetFromCache(combinedPath);
                }

                return new FSFileEntry(combinedPath);
            }
        }

        public IEnumerable<IGameDataEntry> GetEntries()
        {
            var dirs = Directory.GetFiles(m_path);
            var files = Directory.GetDirectories(m_path);

            return dirs.Concat(files).Select(a => a.Replace(m_path, "")).Select(GetEntry);
        }
    }

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

    public class ArchiveDirectoryEntry : IGameDataDirectoryEntry
    {
        private IArchiveDirectory m_directory;

        public string Name => m_directory.Name;

        public ArchiveDirectoryEntry(IArchiveDirectory directory)
        {
            m_directory = directory;
        }

        public IGameDataEntry GetEntry(string name)
        {
            var file = m_directory.GetFile(name);

            if (file != null)
            {
                if (file.Name.EndsWith(".rpf"))
                {
                    var binaryFile = (IArchiveBinaryFile)file;

                    return new ArchiveDirectoryEntry(RageArchiveWrapper7.Open(binaryFile.GetStream(), binaryFile.Name).Root);
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

    public class ArchiveFile
    {
        private static Dictionary<string, ArchiveDirectoryEntry> ms_archiveCache = new Dictionary<string, ArchiveDirectoryEntry>();

        public static ArchiveDirectoryEntry GetFromCache(string path)
        {
            if (ms_archiveCache.TryGetValue(path, out var value))
            {
                return value;
            }

            var entry = new ArchiveDirectoryEntry(RageArchiveWrapper7.Open(File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read), Path.GetFileName(path)).Root);
            ms_archiveCache.Add(path, entry);

            return entry;
        }
    }
}