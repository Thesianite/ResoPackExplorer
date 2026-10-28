using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;

namespace ResPackExplorer.Core.Readers
{
    public static class RecordParser
    {
        public static JObject ParseRecordStream(Stream recordStream)
        {
            using var reader = new StreamReader(recordStream);
            string jsonContent = reader.ReadToEnd();

            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                return new JObject();
            }

            try
            {
                return JObject.Parse(jsonContent);
            }
            catch
            {
                return new JObject();
            }

        }
    }
}