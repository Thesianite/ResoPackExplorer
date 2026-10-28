using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ResoPackExplorer.Core.Models
{
    /// <summary>
    /// Represents a .record from a Resonite package.
    /// </summary>
    public class Record
    {
        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("ownerId")]
        public string? OwnerId { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("assetUri")]
        public string? AssetUri { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("recordType")]
        public string? RecordType { get; set; }

        [JsonProperty("tags")]
        public string[] Tags { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Convets a <see cref="JObject"/> into an instance of <see cref="Record"/>.
        /// </summary>
        /// <param name="json">The JSON object containing the record data.</param>
        /// <returns>A new <see cref="Record"/> instance from the JSON object's data.</returns>
        public static Record FromJson(JObject json)
        {
            return new Record
            {
                Id = json["id"]?.ToString(),
                OwnerId = json["ownerId"]?.ToString(),
                Name = json["name"]?.ToString(),
                AssetUri = json["assetUri"]?.ToString(),
                Description = json["description"]?.ToString(),
                RecordType = json["recordType"]?.ToString(),
                Tags = json["tags"]?.ToObject<string[]>() ?? Array.Empty<string>()
            };
        }
    }
}