using ResoPackExplorer.Core.Models;

namespace ResoPackExplorer.Core.Readers
{
    public class AssetTypeDetector
    {
        public static readonly AssetType Unknown = new() { Name = "_Unknown Type", Extension = ""};
        public static readonly AssetType Frdt = new() { Name = "FrDT", Extension=".FrDT"};
        public static readonly AssetType MeshX = new() { Name = "MeshX", Extension=".meshx"};
        public static readonly AssetType Png = new() { Name = "Portable Network Graphics", Extension=".png"};
        // Audio
        public static readonly AssetType Ogg = new() { Name = "Ogg", Extension=".ogg"};
        public static readonly AssetType Wav = new() { Name = "Waveform Audio", Extension = ".wav"};
        

        private static readonly Dictionary<AssetType, byte?[]> _fileSignatures = new()
        {
            
            {Frdt, new byte?[] { 0x46, 0x72, 0x44, 0x54, 0x00, 0x00, 0x00, 0x00, 0x03, 0x02 }},
            {MeshX, new byte?[] { 0x05, 0x4D, 0x65, 0x73, 0x68, 0x58, 0x07}},
            {Ogg, new byte?[] { 0x4F, 0x67, 0x67, 0x53 }},
            {Png, new byte?[] { 0x89, 0x50, 0x4e, 0x47 }},
            {Wav, new byte?[] { 0x52, 0x49, 0x46, 0x46, null, null, null, null}}
        };

        private static readonly int _maxSigLength = 12;

        public AssetType ?DetectType(Stream assetStream)
        {
            if (assetStream == null | !assetStream.CanRead) return null;


            using var reader = new BinaryReader(assetStream, System.Text.Encoding.UTF8, leaveOpen: true);

            byte[] headerBytes = reader.ReadBytes(_maxSigLength);
            if (headerBytes.Length == 0) return null;

            foreach (var signature in _fileSignatures)
            {
                byte?[] pattern = signature.Value;

                if (headerBytes.Length < pattern.Length) continue;

                bool matches = true;
                for (int i = 0; i < pattern.Length; i++)
                {
                    if (pattern[i] == null) continue;

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

            return Unknown;
        }

    }
}