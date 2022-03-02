using System.Text.Json.Serialization;

namespace CrookedPortal
{
    public class ShopifyProductImage
    {
        [JsonPropertyName("id")]
        public long ID { get; set; }

        [JsonPropertyName("position")]
        public int Position { get; set; }

        [JsonPropertyName("src")]
        public string URL { get; set; }

        public byte[] Image { get; set; }
    }
}
