using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace CrookedPortal
{
    class Utilities
    {
        public static string GetTokenDataFromEtsy(string url)
        {
            using (Models.CrookedPortalContext dataContext = new Models.CrookedPortalContext())
            {
                string data = null;
                Models.EtsyConnectionDatum etsyConnectionData = dataContext.EtsyConnectionData.First();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(etsyConnectionData.EtsyTokenDataUrl);

                request.ContentType = "application/json";
                request.Method = "POST";

                string json = "{ \"grant_type\": \"refresh_token\"," +
                                "\"client_id\": \"" + etsyConnectionData.EtsyApikey + "\"," +
                                "\"refresh_token\": \"" + etsyConnectionData.EtsyApirefreshToken + "\"" +
                              "}";

                using (var writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(json);
                }

                WebResponse response = (HttpWebResponse)request.GetResponse();

                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    data = reader.ReadToEnd();
                }

                return data;
            }
        }

        public static string GetJSONFromEtsy(string url)
        {
            using (Models.CrookedPortalContext dataContext = new Models.CrookedPortalContext())
            {
                StringBuilder json = new StringBuilder();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                Models.EtsyConnectionDatum connectionData = dataContext.EtsyConnectionData.First();

                request.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                request.Headers.Add("x-api-key", Properties.Resources.EtsyAPIKey);
                request.Headers.Add("Authorization", "Bearer " + connectionData.EtsyApiaccessToken);

                WebResponse response = request.GetResponse();
                Stream responseStream = null;
                StreamReader reader;

                try
                {
                    responseStream = response.GetResponseStream();
                    reader = new StreamReader(responseStream);

                    json.Append(reader.ReadToEnd());
                }
                finally
                {
                    if (responseStream != null)
                    {
                        responseStream.Close();
                    }
                }

                return json.ToString();
            }
        }

        public static string GetJSONFromShopify(string url, NetworkCredential creds)
        {
            StringBuilder json = new StringBuilder();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.Credentials = creds;

            WebResponse response = request.GetResponse();
            Stream responseStream = null;
            StreamReader reader;

            try
            {
                responseStream = response.GetResponseStream();
                reader = new StreamReader(responseStream);

                json.Append(reader.ReadToEnd());

                while (response.Headers["Link"] != null && response.Headers["Link"].Contains("next"))
                {
                    string linkHeader = response.Headers["Link"];
                    string next = linkHeader.Substring(linkHeader.LastIndexOf("<") + 1, linkHeader.LastIndexOf(">") - linkHeader.LastIndexOf("<") - 1);

                    request = (HttpWebRequest)WebRequest.Create(next);
                    request.Credentials = creds;
                    response = request.GetResponse();
                    responseStream = response.GetResponseStream();
                    reader = new StreamReader(responseStream);

                    json.Remove(json.Length - 2, 2);
                    json.Append("," + reader.ReadToEnd().Remove(0, 13));
                }
            }
            finally
            {
                if (responseStream != null)
                {
                    responseStream.Close();
                }
            }

            return json.ToString();
        }

        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '?' || c == ',' || c == '(' || c == ')' || c == ' ' || c == '\'' || c == '-' || c == '/' || c == ':' || c == '!' || c == '&' || c == '\"' || c == '£')
                {
                    sb.Append(c);
                    if (sb[^1] == ':' && sb[^1] != '\n')
                    {
                        sb.Append(Environment.NewLine);
                    }
                }
                else
                {
                    if (sb.Length > 0)
                    {
                        if (sb[^1] != '\n' && sb[^1] != '\r')
                        {
                            sb.Append(Environment.NewLine);
                        }
                    }
                }
            }
            return sb.ToString();
        }

        public static string CleanString(string description)
        {
            string cleanString = WebUtility.HtmlEncode(Regex.Replace(description, "<[^>]*>", string.Empty));

            cleanString = cleanString.Replace("&amp;", "&");
            //cleanString = cleanString.Replace("&amp;amp;", "&");
            cleanString = Regex.Replace(cleanString, "&#39;", "\'");
            cleanString = Regex.Replace(cleanString, "&#160;", " ");
            cleanString = Regex.Replace(cleanString, "&#163;", "£");
            cleanString = Regex.Replace(cleanString, "&#233;", "e");
            cleanString = cleanString.Replace("%", " percent");
            
            cleanString = cleanString.Replace("&quot;", "\"");

            return cleanString;
        }

        public static List<ShopifyProduct> GetProductMatches(string searchTerms, ShopifyBrand brand, List<ShopifyProduct> products)
        {
            List<ShopifyProduct> matches = new List<ShopifyProduct>();
            Dictionary<ShopifyProduct, int> stringMatchValues = new Dictionary<ShopifyProduct, int>();

            foreach (ShopifyProduct product in products)
            {
                stringMatchValues.Add(product, Utilities.GetStringMatches(searchTerms, product.Title));
            }

            using (Models.CrookedPortalContext dataContext = new Models.CrookedPortalContext())
            {
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
            
            return matches.Take(10).ToList();
        }

        public static int GetStringMatches(string a, string b)
        {
            int matches = 0;
            string[] aWords = a.Split(" ");
            string[] bWords = b.Split(" ");
            List<int> matchedWordIndexes;

            for (int i = 0; i < aWords.Length; i++)
            {
                matchedWordIndexes = new List<int>();
                for (int j = 0; j < bWords.Length; j++)
                {
                    if (aWords[i].ToLower().Trim() == bWords[j].ToLower().Trim() & !matchedWordIndexes.Contains(i))
                    {
                        matches++;
                        matchedWordIndexes.Add(i);
                    }
                }
            }

            return matches;
        }
    }

    class EtsyTokenData
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }
    }
}