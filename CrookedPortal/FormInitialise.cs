using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Windows.Forms;

namespace CrookedPortal
{
    public partial class FormInitialise : Form
    {
        private string _MCTAPIURL;
        private NetworkCredential _MCTCredentials;
        
        private string _GCAPIURL;
        private NetworkCredential _GCCredentials;

        private string _ShopifyProductsURL;

        private EtsyTokenData _EtsyTokenData;
        private EtsyListingsJSONRoot _EtsyListingsRoot;

        public FormInitialise()
        {
            InitializeComponent();
            _MCTAPIURL = Properties.Resources.MCTAPIURL;
            _MCTCredentials = new NetworkCredential(Properties.Resources.MCTAPIKey, Properties.Resources.MCTPassword);
            
            _GCAPIURL = Properties.Resources.GCAPIURL;
            _GCCredentials = new NetworkCredential(Properties.Resources.GCAPIKey, Properties.Resources.GCPassword);

            _ShopifyProductsURL = Properties.Resources.ShopifyProductsURL + Properties.Resources.ShopifyProductsURLFields;
        }

        private void FormInitialise_Load(object sender, EventArgs e)
        {
            labelGCBold.Visible = false;
            labelGC.Visible = false;
            labelEtsyBold.Visible = false;
            labelEtsy.Visible = false;
        }

        public void InitialiseCollections(ref List<ShopifyProduct> mctProducts, ref List<ShopifyProduct> gcProducts, ref List<EtsyListing> etsyListings)
        {
            labelMCTBold.Refresh();
            labelMCT.Text = "Loading...";
            labelMCT.Refresh();
            mctProducts = JsonSerializer.Deserialize<ShopifyProductsJSONRoot>(Utilities.GetJSONFromShopify(_MCTAPIURL + _ShopifyProductsURL, _MCTCredentials)).Products;
            labelMCT.Text = "Done!";
            labelMCT.Refresh();

            labelGCBold.Visible = true;
            labelGCBold.Refresh();
            labelGC.Text = "Loading...";
            labelGC.Visible = true;
            labelGC.Refresh();
            gcProducts = JsonSerializer.Deserialize<ShopifyProductsJSONRoot>(Utilities.GetJSONFromShopify(_GCAPIURL + _ShopifyProductsURL, _GCCredentials)).Products;
            labelGC.Text = "Done!";
            labelGC.Refresh();

            labelEtsyBold.Visible = true;
            labelEtsyBold.Refresh();
            labelEtsy.Text = "Loading...";
            labelEtsy.Visible = true;
            labelEtsy.Refresh();
            using (Models.CrookedPortalContext dataContext = new Models.CrookedPortalContext())
            {
                Models.EtsyConnectionDatum connectionData = dataContext.EtsyConnectionData.First();
                _EtsyTokenData = JsonSerializer.Deserialize<EtsyTokenData>(Utilities.GetTokenDataFromEtsy(Properties.Resources.EtsyTokenDataURL + connectionData.EtsyApirefreshToken));
                connectionData.EtsyApiaccessToken = _EtsyTokenData.AccessToken;
                connectionData.EtsyApirefreshToken = _EtsyTokenData.RefreshToken;
                dataContext.SaveChanges();
            }
            foreach (string state in Enum.GetNames(typeof(EtsyListingState)))
            {
                List<EtsyListing> stateListings = new List<EtsyListing>();

                _EtsyListingsRoot = JsonSerializer.Deserialize<EtsyListingsJSONRoot>(Utilities.GetJSONFromEtsy(Properties.Resources.EtsyListingsURL + "&state=" + state.ToLower()));
                stateListings = _EtsyListingsRoot.Listings;
                etsyListings.AddRange(_EtsyListingsRoot.Listings);

                int paginationIndex = 1;

                while (stateListings.Count < _EtsyListingsRoot.Count)
                {
                    stateListings.AddRange(JsonSerializer.Deserialize<EtsyListingsJSONRoot>(Utilities.GetJSONFromEtsy(Properties.Resources.EtsyListingsURL + "&state=" + state.ToLower() + "&offset=" + paginationIndex * 100)).Listings);
                    etsyListings.AddRange(JsonSerializer.Deserialize<EtsyListingsJSONRoot>(Utilities.GetJSONFromEtsy(Properties.Resources.EtsyListingsURL + "&state=" + state.ToLower() + "&offset=" + paginationIndex * 100)).Listings);
                    paginationIndex++;
                }
            }
            labelEtsy.Text = "Done!";
            labelEtsy.Refresh();
        }

        public void InitialiseCollections(ref List<ShopifyProduct> gcProducts, ref List<EtsyListing> etsyListings)
        {
            labelMCTBold.Refresh();
            labelMCT.Text = "Loading...";
            labelMCT.Refresh();
            labelMCT.Text = "Done!";
            labelMCT.Refresh();

            labelGCBold.Visible = true;
            labelGCBold.Refresh();
            labelGC.Text = "Loading...";
            labelGC.Visible = true;
            labelGC.Refresh();
            gcProducts = JsonSerializer.Deserialize<ShopifyProductsJSONRoot>(Utilities.GetJSONFromShopify(_GCAPIURL + _ShopifyProductsURL, _GCCredentials)).Products;
            labelGC.Text = "Done!";
            labelGC.Refresh();

            labelEtsyBold.Visible = true;
            labelEtsyBold.Refresh();
            labelEtsy.Text = "Loading...";
            labelEtsy.Visible = true;
            labelEtsy.Refresh();
            _EtsyTokenData = JsonSerializer.Deserialize<EtsyTokenData>(Utilities.GetTokenDataFromEtsy(Properties.Resources.EtsyTokenDataURL));
            Properties.Settings.Default.EtsyAPIAccessToken = _EtsyTokenData.AccessToken;
            Properties.Settings.Default.EtsyAPIRefreshToken = _EtsyTokenData.RefreshToken;
            Properties.Settings.Default.Save();
            foreach (string state in Enum.GetNames(typeof(EtsyListingState)))
            {
                List<EtsyListing> stateListings = new List<EtsyListing>();

                _EtsyListingsRoot = JsonSerializer.Deserialize<EtsyListingsJSONRoot>(Utilities.GetJSONFromEtsy(Properties.Resources.EtsyListingsURL + "&state=" + state.ToLower()));
                stateListings = _EtsyListingsRoot.Listings;
                etsyListings.AddRange(_EtsyListingsRoot.Listings);

                int paginationIndex = 1;

                while (stateListings.Count < _EtsyListingsRoot.Count)
                {
                    stateListings.AddRange(JsonSerializer.Deserialize<EtsyListingsJSONRoot>(Utilities.GetJSONFromEtsy(Properties.Resources.EtsyListingsURL + "&state=" + state.ToLower() + "&offset=" + paginationIndex * 100)).Listings);
                    etsyListings.AddRange(JsonSerializer.Deserialize<EtsyListingsJSONRoot>(Utilities.GetJSONFromEtsy(Properties.Resources.EtsyListingsURL + "&state=" + state.ToLower() + "&offset=" + paginationIndex * 100)).Listings);
                    paginationIndex++;
                }
            }
            labelEtsy.Text = "Done!";
            labelEtsy.Refresh();
        }
    }
}
