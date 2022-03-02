using System.IO;
using System.Net;
using System.Text.Json.Serialization;

namespace CrookedPortal
{
    public class ShopifyProductVariant
    {
        [JsonPropertyName("id")]
        public long ID { get; set; }

        [JsonPropertyName("inventory_item_id")]
        public long InventoryItemID { get; set; }

        [JsonPropertyName("inventory_policy")]
        public string InventoryPolicy { get; set; }

        [JsonPropertyName("inventory_quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("price")]
        public string Price { get; set; }

        [JsonPropertyName("compare_at_price")]
        public string CompareAtPrice { get; set; }

        [JsonPropertyName("option1")]
        public string Size { get; set; }

        [JsonPropertyName("option2")]
        public string Colour { get; set; }

        public ShopifyProductVariant()
        {

        }

        public ShopifyProductVariant(string size, string colour, string price, int quantity)
        {
            this.Size = size;
            this.Colour = colour;
            this.Price = price;
            this.Quantity = quantity;
        }

        public void UpdateInventory(ShopifyBrand brand, ShopifyProduct product, string size, string colour, string price, string compareAtPrice, int quantity, string policy)
        {
            this.Size = size;
            this.Colour = colour;
            this.Price = price;
            this.CompareAtPrice = compareAtPrice;
            this.Quantity = quantity;
            this.InventoryPolicy = policy;

            string url = brand == ShopifyBrand.MCT ? Properties.Resources.MCTAPIURL : Properties.Resources.GCAPIURL;
            string username = brand == ShopifyBrand.MCT ? Properties.Resources.MCTAPIKey : Properties.Resources.GCAPIKey;
            string password = brand == ShopifyBrand.MCT ? Properties.Resources.MCTPassword : Properties.Resources.GCPassword;
            string locationID = brand == ShopifyBrand.MCT ? Properties.Resources.MCTLocationID : Properties.Resources.GCLocationID;

            HttpWebRequest request;
            WebResponse response;

            if (product.Variants[0].Size == "Default Title")
            {
                request = (HttpWebRequest)WebRequest.Create(url + "products/" + product.ID + ".json");
                request.Credentials = new NetworkCredential(username, password);
                request.ContentType = "application/json; charset=utf-8";
                request.Method = "PUT";

                using (var writer = new StreamWriter(request.GetRequestStream()))
                {
                    string json = "{ \"product\": { \"id\": " + product.ID + ", \"options\": [{ \"name\": \"Size\", \"position\": 1 }] } }";

                    writer.Write(json);
                }

                response = (HttpWebResponse)request.GetResponse();
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var result = reader.ReadToEnd();
                }
            }

            request = (HttpWebRequest)WebRequest.Create(url + "variants/" + this.ID + ".json");
            request.Credentials = new NetworkCredential(username, password);
            request.ContentType = "application/json; charset=utf-8";
            request.Method = "PUT";

            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                string json = "{ \"variant\": { \"id\": " + this.ID + ", " +
                              "\"option1\": \"" + size + "\", " +
                              "\"option2\": \"" + colour + "\", " +
                              "\"price\": \"" + price + "\", " +
                              "\"compare_at_price\": \"" + compareAtPrice + "\", " +
                              "\"inventory_policy\": \"" + policy + "\" } }";

                writer.Write(json);
            }

            response = (HttpWebResponse)request.GetResponse();

            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var result = reader.ReadToEnd();
            }

            request = (HttpWebRequest)WebRequest.Create(url + "inventory_levels/set.json");
            request.Credentials = new NetworkCredential(username, password);
            request.ContentType = "application/json; charset=utf-8";
            request.Method = "POST";

            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                string json = "{ \"location_id\": " + locationID + ", " +
                              "\"inventory_item_id\": " + this.InventoryItemID + ", " +
                              "\"available\": " + this.Quantity + " }";

                writer.Write(json);
            }

            response = (HttpWebResponse)request.GetResponse();
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var result = reader.ReadToEnd();
            }
        }
    }

    public class ShopifyProductVariantRoot
    {
        [JsonPropertyName("variant")]
        public ShopifyProductVariant Variant { get; set; }
    }
}
