using ResPackExplorer.Core.Models;
using ResPackExplorer.Core.Readers;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== ResPackExplorer CLI ===");
        
        string packPath = args.Length > 0 ? args[0] : "input.resonitepackage";
        
        // Check for valid path
        if (!File.Exists(packPath))
        {
            Console.WriteLine($"[Error] Package file not found at: {packPath}");
            Console.WriteLine("Please put in file path as arg!");
            return;
        }

        try
        {
            Console.WriteLine($"Opening pack: {packPath}...");
            using var PackageReader = new PackageReader(packPath);
            
            if (!PackageReader.HasEntry("R-Main.record"))
            {
                Console.WriteLine("[Error] R-Main.record not found in package!!!");
                return;
            }

            Console.WriteLine("Extracting and parsing R-Main.Record...");
            using var recordStream = PackageReader.GetEntryStream("R-Main.record");

            if (recordStream != null)
            {
                var JObject = RecordParser.ParseRecordStream(recordStream);
                var record = Record.FromJson(JObject);
                
                //Record Info
                Console.WriteLine($"\n--- Record Info ---");
                Console.WriteLine($"Name: {record.Name ?? "N/A"}");
                Console.WriteLine($"Description: {record.Description ?? "No description"}");
                Console.WriteLine($"RecordType: {record.RecordType ?? "N/A"}");
                Console.WriteLine($"Id: {record.Id ?? "N/A"}");
                Console.WriteLine($"OwnerId: {record.OwnerId ?? "N/A"}");
                Console.WriteLine($"AssetUri: {record.AssetUri ?? "N/A"}");
                Console.WriteLine($"Tags: {(record.Tags.Length > 0 ? string.Join(", ", record.Tags) : "None")}");
                
                // Package Files
                Console.WriteLine("--- Files ---");
                foreach (var (key, size) in PackageReader.GetEntries())
                {
                    Console.WriteLine($" - {key} ({size} bytes)");
                }
                
                Console.WriteLine($"--- End ---\n");
            }
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Exception] failed to read package: {ex.Message}");
        }
    }
}