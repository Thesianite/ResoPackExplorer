using ResoPackExplorer.Core.Models;

namespace ResoPackExplorer.Core.Readers
{
    public class AssetTypeDetector
    {
        public static readonly AssetType Unknown = new() { Name = "Unknown Type", Extension = ""};
        public static readonly AssetType Frdt = new() { Name = "FrDT", Extension=".FrDT"};

        private static readonly Dictionary<AssetType, byte[]> _fileSignatures = new()
        {
            
            {Frdt, new byte[] { 0x46, 0x72, 0x44, 0x54, 0x00, 0x00, 0x00, 0x00, 0x03, 0x02 }}
        };

        private static readonly int _maxSigLength = 10;

        public AssetType ?DetectType(Stream assetStream)
        {

            using var reader = new BinaryReader(assetStream, System.Text.Encoding.UTF8, leaveOpen: true);

            byte[] headerBytes = reader.ReadBytes(_maxSigLength);
            if (headerBytes.Length == 0) return null;

            foreach (var signature in _fileSignatures)
            {
                byte[] pattern = signature.Value;

                if (headerBytes.Length < pattern.Length) continue;

                bool matches = true;
                for (int i = 0; i < pattern.Length; i++)
                {
                    if (headerBytes[i] != pattern[i])
                    {
                        matches = false;
                        break;
                    }

                    if (matches)
                    {
                        return signature.Key;
                    }
                }
            }

            return null;
        }

    }
}