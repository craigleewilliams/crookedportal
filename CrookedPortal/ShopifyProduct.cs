using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using RestSharp;

namespace CrookedPortal
{
    public class ShopifyOrderRoot
    {
        [JsonPropertyName("orders")]
        public List<ShopifyOrder> Orders { get; set; }
    }

    public class ShopifyOrder
    {
        [JsonPropertyName("line_items")]
        public List<ShopifyOrderItem> OrderItems { get; set; }
    }

    public class ShopifyOrderItem
    {
        [JsonPropertyName("product_id")]
        public long ProductID { get; set; }

        [JsonPropertyName("variant_id")]
        public long VariantID { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
    }

    public class ShopifyProduct
    {
        [JsonPropertyName("id")]
        public long ID { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("body_html")]
        public string Description { get; set; }

        [JsonPropertyName("images")]
        public List<ShopifyProductImage> Images { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("tags")]
        public string Tags { get; set; }

        [JsonPropertyName("variants")]
        public List<ShopifyProductVariant> Variants { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("published_at")]
        public DateTime? PublishedAt { get; set; }

        [JsonPropertyName("published_scope")]
        public string? PublishedScope { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        public void UpdateFromCorrespondingProduct(ShopifyBrand brand, ShopifyProduct product)
        {
            string url = brand == ShopifyBrand.MCT ? Properties.Resources.MCTAPIURL : Properties.Resources.GCAPIURL;
            string username = brand == ShopifyBrand.MCT ? Properties.Resources.MCTAPIKey : Properties.Resources.GCAPIKey;
            string password = brand == ShopifyBrand.MCT ? Properties.Resources.MCTPassword : Properties.Resources.GCPassword;
            string locationID = brand == ShopifyBrand.MCT ? Properties.Resources.MCTLocationID : Properties.Resources.GCLocationID;

            HttpWebRequest request;
            WebResponse response;
            ShopifyProductVariant variant;

            if (this.Variants.Count < product.Variants.Count)
            {
                for (int i = 0; i < product.Variants.Count; i++)
                {
                    if (i >= this.Variants.Count)
                    {
                        request = (HttpWebRequest)WebRequest.Create(url + "products/" + this.ID + "/variants.json");
                        request.ContentType = "application/json; charset=utf-8";
                        request.Credentials = new NetworkCredential(username, password);
                        request.Method = "POST";

                        using (var writer = new StreamWriter(request.GetRequestStream()))
                        {
                            string json = "{ \"variant\": {" +
                                          "\"option1\": \"" + product.Variants[i].Size + "\", " +
                                          "\"option2\": \"" + product.Variants[i].Colour + "\", " +
                                          "\"price\": \"" + product.Variants[i].Price + "\", " +
                                          "\"compare_at_price\": \"" + product.Variants[i].CompareAtPrice + "\", " +
                                          "\"inventory_policy\": \"" + product.Variants[i].InventoryPolicy + "\" } }";

                            writer.Write(json);
                        }

                        response = (HttpWebResponse)request.GetResponse();
                        using (var reader = new StreamReader(response.GetResponseStream()))
                        {
                            variant = JsonSerializer.Deserialize<ShopifyProductVariantRoot>(reader.ReadToEnd()).Variant;
                        }

                        request = (HttpWebRequest)WebRequest.Create(url + "inventory_levels/set.json");
                        request.Credentials = new NetworkCredential(username, password);
                        request.ContentType = "application/json; charset=utf-8";
                        request.Method = "POST";

                        using (var writer = new StreamWriter(request.GetRequestStream()))
                        {
                            string json = "{ \"location_id\": " + locationID + ", " +
                                          "\"inventory_item_id\": " + variant.InventoryItemID + ", " +
                                          "\"available\": " + product.Variants[i].Quantity + " }";

                            writer.Write(json);
                        }

                        response = (HttpWebResponse)request.GetResponse();
                        using (var reader = new StreamReader(response.GetResponseStream()))
                        {
                            var result = reader.ReadToEnd();
                        }

                        this.Variants.Add(new ShopifyProductVariant()
                        {
                            ID = variant.ID,
                            Size = variant.Size,
                            Colour = variant.Colour,
                            Price = variant.Price,
                            CompareAtPrice = variant.CompareAtPrice,
                            Quantity = variant.Quantity,
                            InventoryPolicy = variant.InventoryPolicy,
                            InventoryItemID = variant.InventoryItemID
                        });
                    }

                    this.Variants[i].UpdateInventory(brand, product, product.Variants[i].Size, product.Variants[i].Colour, product.Variants[i].Price,
                                                     product.Variants[i].CompareAtPrice, product.Variants[i].Quantity, product.Variants[i].InventoryPolicy);
                }
            }
            else if (this.Variants.Count == product.Variants.Count)
            {
                for (int i = 0; i < this.Variants.Count; i++)
                {
                    this.Variants[i].UpdateInventory(brand, product, product.Variants[i].Size, product.Variants[i].Colour, product.Variants[i].Price, product.Variants[i].CompareAtPrice, product.Variants[i].Quantity, product.Variants[i].InventoryPolicy);
                }
            }
            else
            {
                for (int i = 0; i < this.Variants.Count; i++)
                {
                    if (i < product.Variants.Count)
                    {
                        this.Variants[i].UpdateInventory(brand, product, product.Variants[i].Size, product.Variants[i].Colour, product.Variants[i].Price,
                                                         product.Variants[i].CompareAtPrice, product.Variants[i].Quantity, product.Variants[i].InventoryPolicy);
                    }
                    else
                    {
                        request = (HttpWebRequest)WebRequest.Create(url + "products/" + this.ID + "/variants/" + this.Variants[i].ID + ".json");
                        request.Credentials = new NetworkCredential(username, password);
                        request.Method = "DEL";

                        _ = request.GetResponse();

                        this.Variants.RemoveAt(i);
                    }
                }
            }
        }

        public void UpdateTitle(ShopifyBrand brand, string title)
        {
            string url = brand == ShopifyBrand.MCT ? Properties.Resources.MCTAPIURL : Properties.Resources.GCAPIURL;
            string username = brand == ShopifyBrand.MCT ? Properties.Resources.MCTAPIKey : Properties.Resources.GCAPIKey;
            string password = brand == ShopifyBrand.MCT ? Properties.Resources.MCTPassword : Properties.Resources.GCPassword;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + "products/" + this.ID + ".json");
            WebResponse response;

            request.Credentials = new NetworkCredential(username, password);
            request.ContentType = "application/json; charset=utf-8";
            request.Method = "PUT";

            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                string json = "{ \"product\": { \"id\": " + this.ID + ", \"title\": \"" + title + "\" } }";

                writer.Write(json);
            }

            response = (HttpWebResponse)request.GetResponse();
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var result = reader.ReadToEnd();
            }
        }

        public void UpdateDescription(ShopifyBrand brand, string description)
        {
            string url = brand == ShopifyBrand.MCT ? Properties.Resources.MCTAPIURL : Properties.Resources.GCAPIURL;
            string username = brand == ShopifyBrand.MCT ? Properties.Resources.MCTAPIKey : Properties.Resources.GCAPIKey;
            string password = brand == ShopifyBrand.MCT ? Properties.Resources.MCTPassword : Properties.Resources.GCPassword;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + "products/" + this.ID + ".json");
            WebResponse response;

            request.Credentials = new NetworkCredential(username, password);
            request.ContentType = "application/json; charset=utf-8";
            request.Method = "PUT";

            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                string json = "{ \"product\": { \"id\": " + this.ID + ", \"body_html\": \"" + description.Replace("\"", "\\\"").Replace("\n", "").Replace("\r", "") + "\" } }";

                writer.Write(json);
            }

            response = (HttpWebResponse)request.GetResponse();
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var result = reader.ReadToEnd();
            }
        }

        public void UpdateStatus(ShopifyBrand brand, string status)
        {
            string url = brand == ShopifyBrand.MCT ? Properties.Resources.MCTAPIURL : Properties.Resources.GCAPIURL;
            string username = brand == ShopifyBrand.MCT ? Properties.Resources.MCTAPIKey : Properties.Resources.GCAPIKey;
            string password = brand == ShopifyBrand.MCT ? Properties.Resources.MCTPassword : Properties.Resources.GCPassword;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + "products/" + this.ID + ".json");
            WebResponse response;

            request.Credentials = new NetworkCredential(username, password);
            request.ContentType = "application/json; charset=utf-8";
            request.Method = "PUT";

            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                string json = "{ \"product\": { \"id\": " + this.ID + ", \"status\": \"" + status.ToLower() + "\" } }";

                writer.Write(json);
            }

            response = (HttpWebResponse)request.GetResponse();
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var result = reader.ReadToEnd();
            }
        }

        public List<ShopifyProduct> GetProductMatches(ShopifyBrand brand, List<ShopifyProduct> products)
        {
            List<ShopifyProduct> matches = new List<ShopifyProduct>();
            Dictionary<ShopifyProduct, int> stringMatchValues = new Dictionary<ShopifyProduct, int>();

            using (Models.CrookedPortalContext dataContext = new Models.CrookedPortalContext())
            {
                foreach (ShopifyProduct product in products)
                {
                    stringMatchValues.Add(product, Utilities.GetStringMatches(this.Title, product.Title));
                }

                switch (brand)
                {
                    case ShopifyBrand.MCT:
                        {
                            matches.AddRange(stringMatchValues.Where(match => match.Value > 1 & !matches.Contains(match.Key) &&
                                            (!dataContext.CrossPlatformProductListings.Any(cppl => cppl.GcproductId == match.Key.ID ||
                                             dataContext.CrossPlatformProductListings.Any(cppl => cppl.GcproductId == match.Key.ID && cppl.EtsyListingId != null))))
                                   .OrderByDescending(match => match.Value).Select(match => match.Key).ToList());
                        }
                        break;
                    case ShopifyBrand.GC:
                        {
                            matches.AddRange(stringMatchValues.Where(match => match.Value > 1 & !matches.Contains(match.Key) &&
                                            (!dataContext.CrossPlatformProductListings.Any(cppl => cppl.MctproductId == match.Key.ID ||
                                             dataContext.CrossPlatformProductListings.Any(cppl => cppl.MctproductId == match.Key.ID && cppl.EtsyListingId != null))))
                                   .OrderByDescending(match => match.Value).Select(match => match.Key).ToList());
                        }
                        break;
                }
            }

            return matches;
        }

        public List<EtsyListing> GetListingMatches(ShopifyBrand brand, List<EtsyListing> listings)
        {
            List<EtsyListing> matches = new List<EtsyListing>();
            Dictionary<EtsyListing, int> stringMatchValues = new Dictionary<EtsyListing, int>();

            using (Models.CrookedPortalContext dataContext = new Models.CrookedPortalContext())
            {
                foreach (EtsyListing listing in listings)
                {
                    stringMatchValues.Add(listing, Utilities.GetStringMatches(this.Title, listing.Title));
                }

                switch (brand)
                {
                    case ShopifyBrand.MCT:
                        {
                            matches.AddRange(stringMatchValues.Where(match => match.Value > 1 & !matches.Contains(match.Key) &&
                            (!dataContext.CrossPlatformProductListings.Any(cppl => cppl.EtsyListingId == match.Key.ID ||
                            dataContext.CrossPlatformProductListings.Any(cppl => cppl.EtsyListingId == match.Key.ID && cppl.MctproductId != null))))
                                .OrderByDescending(match => match.Value).Select(match => match.Key).ToList());
                        }
                        break;
                    case ShopifyBrand.GC:
                        {
                            matches.AddRange(stringMatchValues.Where(match => match.Value > 1 & !matches.Contains(match.Key) &&
                                            (!dataContext.CrossPlatformProductListings.Any(cppl => cppl.EtsyListingId == match.Key.ID ||
                                             dataContext.CrossPlatformProductListings.Any(cppl => cppl.EtsyListingId == match.Key.ID && cppl.MctproductId != null))))
                                   .OrderByDescending(match => match.Value).Select(match => match.Key).ToList());
                        }
                        break;
                }
            }

            return matches;
        }

        public EtsyListing CopyToEtsy(string description)
        {
            RestClient restClient = new RestClient();
            RestRequest restRequest = null;
            IRestResponse restResponse = null;

            string tags = null;

            EtsyListing listing = null;

            tags = String.Join(",", this.Tags.Split(",").Where(tag => !tag.Contains("/") && tag.Length <= 20).Take(13).ToArray());

            restRequest = new RestRequest("https://openapi.etsy.com/v3/application/shops/" + Properties.Resources.EtsyShopID + "/listings", Method.POST);

            restRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            restRequest.AddHeader("x-api-key", Properties.Resources.EtsyAPIKey);
            restRequest.AddHeader("Authorization", "Bearer " + Properties.Settings.Default.EtsyAPIAccessToken);

            restRequest.AddParameter("title", Utilities.CleanString(this.Title));
            restRequest.AddParameter("description", description);
            restRequest.AddParameter("quantity", 1);
            restRequest.AddParameter("price", this.Variants[0].Price);
            restRequest.AddParameter("tags", tags);
            restRequest.AddParameter("taxonomy_id", 500);
            restRequest.AddParameter("who_made", "i_did");
            restRequest.AddParameter("is_supply", false);
            restRequest.AddParameter("when_made", "2020_2022");
            restRequest.AddParameter("shipping_profile_id", 18702005694);

            restResponse = restClient.Execute(restRequest);

            listing = JsonSerializer.Deserialize<EtsyListing>(restResponse.Content);

            foreach (ShopifyProductImage image in this.Images)
            {
                restRequest = new RestRequest("https://openapi.etsy.com/v3/application/shops/" + Properties.Resources.EtsyShopID + "/listings/" + listing.ID + "/images", Method.POST);

                restRequest.AddHeader("Content-Type", "multipart/form-data");
                restRequest.AddHeader("x-api-key", Properties.Resources.EtsyAPIKey);
                restRequest.AddHeader("Authorization", "Bearer " + Properties.Settings.Default.EtsyAPIAccessToken);

                using (WebClient client = new WebClient())
                {
                    restRequest.AddFile("image", client.DownloadData(image.URL), image.Position.ToString() + ".png");
                }
                restRequest.AddParameter("rank", image.Position);

                restResponse = restClient.Execute(restRequest);
            }

            listing.UpdateProducts(ref listing, this.Variants, true);

            return listing;
        }
    }

    class ShopifyProductsJSONRoot
    {
        [JsonPropertyName("products")]
        public List<ShopifyProduct> Products { get; set; }
    }

    class ShopifyProductJSONRoot
    {
        [JsonPropertyName("product")]
        public ShopifyProduct Product { get; set; }
    }

    public enum ShopifyBrand
    {
        MCT,
        GC
    }
}