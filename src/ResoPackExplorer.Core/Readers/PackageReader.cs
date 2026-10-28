using SharpCompress.Archives;

namespace ResoPackExplorer.Core.Readers
{
    /// <summary>
    /// Provides methods for reading, extracting, and querying content from a Resonite Package (using SharpCompress).
    /// </summary>
    public class PackageReader : IDisposable
    {
        private readonly IArchive _archive;
        private bool _disposed = false;
        
        /// <summary>
        /// Creates a new instance of <see cref="PackageReader"/>.
        /// </summary>
        /// <param name="packagePath">The path to the package file.</param>
        public PackageReader(string packagePath)
        {
            _archive = ArchiveFactory.OpenArchive(packagePath);
        }

        /// <summary>
        /// Gets a readable file stream for entry into the archive.
        /// </summary>
        /// <param name="entryKey">The relative path of the file to locate.</param>
        /// <returns>A <see cref="Stream"/> containing the entry data.</returns>
        public Stream? GetEntryStream(string entryKey)
        {
            var entry = _archive.Entries
            .FirstOrDefault(e => !e.IsDirectory && e.Key.Equals(entryKey, StringComparison.OrdinalIgnoreCase));

            return entry?.OpenEntryStream();
        }

        /// <summary>
        /// Checks if a specific file entry exists in the archive.
        /// </summary>
        /// <param name="entryKey">The relative file path to check.</param>
        /// <returns><see langword="true"/> if the file exists.</returns>
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