using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using RestSharp;

namespace CrookedPortal
{
    public class EtsyListing
    {
        [JsonPropertyName("listing_id")]
        public long ID { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        public void UpdateTitle(string title)
        {
            RestClient restClient = new RestClient();
            RestRequest request;

            request = new RestRequest("https://openapi.etsy.com/v3/application/shops/" + Properties.Resources.EtsyShopID + "/listings/" + this.ID.ToString(), Method.PUT);

            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("x-api-key", Properties.Resources.EtsyAPIKey);
            request.AddHeader("Authorization", "Bearer " + Properties.Settings.Default.EtsyAPIAccessToken);

            request.AddParameter("title", title);

            restClient.Execute(request);
        }

        public void UpdateDescription(string description)
        {
            RestClient restClient = new RestClient();
            RestRequest request;

            request = new RestRequest("https://openapi.etsy.com/v3/application/shops/" + Properties.Resources.EtsyShopID + "/listings/" + this.ID.ToString(), Method.PUT);

            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("x-api-key", Properties.Resources.EtsyAPIKey);
            request.AddHeader("Authorization", "Bearer " + Properties.Settings.Default.EtsyAPIAccessToken);

            request.AddParameter("description", description);

            restClient.Execute(request);
        }

        public void UpdateState(string state)
        {
            using (Models.CrookedPortalContext dataContext = new Models.CrookedPortalContext())
            {
                Models.EtsyConnectionDatum connectionData = dataContext.EtsyConnectionData.First();
                RestClient restClient = new RestClient();
                RestRequest request;

                request = new RestRequest("https://openapi.etsy.com/v3/application/shops/" + Properties.Resources.EtsyShopID + "/listings/" + this.ID.ToString(), Method.PUT);

                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddHeader("x-api-key", Properties.Resources.EtsyAPIKey);
                request.AddHeader("Authorization", "Bearer " + connectionData.EtsyApiaccessToken);

                request.AddParameter("state", state.ToLower());

                restClient.Execute(request);
            }
        }

        public void UpdateProducts(ref EtsyListing listing, List<ShopifyProductVariant> variants, bool overwritePrice)
        {
            int inactiveQuantity = 0;

            if (variants.TrueForAll(variant => variant.Quantity <= 0 && variant.InventoryPolicy == "deny"))
            {
                this.UpdateState("inactive");
                listing.State = "inactive";
                inactiveQuantity = 1;
            }

            using (Models.CrookedPortalContext dataContext = new Models.CrookedPortalContext())
            {
                StringBuilder jsonProducts = new StringBuilder("{\"products\": [");
                string propertyValues = string.Empty;
                string propertyIds = string.Empty;

                if (variants.Count > 1)
                {
                    for (int i = 0; i < variants.Count; i++)
                    {
                        if (!String.IsNullOrEmpty(variants[i].Size) && String.IsNullOrEmpty(variants[i].Colour))
                        {
                            propertyValues = "{\"property_id\": 513,\"property_name\": \"Size\",\"value_ids\": [],\"values\": [\"" + variants[i].Size + "\"]}";
                            propertyIds = "513";
                        }
                        if (!String.IsNullOrEmpty(variants[i].Size) & !String.IsNullOrEmpty(variants[i].Colour))
                        {
                            propertyValues = "{\"property_id\": 513,\"property_name\": \"Size\",\"value_ids\": [],\"values\": [\"" + variants[i].Size + "\"]},{\"property_id\": 514,\"property_name\": \"Colour\",\"value_ids\": [],\"values\": [\"" + variants[i].Colour + "\"]}";
                            propertyIds = "513,514";
                        }

                        decimal price = overwritePrice ? decimal.Parse(variants[i].Price) : JsonSerializer.Deserialize<EtsyListingProductsJSONRoot>(Utilities.GetJSONFromEtsy(Properties.Resources.EtsyListingProductsURL + this.ID + "/inventory")).Products[i].Offerings[0].Price.Value;
                        int quantity = variants[i].Quantity > 0 ? variants[i].Quantity : (variants[i].InventoryPolicy == "continue" ? 1 : 0);

                        if (inactiveQuantity > 0)
                        {
                            quantity = inactiveQuantity;
                        }

                        jsonProducts.Append("{\"property_values\": [" + propertyValues + "],\"offerings\": [{\"price\": " + price + ",\"quantity\": " + quantity + ",\"is_enabled\": true}]}");
                        if (i < variants.Count - 1)
                        {
                            jsonProducts.Append(",");
                        }
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(variants[0].Size) && String.IsNullOrEmpty(variants[0].Colour))
                    {
                        propertyValues = "{\"property_id\": 513,\"property_name\": \"Size\",\"value_ids\": [],\"values\": [\"" + variants[0].Size + "\"]}";
                        propertyIds = "513";
                    }
                    if (!String.IsNullOrEmpty(variants[0].Size) & !String.IsNullOrEmpty(variants[0].Colour))
                    {
                        propertyValues = "{\"property_id\": 513,\"property_name\": \"Size\",\"value_ids\": [],\"values\": [\"" + variants[0].Size + "\"]},{\"property_id\": 514,\"property_name\": \"Colour\",\"value_ids\": [],\"values\": [\"" + variants[0].Colour + "\"]}";
                        propertyIds = "513,514";
                    }

                    decimal price = overwritePrice ? decimal.Parse(variants[0].Price) : JsonSerializer.Deserialize<EtsyListingProductsJSONRoot>(Utilities.GetJSONFromEtsy(Properties.Resources.EtsyListingProductsURL + this.ID + "/inventory")).Products[0].Offerings[0].Price.Value;
                    int quantity = variants[0].Quantity > 0 ? variants[0].Quantity : 1;

                    if (inactiveQuantity > 0)
                    {
                        quantity = inactiveQuantity;
                    }

                    jsonProducts.Append("{\"property_values\": [" + propertyValues + "],\"offerings\": [{\"price\": " + price + ",\"quantity\": " + quantity + ",\"is_enabled\": true}]}");
                }
                jsonProducts.Append("],\"price_on_property\": [" + propertyIds + "],\"quantity_on_property\": [" + propertyIds + "]}");

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Properties.Resources.EtsyListingProductsURL + this.ID + "/inventory");
                Models.EtsyConnectionDatum connectionData = dataContext.EtsyConnectionData.First();

                request.Headers.Add("Content-Type", "application/json");
                request.Headers.Add("x-api-key", Properties.Resources.EtsyAPIKey);
                request.Headers.Add("Authorization", "Bearer " + connectionData.EtsyApiaccessToken);
                request.Method = "PUT";

                using (var writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(jsonProducts);
                }

                WebResponse response = (HttpWebResponse)request.GetResponse();

                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var result = reader.ReadToEnd();
                }
            }

            if (variants.TrueForAll(variant => variant.Quantity <= 0 && variant.InventoryPolicy == "deny"))
            {
                this.UpdateState("inactive");
                listing.State = "inactive";
            }
        }
    }

    class EtsyListingsJSONRoot
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("results")]
        public List<EtsyListing> Listings { get; set; }

        [JsonPropertyName("pagination")]
        public EtsyPagination Pagination { get; set; }
    }

    class EtsyPagination
    {
        [JsonPropertyName("effective_page")]
        public int Page { get; set; }

        [JsonPropertyName("next_page")]
        public int? NextPage { get; set; }
    }

    enum EtsyListingState
    {
        Active,
        Inactive,
        Draft,
        Expired,
        Sold_Out
    }
}