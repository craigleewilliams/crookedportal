using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CrookedPortal
{
    class EtsyListingProduct
    {
        [JsonPropertyName("product_id")]
        public long ID { get; set; }

        [JsonPropertyName("offerings")]
        public List<EtsyListingProductOffering> Offerings { get; set; }

        [JsonPropertyName("property_values")]
        public List<EtsyListingProductPropertyValues> PropertyValues { get; set; }
    }

    class EtsyListingProductsJSONRoot
    {
        [JsonPropertyName("products")]
        public List<EtsyListingProduct> Products { get; set; }
    }

    class EtsyListingProductOffering
    {
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("price")]
        public EtsyListingProductOfferingPrice Price { get; set; }
    }

    class EtsyListingProductOfferingPrice
    {
        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        [JsonPropertyName("divisor")]
        public int Divisor { get; set; }

        public decimal Value
        {
            get
            {
                return (decimal)this.Amount / this.Divisor;
            }
            set
            {
                this.Value = value;
            }
        }
    }

    class EtsyListingProductPropertyValues
    {
        [JsonPropertyName("property_name")]
        public string PropertyName { get; set; }

        [JsonPropertyName("values")]
        public List<string> Values { get; set; }
    }
}
