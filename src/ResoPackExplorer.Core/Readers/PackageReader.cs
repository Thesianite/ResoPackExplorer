using System.Drawing;
using SharpCompress.Archives;

namespace ResoPackExplorer.Core.Readers
{
    public class PackageReader : IDisposable
    {
        private readonly IArchive _archive;
        private bool _disposed = false;
        
        public PackageReader(string packagePath)
        {
            _archive = ArchiveFactory.OpenArchive(packagePath);
        }

        public Stream? GetEntryStream(string entryKey)
        {
            var entry = _archive.Entries
            .FirstOrDefault(e => !e.IsDirectory && e.Key.Equals(entryKey, StringComparison.OrdinalIgnoreCase));

            return entry?.OpenEntryStream();
        }

        public bool HasEntry(string entryKey)
        {
            return _archive.Entries
            .Any(e => !e.IsDirectory && e.Key.Equals(entryKey, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<(string Key, long Size)> GetEntries()
        {
            return _archive.Entries
                .Where(e => !e.IsDirectory)
                .Select(e => (Key: e.Key ?? "Unkown", Size: e.Size));
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _archive?.Dispose();
                _disposed = true;
            }
        }
    }
}