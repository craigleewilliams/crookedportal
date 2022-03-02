using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CrookedPortal
{
    class EtsyListingImage
    {
        [JsonPropertyName("listing_image_id")]
        public long ID { get; set; }

        [JsonPropertyName("url_fullxfull")]
        public string URL { get; set; }

        [JsonPropertyName("rank")]
        public int Rank { get; set; }
    }

    class EtsyListingImagesJSONRoot
    {
        [JsonPropertyName("results")]
        public List<EtsyListingImage> Images { get; set; }
    }
}
