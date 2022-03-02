using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace CrookedPortal
{
    public partial class FormMain : Form
    {
        List<ShopifyProduct> _MCTProducts;
        List<ShopifyProduct> _GCProducts;
        List<ShopifyProduct> _ProductsToUpdate;
        List<ShopifyProduct> _NewMCTProducts;
        List<ShopifyProduct> _NewGCProducts;
        ShopifyProduct _SelectedProduct;
        ShopifyProduct _CorrespondingProduct;
        List<long> _MatchedShopifyProductIds;
        bool gcMatched;

        List<EtsyListing> _EtsyListings;
        List<EtsyListing> _NewEtsyListings;
        EtsyListing _CorrespondingListing;
        List<long> _MatchedEtsyListingIds;
        bool etsyMatched;

        int _Index;

        ShopifyBrand brand;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            _MCTProducts = new List<ShopifyProduct>();
            _GCProducts = new List<ShopifyProduct>();
            _NewMCTProducts = new List<ShopifyProduct>();
            _NewGCProducts = new List<ShopifyProduct>();
            _CorrespondingProduct = new ShopifyProduct();
            gcMatched = false;

            _EtsyListings = new List<EtsyListing>();
            _NewEtsyListings = new List<EtsyListing>();
            _CorrespondingListing = new EtsyListing();
            etsyMatched = false;

            _Index = 0;// Properties.Settings.Default.Index;
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            this.Refresh();

            using (FormInitialise form = new FormInitialise())
            {
                form.Show();
                form.InitialiseCollections(ref _MCTProducts, ref _GCProducts, ref _EtsyListings);
                form.Focus();
            }

            UpdateEtsyStockLevels();

            buttonAddData.Enabled = false;
            //using (Models.CrookedPortalContext dataContext = new Models.CrookedPortalContext())
            //{
            //    foreach (ShopifyProduct product in _MCTProducts)
            //    {
            //        if (!dataContext.Mctproducts.Any(mctp => mctp.Id == product.ID))
            //        {
            //            _NewMCTProducts.Add(product);
            //        }
            //    }
            //    labelNewMCT.Text = _NewMCTProducts.Count + " new MCT products to add";

            //    foreach (ShopifyProduct product in _GCProducts)
            //    {
            //        if (!dataContext.Gcproducts.Any(gcp => gcp.Id == product.ID))
            //        {
            //            _NewGCProducts.Add(product);
            //        }
            //    }
            //    labelNewGC.Text = _NewGCProducts.Count + " new GC products to add";

            //    foreach (EtsyListing listing in _EtsyListings)
            //    {
            //        if (!dataContext.EtsyListings.Any(el => el.Id == listing.ID))
            //        {
            //            _NewEtsyListings.Add(listing);
            //        }
            //    }
            //    labelNewEtsy.Text = _NewEtsyListings.Count + " new Etsy listings to add";

            //    buttonAddData.Enabled = _NewMCTProducts.Count > 0 || _NewGCProducts.Count > 0 || _NewEtsyListings.Count > 0;
            //}

            _ProductsToUpdate = _MCTProducts;

            _SelectedProduct = _ProductsToUpdate[_Index];

            LoadProductDetails();
        }

        private void getCrookedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (FormShopify form = new FormShopify(ShopifyBrand.GC, _GCProducts, _EtsyListings))
            {
                form.ShowDialog();
            }
        }

        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            string searchTerms = textBoxSearch.Text.ToLower();

            listBoxSearch.Visible = false;
            if (String.IsNullOrEmpty(searchTerms))
            {
                return;
            }

            List<ShopifyProduct> matches = Utilities.GetProductMatches(searchTerms, ShopifyBrand.MCT, _MCTProducts);

            if (matches.Count == 0)
            {
                return;
            }

            listBoxSearch.Items.Clear();

            matches.ForEach(delegate (ShopifyProduct product)
            {
                listBoxSearch.Items.Add(product.Title);
            });

            listBoxSearch.Visible = true;
        }

        private void listBoxSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            _SelectedProduct = _MCTProducts.First(product => product.Title == listBoxSearch.SelectedItem.ToString());

            textBoxSearch.Text = listBoxSearch.SelectedItem.ToString();
            listBoxSearch.Visible = false;

            LoadProductDetails();
        }

        private void DisableControls()
        {
            buttonPrevious.Enabled = false;
            buttonNext.Enabled = false;
            buttonUpdateGC.Enabled = false;
            buttonUpdateFromGC.Enabled = false;
            buttonCopyToEtsy.Enabled = false;
            buttonUpdateEtsy.Enabled = false;
            linkLabelEditMCTDetails.Enabled = false;
            linkLabelEditGC.Enabled = false;
            buttonMatchGC.Enabled = false;
            buttonUnmatchGC.Enabled = false;
            buttonClearGC.Enabled = false;
            linkLabelEditEtsy.Enabled = false;
            buttonMatchEtsy.Enabled = false;
            buttonUnmatchEtsy.Enabled = false;
            buttonClearEtsy.Enabled = false;
            checkBoxOverwritePrice.Enabled = false;
            buttonMatchBoth.Enabled = false;
        }

        private void EnableControls()
        {
            buttonPrevious.Enabled = _Index > 0;
            buttonNext.Enabled = _Index < _ProductsToUpdate.Count;

            buttonUpdateGC.Enabled = listBoxGCProducts.SelectedIndex > -1;
            buttonUpdateFromGC.Enabled = listBoxGCProducts.SelectedIndex > -1;
            buttonCopyToEtsy.Enabled = !etsyMatched;
            buttonUpdateEtsy.Enabled = listBoxEtsyListings.SelectedIndex > -1;
            checkBoxOverwritePrice.Enabled = listBoxEtsyListings.SelectedIndex > -1;
            linkLabelEditMCT.Enabled = true;
            linkLabelEditMCTDetails.Enabled = true;

            linkLabelEditGC.Enabled = listBoxGCProducts.SelectedIndex > -1;
            buttonMatchGC.Enabled = !gcMatched && listBoxGCProducts.SelectedIndex > -1;
            buttonUnmatchGC.Enabled = gcMatched;
            buttonClearGC.Enabled = listBoxGCProducts.SelectedIndex > -1 & !gcMatched;
            linkLabelEditGCDetails.Enabled = listBoxGCProducts.SelectedIndex > -1;

            linkLabelEditEtsy.Enabled = listBoxEtsyListings.SelectedIndex > -1;
            buttonMatchEtsy.Enabled = !etsyMatched && listBoxEtsyListings.SelectedIndex > -1;
            buttonUnmatchEtsy.Enabled = etsyMatched;
            buttonClearEtsy.Enabled = listBoxEtsyListings.SelectedIndex > -1 & !etsyMatched;
            linkLabelEditEtsyDetails.Enabled = listBoxEtsyListings.SelectedIndex > -1;

            buttonMatchBoth.Enabled = !gcMatched && listBoxGCProducts.SelectedIndex > -1 & !etsyMatched && listBoxEtsyListings.SelectedIndex > -1;
        }

        private void LoadProductDetails()
        {
            DisableControls();

            labelTitle.Text = _SelectedProduct.Title;
            labelNumber.Text = String.Format("Megan Crook Textiles - {0} of {1}", _Index + 1, _ProductsToUpdate.Count);
            this.Refresh();
            if (_SelectedProduct.Images.Count > 0)
            {
                pictureBoxMain.Load(_SelectedProduct.Images[0].URL);
                this.Refresh();
            }
            labelMCTStatus.Text = _SelectedProduct.Status[0].ToString().ToUpper() + _SelectedProduct.Status[1..];
            labelMCTPublished.Text = "Sales Channels: " + (_SelectedProduct.PublishedScope != null ? _SelectedProduct.PublishedScope[0].ToString().ToUpper() + _SelectedProduct.PublishedScope[1..] : "None");
            this.Refresh();
            listViewMCTVariants.Items.Clear();
            foreach (ShopifyProductVariant variant in _SelectedProduct.Variants)
            {
                listViewMCTVariants.Items.Add(new ListViewItem(new[]
                {
                    variant.Size,
                    variant.Colour,
                    "£" + decimal.Parse(variant.Price).ToString("0.00"),
                    "£" + variant.CompareAtPrice ?? "0.00",
                    variant.Quantity.ToString(),
                    variant.InventoryPolicy
                }));
                this.Refresh();
            }
            this.Refresh();

            labelGCStatus.Text = String.Empty;
            _MatchedShopifyProductIds = new List<long>();
            listBoxGCProducts.Items.Clear();
            using (Models.CrookedPortalContext dataContext = new Models.CrookedPortalContext())
            {
                if (dataContext.CrossPlatformProductListings.Any(cppl => cppl.MctproductId == _SelectedProduct.ID && cppl.GcproductId != null))
                {
                    gcMatched = true;

                    Models.CrossPlatformProductListing record = dataContext.CrossPlatformProductListings.First(cppl => cppl.MctproductId == _SelectedProduct.ID);

                    _CorrespondingProduct = _GCProducts.First(product => product.ID == record.GcproductId);

                    _MatchedShopifyProductIds.Add(_CorrespondingProduct.ID);
                    listBoxGCProducts.Items.Add(_CorrespondingProduct.Title);
                    listBoxGCProducts.SetSelected(0, true);
                    if (_CorrespondingProduct.Images.Count > 0)
                    {
                        pictureBoxGC.Load(_CorrespondingProduct.Images[0].URL);
                        this.Refresh();
                    }
                    labelGCStatus.Text = _CorrespondingProduct.Status[0].ToString().ToUpper() + _CorrespondingProduct.Status[1..];
                    listViewGCVariants.Items.Clear();
                    foreach (ShopifyProductVariant variant in _CorrespondingProduct.Variants)
                    {
                        listViewGCVariants.Items.Add(new ListViewItem(new[]
                        {
                            variant.Size,
                            variant.Colour,
                            "£" + decimal.Parse(variant.Price).ToString("0.00"),
                            "£" + variant.CompareAtPrice ?? "0.00",
                            variant.Quantity.ToString(),
                            variant.InventoryPolicy
                        }));
                        this.Refresh();
                    }
                    this.Refresh();
                }
                else
                {
                    _SelectedProduct.GetProductMatches(ShopifyBrand.MCT, _GCProducts).ForEach(delegate (ShopifyProduct product)
                    {
                        _MatchedShopifyProductIds.Add(product.ID);
                        listBoxGCProducts.Items.Add(product.Title);
                    });
                    gcMatched = false;
                    pictureBoxGC.Image = null;
                    listViewGCVariants.Items.Clear();
                }
            }

            labelEtsyState.Text = String.Empty;
            _MatchedEtsyListingIds = new List<long>();
            listBoxEtsyListings.Items.Clear();
            using (Models.CrookedPortalContext dataContext = new Models.CrookedPortalContext())
            {
                if (dataContext.CrossPlatformProductListings.Any(cppl => cppl.MctproductId == _SelectedProduct.ID && cppl.EtsyListingId != null))
                {
                    etsyMatched = true;

                    Models.CrossPlatformProductListing record = dataContext.CrossPlatformProductListings.First(cppl => cppl.MctproductId == _SelectedProduct.ID);

                    _CorrespondingListing = _EtsyListings.First(listing => listing.ID == record.EtsyListingId);

                    _MatchedEtsyListingIds.Add(_CorrespondingListing.ID);
                    listBoxEtsyListings.Items.Add(_CorrespondingListing.Title);
                    listBoxEtsyListings.SetSelected(0, true);

                    EtsyListingImage image = JsonSerializer.Deserialize<EtsyListingImagesJSONRoot>(Utilities.GetJSONFromEtsy(Properties.Resources.EtsyListingImagesURL + _CorrespondingListing.ID + "/images")).Images.OrderBy(i => i.Rank).First();

                    pictureBoxEtsy.Load(image.URL);
                    this.Refresh();

                    labelEtsyState.Text = _CorrespondingListing.State == "edit" ? "Inactive" : _CorrespondingListing.State[0].ToString().ToUpper() + _CorrespondingListing.State[1..];
                    this.Refresh();
                    listViewEtsyProducts.Items.Clear();
                    foreach (EtsyListingProduct product in JsonSerializer.Deserialize<EtsyListingProductsJSONRoot>(Utilities.GetJSONFromEtsy(Properties.Resources.EtsyListingProductsURL + _CorrespondingListing.ID + "/inventory")).Products)
                    {
                        listViewEtsyProducts.Items.Add(new ListViewItem(new string[]
                        {
                            product.PropertyValues.Count > 0 ? product.PropertyValues[0].Values[0] : "",
                            product.PropertyValues.Count > 1 ? product.PropertyValues[1].Values[0] : "",
                            "£" + product.Offerings[0].Price.Value.ToString("0.00"),
                            product.Offerings.Count > 0 ? product.Offerings[0].Quantity.ToString() : ""
                        }));
                        this.Refresh();
                    }
                    this.Refresh();
                }
                else
                {
                    etsyMatched = false;

                    _SelectedProduct.GetListingMatches(ShopifyBrand.MCT, _EtsyListings).ForEach(delegate (EtsyListing listing)
                    {
                        _MatchedEtsyListingIds.Add(listing.ID);
                        listBoxEtsyListings.Items.Add(listing.Title);
                    });
                    pictureBoxEtsy.Image = null;
                    listViewEtsyProducts.Items.Clear();
                }
            }
            this.Refresh();

            EnableControls();
        }

        private void buttonPrevious_Click(object sender, EventArgs e)
        {
            DisableControls();

            _Index--;
            Properties.Settings.Default.Index = _Index;
            Properties.Settings.Default.Save();

            _SelectedProduct = _ProductsToUpdate[_Index];

            LoadProductDetails();
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            DisableControls();

            _Index++;
            Properties.Settings.Default.Index = _Index;
            Properties.Settings.Default.Save();

            _SelectedProduct = _ProductsToUpdate[_Index];

            LoadProductDetails();
        }

        private void buttonUpdateGC_Click(object sender, EventArgs e)
        {
            DisableControls();

            UpdateGC();

            UpdateGCData();

            EnableControls();
        }

        private void UpdateGC()
        {
            //_CorrespondingProduct.Title = _SelectedProduct.Title;
            //_CorrespondingProduct.UpdateTitle(ShopifyBrand.GC, _SelectedProduct.Title);
            _CorrespondingProduct.UpdateFromCorrespondingProduct(ShopifyBrand.GC, _SelectedProduct);

            using (Models.CrookedPortalContext dataContext = new Models.CrookedPortalContext())
            {
                foreach (Models.GcproductVariant variant in dataContext.GcproductVariants.Where(gcpv => gcpv.ProductId == _CorrespondingProduct.ID))
                {
                    dataContext.Remove(variant);
                }

                dataContext.SaveChanges();

                foreach (ShopifyProductVariant variant in _CorrespondingProduct.Variants)
                {
                    dataContext.Add(new Models.GcproductVariant()
                    {
                        Id = variant.ID,
                        ProductId = _CorrespondingProduct.ID,
                        InventoryItemId = variant.InventoryItemID,
                        InventoryPolicy = variant.InventoryPolicy,
                        Price = decimal.Parse(variant.Price),
                        CompareAtPrice = decimal.TryParse(variant.CompareAtPrice, out decimal compareAtPrice) ? decimal.Parse(variant.CompareAtPrice) : 0.00m,
                        Quantity = variant.Quantity,
                        Size = variant.Size,
                        Colour = !String.IsNullOrEmpty(variant.Colour) ? variant.Colour : null
                    });
                }

                dataContext.SaveChanges();
            }

            //labelEtsyState.Text = _CorrespondingProduct.Status[0].ToString().ToUpper() + _CorrespondingProduct.Status[1..];
            //listBoxGCProducts.Items.Clear();
            //listBoxGCProducts.Items.Add(_CorrespondingListing.Title);
            //listBoxGCProducts.SetSelected(0, true);
            //listViewGCVariants.Items.Clear();
            //foreach (ShopifyProductVariant variant in _CorrespondingProduct.Variants)
            //{
            //    listViewGCVariants.Items.Add(new ListViewItem(new[]
            //    {
            //        variant.Size,
            //        variant.Colour,
            //        "£" + decimal.Parse(variant.Price).ToString("0.00"),
            //        "£" + variant.CompareAtPrice ?? "0.00",
            //        variant.Quantity.ToString(),
            //        variant.InventoryPolicy
            //    }));
            //}
        }

        private void UpdateMCT()
        {
            //_CorrespondingProduct.Title = _SelectedProduct.Title;
            //_CorrespondingProduct.UpdateTitle(ShopifyBrand.GC, _SelectedProduct.Title);
            _CorrespondingProduct.UpdateFromCorrespondingProduct(ShopifyBrand.MCT, _SelectedProduct);

            using (Models.CrookedPortalContext dataContext = new Models.CrookedPortalContext())
            {
                foreach (Models.MctproductVariant variant in dataContext.MctproductVariants.Where(mctpv => mctpv.ProductId == _CorrespondingProduct.ID))
                {
                    dataContext.Remove(variant);
                }

                dataContext.SaveChanges();

                foreach (ShopifyProductVariant variant in _CorrespondingProduct.Variants)
                {
                    dataContext.Add(new Models.MctproductVariant()
                    {
                        Id = variant.ID,
                        ProductId = _CorrespondingProduct.ID,
                        InventoryItemId = variant.InventoryItemID,
                        InventoryPolicy = variant.InventoryPolicy,
                        Price = decimal.Parse(variant.Price),
                        CompareAtPrice = decimal.TryParse(variant.CompareAtPrice, out decimal compareAtPrice) ? decimal.Parse(variant.CompareAtPrice) : 0.00m,
                        Quantity = variant.Quantity,
                        Size = variant.Size,
                        Colour = !String.IsNullOrEmpty(variant.Colour) ? variant.Colour : null
                    });
                }

                dataContext.SaveChanges();
            }

            //labelEtsyState.Text = _CorrespondingProduct.Status[0].ToString().ToUpper() + _CorrespondingProduct.Status[1..];
            //listBoxGCProducts.Items.Clear();
            //listBoxGCProducts.Items.Add(_CorrespondingListing.Title);
            //listBoxGCProducts.SetSelected(0, true);
            //listViewGCVariants.Items.Clear();
            //foreach (ShopifyProductVariant variant in _CorrespondingProduct.Variants)
            //{
            //    listViewGCVariants.Items.Add(new ListViewItem(new[]
            //    {
            //        variant.Size,
            //        variant.Colour,
            //        "£" + decimal.Parse(variant.Price).ToString("0.00"),
            //        "£" + variant.CompareAtPrice ?? "0.00",
            //        variant.Quantity.ToString(),
            //        variant.InventoryPolicy
            //    }));
            //}
        }

        private void UpdateGCData()
        {
            using (Models.CrookedPortalContext dataContext = new Models.CrookedPortalContext())
            {
                Models.CrossPlatformProductListing crossPlatformProductListing = dataContext.CrossPlatformProductListings.First(cppl => cppl.MctproductId == _SelectedProduct.ID);
                List<Models.MctproductVariant> mctProductVariants = dataContext.MctproductVariants.Where(mctpv => mctpv.ProductId == crossPlatformProductListing.MctproductId).ToList();
                List<Models.GcproductVariant> gcProductVariants = dataContext.GcproductVariants.Where(gcpv => gcpv.ProductId == crossPlatformProductListing.GcproductId).ToList();

                for (int i = 0; i < mctProductVariants.Count; i++)
                {
                    mctProductVariants.First(mctpv => mctpv.Id == mctProductVariants[i].Id).GcvariantId = gcProductVariants.First(gcpv => gcpv.Size == mctProductVariants[i].Size && gcpv.Colour == mctProductVariants[i].Colour).Id;
                    gcProductVariants.First(gcpv => gcpv.Id == gcProductVariants[i].Id).MctvariantId = mctProductVariants.First(mctpv => mctpv.Size == gcProductVariants[i].Size && mctpv.Colour == gcProductVariants[i].Colour).Id;

                    if (crossPlatformProductListing.GcproductId != null)
                    {
                        List<Models.EtsyListingProduct> etsyListingProducts = dataContext.EtsyListingProducts.Where(elp => elp.ListingId == crossPlatformProductListing.EtsyListingId).ToList();

                        mctProductVariants.First(mctpv => mctpv.Id == mctProductVariants[i].Id).EtsyProductId = etsyListingProducts.First(elp => elp.Size == mctProductVariants[i].Size && elp.Colour == mctProductVariants[i].Colour).Id;
                        gcProductVariants.First(gcpv => gcpv.Id == gcProductVariants[i].Id).EtsyProductId = etsyListingProducts.First(elp => elp.Size == gcProductVariants[i].Size && elp.Colour == gcProductVariants[i].Colour).Id;
                        etsyListingProducts.First(elp => elp.Id == etsyListingProducts[i].Id).MctvariantId = mctProductVariants.First(mctpv => mctpv.Size == etsyListingProducts[i].Size && mctpv.Colour == etsyListingProducts[i].Colour).Id;
                        etsyListingProducts.First(elp => elp.Id == etsyListingProducts[i].Id).GcvariantId = gcProductVariants.First(gcv => gcv.Size == etsyListingProducts[i].Size && gcv.Colour == etsyListingProducts[i].Colour).Id;
                    }
                }

                dataContext.SaveChanges();
            }
        }

        private void UpdateMCTData()
        {
            using (Models.CrookedPortalContext dataContext = new Models.CrookedPortalContext())
            {
                Models.CrossPlatformProductListing crossPlatformProductListing = dataContext.CrossPlatformProductListings.First(cppl => cppl.GcproductId == _SelectedProduct.ID);
                List<Models.MctproductVariant> mctProductVariants = dataContext.MctproductVariants.Where(mctpv => mctpv.ProductId == crossPlatformProductListing.MctproductId).ToList();
                List<Models.GcproductVariant> gcProductVariants = dataContext.GcproductVariants.Where(gcpv => gcpv.ProductId == crossPlatformProductListing.GcproductId).ToList();

                for (int i = 0; i < gcProductVariants.Count; i++)
                {
                    mctProductVariants.First(mctpv => mctpv.Id == mctProductVariants[i].Id).GcvariantId = gcProductVariants.First(gcpv => gcpv.Size == mctProductVariants[i].Size && gcpv.Colour == mctProductVariants[i].Colour).Id;
                    gcProductVariants.First(gcpv => gcpv.Id == gcProductVariants[i].Id).MctvariantId = mctProductVariants.First(mctpv => mctpv.Size == gcProductVariants[i].Size && mctpv.Colour == gcProductVariants[i].Colour).Id;

                    if (crossPlatformProductListing.MctproductId != null)
                    {
                        List<Models.EtsyListingProduct> etsyListingProducts = dataContext.EtsyListingProducts.Where(elp => elp.ListingId == crossPlatformProductListing.EtsyListingId).ToList();

                        mctProductVariants.First(mctpv => mctpv.Id == mctProductVariants[i].Id).EtsyProductId = etsyListingProducts.First(elp => elp.Size == mctProductVariants[i].Size && elp.Colour == mctProductVariants[i].Colour).Id;
                        gcProductVariants.First(gcpv => gcpv.Id == gcProductVariants[i].Id).EtsyProductId = etsyListingProducts.First(elp => elp.Size == gcProductVariants[i].Size && elp.Colour == gcProductVariants[i].Colour).Id;
                        etsyListingProducts.First(elp => elp.Id == etsyListingProducts[i].Id).MctvariantId = mctProductVariants.First(mctpv => mctpv.Size == etsyListingProducts[i].Size && mctpv.Colour == etsyListingProducts[i].Colour).Id;
                        etsyListingProducts.First(elp => elp.Id == etsyListingProducts[i].Id).GcvariantId = gcProductVariants.First(gcv => gcv.Size == etsyListingProducts[i].Size && gcv.Colour == etsyListingProducts[i].Colour).Id;
                    }
                }

                dataContext.SaveChanges();
            }
        }

        private void buttonUpdateFromGC_Click(object sender, EventArgs e)
        {
            DisableControls();

            _SelectedProduct.Title = _CorrespondingProduct.Title;
            _SelectedProduct.UpdateTitle(ShopifyBrand.MCT, _CorrespondingProduct.Title);
            _SelectedProduct.UpdateFromCorrespondingProduct(ShopifyBrand.MCT, _CorrespondingProduct);

            using (Models.CrookedPortalContext dataContext = new Models.CrookedPortalContext())
            {
                foreach (Models.MctproductVariant variant in dataContext.MctproductVariants.Where(mctpv => mctpv.ProductId == _SelectedProduct.ID))
                {
                    dataContext.Remove(variant);
                }

                dataContext.SaveChanges();

                foreach (ShopifyProductVariant variant in _SelectedProduct.Variants)
                {
                    dataContext.Add(new Models.MctproductVariant()
                    {
                        Id = variant.ID,
                        ProductId = _SelectedProduct.ID,
                        InventoryItemId = variant.InventoryItemID,
                        InventoryPolicy = variant.InventoryPolicy,
                        Price = decimal.Parse(variant.Price),
                        CompareAtPrice = decimal.TryParse(variant.CompareAtPrice, out decimal compareAtPrice) ? decimal.Parse(variant.CompareAtPrice) : 0.00m,
                        Quantity = variant.Quantity,
                        Size = variant.Size,
                        Colour = !String.IsNullOrEmpty(variant.Colour) ? variant.Colour : null
                    });
                }

                dataContext.SaveChanges();
            }

            listViewMCTVariants.Items.Clear();
            foreach (ShopifyProductVariant variant in _SelectedProduct.Variants)
            {
                listViewMCTVariants.Items.Add(new ListViewItem(new[]
                {
                    variant.Size,
                    variant.Colour,
                    "£" + decimal.Parse(variant.Price).ToString("0.00"),
                    "£" + variant.CompareAtPrice ?? "0.00",
                    variant.Quantity.ToString(),
                    variant.InventoryPolicy
                }));
            }

            EnableControls();
        }

        private void buttonCopyToEtsy_Click(object sender, EventArgs e)
        {
            DisableControls();

            using (FormEditCopy form = new FormEditCopy(ref _SelectedProduct, ShopifyBrand.MCT, false))
            {
                form.ShowDialog();

                _EtsyListings.Add(form.EtsyListing);
                _CorrespondingListing = form.EtsyListing;

                using (Models.CrookedPortalContext dataContext = new Models.CrookedPortalContext())
                {
                    Models.CrossPlatformProductListing crossPlatformProductListing;

                    if (form.ShopifyBrand == ShopifyBrand.MCT)
                    {
                        if (dataContext.CrossPlatformProductListings.Any(cppl => cppl.MctproductId == _SelectedProduct.ID))
                        {
                            crossPlatformProductListing = dataContext.CrossPlatformProductListings.First(cppl => cppl.MctproductId == _SelectedProduct.ID);
                            crossPlatformProductListing.EtsyListingId = _CorrespondingListing.ID;

                            if (!dataContext.EtsyListings.Any(listing => listing.Id == _CorrespondingListing.ID))
                            {
                                dataContext.Add(new Models.EtsyListing()
                                {
                                    Id = _CorrespondingListing.ID,
                                    Title = _CorrespondingListing.Title,
                                    Description = _CorrespondingListing.Description,
                                    State = _CorrespondingListing.State,
                                    LastUpdated = DateTime.Now
                                });

                                List<EtsyListingProduct> listingProducts = JsonSerializer.Deserialize<EtsyListingProductsJSONRoot>(Utilities.GetJSONFromEtsy(Properties.Resources.EtsyListingProductsURL + _CorrespondingListing.ID + "/inventory")).Products;

                                foreach (EtsyListingProduct product in listingProducts)
                                {
                                    dataContext.Add(new Models.EtsyListingProduct()
                                    {
                                        Id = product.ID,
                                        ListingId = _CorrespondingListing.ID,
                                        Price = product.Offerings[0].Price.Value,
                                        Quantity = product.Offerings[0].Quantity,
                                        Size = product.PropertyValues.Count > 0 ? product.PropertyValues[0].Values[0] : null,
                                        Colour = product.PropertyValues.Count > 1 ? product.PropertyValues[1].Values[0] : null
                                    });
                                }

                                List<EtsyListingImage> images = JsonSerializer.Deserialize<EtsyListingImagesJSONRoot>(Utilities.GetJSONFromEtsy(Properties.Resources.EtsyListingImagesURL + _CorrespondingListing.ID + "/images")).Images;

                                foreach (EtsyListingImage image in images)
                                {
                                    dataContext.Add(new Models.EtsyListingImage()
                                    {
                                        ImageId = image.ID,
                                        ListingId = _CorrespondingListing.ID,
                                        Rank = image.Rank,
                                        Url = image.URL
                                    });
                                }
                            }

                            crossPlatformProductListing.LastUpdated = DateTime.Now;
                        }
                        else
                        {
                            crossPlatformProductListing = new Models.CrossPlatformProductListing
                            {
                                MctproductId = _SelectedProduct.ID,
                                EtsyListingId = _CorrespondingListing.ID
                            };

                            if (!dataContext.Mctproducts.Any(product => product.Id == _SelectedProduct.ID))
                            {
                                dataContext.Add(new Models.Mctproduct()
                                {
                                    Id = _SelectedProduct.ID,
                                    Title = _SelectedProduct.Title,
                                    Description = _SelectedProduct.Description,
                                    Tags = _SelectedProduct.Tags,
                                    Status = _SelectedProduct.Status,
                                    LastUpdated = DateTime.Now,
                                    PublishedScope = _SelectedProduct.PublishedScope
                                });

                                foreach (ShopifyProductVariant variant in _SelectedProduct.Variants)
                                {
                                    dataContext.Add(new Models.MctproductVariant()
                                    {
                                        Id = variant.ID,
                                        ProductId = _SelectedProduct.ID,
                                        InventoryItemId = variant.InventoryItemID,
                                        InventoryPolicy = variant.InventoryPolicy,
                                        Price = decimal.Parse(variant.Price),
                                        CompareAtPrice = decimal.TryParse(variant.CompareAtPrice, out decimal compareAtPrice) ? decimal.Parse(variant.CompareAtPrice) : 0.00m,
                                        Quantity = variant.Quantity,
                                        Size = variant.Size,
                                        Colour = variant.Colour
                                    });
                                }

                                foreach (ShopifyProductImage image in _SelectedProduct.Images)
                                {
                                    dataContext.Add(new Models.MctproductImage()
                                    {
                                        Id = image.ID,
                                        ProductId = _SelectedProduct.ID,
                                        Position = image.Position,
                                        Url = image.URL
                                    });
                                }
                            }

                            if (!dataContext.EtsyListings.Any(listing => listing.Id == _CorrespondingListing.ID))
                            {
                                dataContext.Add(new Models.EtsyListing()
                                {
                                    Id = _CorrespondingListing.ID,
                                    Title = _CorrespondingListing.Title,
                                    Description = _CorrespondingListing.Description,
                                    State = _CorrespondingListing.State,
                                    LastUpdated = DateTime.Now
                                });

                                List<EtsyListingProduct> listingProducts = JsonSerializer.Deserialize<EtsyListingProductsJSONRoot>(Utilities.GetJSONFromEtsy(Properties.Resources.EtsyListingProductsURL + _CorrespondingListing.ID + "/inventory")).Products;

                                foreach (EtsyListingProduct product in listingProducts)
                                {
                                    dataContext.Add(new Models.EtsyListingProduct()
                                    {
                                        Id = product.ID,
                                        ListingId = _CorrespondingListing.ID,
                                        Price = product.Offerings[0].Price.Value,
                                        Quantity = product.Offerings[0].Quantity,
                                        Size = product.PropertyValues.Count > 0 ? product.PropertyValues[0].Values[0] : null,
                                        Colour = product.PropertyValues.Count > 1 ? product.PropertyValues[1].Values[0] : null
                                    });
                                }

                                List<EtsyListingImage> images = JsonSerializer.Deserialize<EtsyListingImagesJSONRoot>(Utilities.GetJSONFromEtsy(Properties.Resources.EtsyListingImagesURL + _CorrespondingListing.ID + "/images")).Images;

                                foreach (EtsyListingImage image in images)
                                {
                                    dataContext.Add(new Models.EtsyListingImage()
                                    {
                                        ImageId = image.ID,
                                        ListingId = _CorrespondingListing.ID,
                                        Rank = image.Rank,
                                        Url = image.URL
                                    });
                                }
                            }

                            crossPlatformProductListing.LastUpdated = DateTime.Now;

                            dataContext.Add(crossPlatformProductListing);
                        }

                        dataContext.SaveChanges();

                        List<Models.MctproductVariant> mctProductVariants = dataContext.MctproductVariants.Where(mctpv => mctpv.ProductId == crossPlatformProductListing.MctproductId).ToList();
                        List<Models.EtsyListingProduct> etsyListingProducts = dataContext.EtsyListingProducts.Where(elp => elp.ListingId == crossPlatformProductListing.EtsyListingId).ToList();

                        for (int i = 0; i < mctProductVariants.Count; i++)
                        {
                            mctProductVariants.First(mctpv => mctpv.Id == mctProductVariants[i].Id).EtsyProductId = etsyListingProducts.First(elp => elp.Size == mctProductVariants[i].Size && elp.Colour == mctProductVariants[i].Colour).Id;
                            etsyListingProducts.First(elp => elp.Id == etsyListingProducts[i].Id).MctvariantId = mctProductVariants.First(mctpv => mctpv.Size == etsyListingProducts[i].Size && mctpv.Colour == etsyListingProducts[i].Colour).Id;

                            if (crossPlatformProductListing.GcproductId != null)
                            {
                                List<Models.GcproductVariant> gcProductVariants = dataContext.GcproductVariants.Where(gcpv => gcpv.ProductId == crossPlatformProductListing.GcproductId).ToList();

                                mctProductVariants.First(mctpv => mctpv.Id == mctProductVariants[i].Id).GcvariantId = gcProductVariants.First(gcpv => gcpv.Size == mctProductVariants[i].Size && gcpv.Colour == mctProductVariants[i].Colour).Id;
                                gcProductVariants.First(gcpv => gcpv.Id == gcProductVariants[i].Id).MctvariantId = mctProductVariants.First(mctpv => mctpv.Size == gcProductVariants[i].Size && mctpv.Colour == gcProductVariants[i].Colour).Id;
                                gcProductVariants.First(gcpv => gcpv.Id == gcProductVariants[i].Id).EtsyProductId = etsyListingProducts.First(elp => elp.Size == gcProductVariants[i].Size && elp.Colour == gcProductVariants[i].Colour).Id;
                                etsyListingProducts.First(elp => elp.Id == etsyListingProducts[i].Id).GcvariantId = gcProductVariants.First(gcv => gcv.Size == etsyListingProducts[i].Size && gcv.Colour == etsyListingProducts[i].Colour).Id;
                            }
                        }
                    }
                    else if (form.ShopifyBrand == ShopifyBrand.GC)
                    {
                        if (dataContext.CrossPlatformProductListings.Any(cppl => cppl.GcproductId == _SelectedProduct.ID))
                        {
                            crossPlatformProductListing = dataContext.CrossPlatformProductListings.First(cppl => cppl.GcproductId == _SelectedProduct.ID);
                            crossPlatformProductListing.EtsyListingId = _CorrespondingListing.ID;

                            if (!dataContext.EtsyListings.Any(listing => listing.Id == _CorrespondingListing.ID))
                            {
                                dataContext.Add(new Models.EtsyListing()
                                {
                                    Id = _CorrespondingListing.ID,
                                    Title = _CorrespondingListing.Title,
                                    Description = _CorrespondingListing.Description,
                                    State = _CorrespondingListing.State,
                                    LastUpdated = DateTime.Now
                                });

                                List<EtsyListingProduct> listingProducts = JsonSerializer.Deserialize<EtsyListingProductsJSONRoot>(Utilities.GetJSONFromEtsy(Properties.Resources.EtsyListingProductsURL + _CorrespondingListing.ID + "/inventory")).Products;

                                foreach (EtsyListingProduct product in listingProducts)
                                {
                                    dataContext.Add(new Models.EtsyListingProduct()
                                    {
                                        Id = product.ID,
                                        ListingId = _CorrespondingListing.ID,
                                        Price = product.Offerings[0].Price.Value,
                                        Quantity = product.Offerings[0].Quantity,
                                        Size = product.PropertyValues.Count > 0 ? product.PropertyValues[0].Values[0] : null,
                                        Colour = product.PropertyValues.Count > 1 ? product.PropertyValues[1].Values[0] : null
                                    });
                                }

                                List<EtsyListingImage> images = JsonSerializer.Deserialize<EtsyListingImagesJSONRoot>(Utilities.GetJSONFromEtsy(Properties.Resources.EtsyListingImagesURL + _CorrespondingListing.ID + "/images")).Images;

                                foreach (EtsyListingImage image in images)
                                {
                                    dataContext.Add(new Models.EtsyListingImage()
                                    {
                                        ImageId = image.ID,
                                        ListingId = _CorrespondingListing.ID,
                                        Rank = image.Rank,
                                        Url = image.URL
                                    });
                                }
                            }

                            crossPlatformProductListing.LastUpdated = DateTime.Now;
                        }
                        else
                        {
                            crossPlatformProductListing = new Models.CrossPlatformProductListing
                            {
                                GcproductId = _SelectedProduct.ID,
                                EtsyListingId = _CorrespondingListing.ID
                            };

                            if (!dataContext.Gcproducts.Any(product => product.Id == _SelectedProduct.ID))
                            {
                                dataContext.Add(new Models.Gcproduct()
                                {
                                    Id = _SelectedProduct.ID,
                                    Title = _SelectedProduct.Title,
                                    Description = _SelectedProduct.Description,
                                    Tags = _SelectedProduct.Tags,
                                    Status = _SelectedProduct.Status,
                                    LastUpdated = DateTime.Now,
                                    PublishedScope = _SelectedProduct.PublishedScope
                                });

                                foreach (ShopifyProductVariant variant in _SelectedProduct.Variants)
                                {
                                    dataContext.Add(new Models.GcproductVariant()
                                    {
                                        Id = variant.ID,
                                        ProductId = _SelectedProduct.ID,
                                        InventoryItemId = variant.InventoryItemID,
                                        InventoryPolicy = variant.InventoryPolicy,
                                        Price = decimal.Parse(variant.Price),
                                        CompareAtPrice = decimal.TryParse(variant.CompareAtPrice, out decimal compareAtPrice) ? decimal.Parse(variant.CompareAtPrice) : 0.00m,
                                        Quantity = variant.Quantity,
                                        Size = variant.Size,
                                        Colour = variant.Colour
                                    });
                                }

                                foreach (ShopifyProductImage image in _SelectedProduct.Images)
                                {
                                    dataContext.Add(new Models.GcproductImage()
                                    {
                                        Id = image.ID,
                                        ProductId = _SelectedProduct.ID,
                                        Position = image.Position,
                                        Url = image.URL
                                    });
                                }
                            }

                            if (!dataContext.EtsyListings.Any(listing => listing.Id == _CorrespondingListing.ID))
                            {
                                dataContext.Add(new Models.EtsyListing()
                                {
                                    Id = _CorrespondingListing.ID,
                                    Title = _CorrespondingListing.Title,
                                    Description = _CorrespondingListing.Description,
                                    State = _CorrespondingListing.State,
                                    LastUpdated = DateTime.Now
                                });

                                List<EtsyListingProduct> listingProducts = JsonSerializer.Deserialize<EtsyListingProductsJSONRoot>(Utilities.GetJSONFromEtsy(Properties.Resources.EtsyListingProductsURL + _CorrespondingListing.ID + "/inventory")).Products;

                                foreach (EtsyListingProduct product in listingProducts)
                                {
                                    dataContext.Add(new Models.EtsyListingProduct()
                                    {
                                        Id = product.ID,
                                        ListingId = _CorrespondingListing.ID,
                                        Price = product.Offerings[0].Price.Value,
                                        Quantity = product.Offerings[0].Quantity,
                                        Size = product.PropertyValues.Count > 0 ? product.PropertyValues[0].Values[0] : null,
                                        Colour = product.PropertyValues.Count > 1 ? product.PropertyValues[1].Values[0] : null
                                    });
                                }

                                List<EtsyListingImage> images = JsonSerializer.Deserialize<EtsyListingImagesJSONRoot>(Utilities.GetJSONFromEtsy(Properties.Resources.EtsyListingImagesURL + _CorrespondingListing.ID + "/images")).Images;

                                foreach (EtsyListingImage image in images)
                                {
                                    dataContext.Add(new Models.EtsyListingImage()
                                    {
                                        ImageId = image.ID,
                                        ListingId = _CorrespondingListing.ID,
                                        Rank = image.Rank,
                                        Url = image.URL
                                    });
                                }
                            }

                            crossPlatformProductListing.LastUpdated = DateTime.Now;

                            dataContext.Add(crossPlatformProductListing);
                        }

                        dataContext.SaveChanges();

                        List<Models.GcproductVariant> gcProductVariants = dataContext.GcproductVariants.Where(gcpv => gcpv.ProductId == crossPlatformProductListing.GcproductId).ToList();
                        List<Models.EtsyListingProduct> etsyListingProducts = dataContext.EtsyListingProducts.Where(elp => elp.ListingId == crossPlatformProductListing.EtsyListingId).ToList();

                        for (int i = 0; i < gcProductVariants.Count; i++)
                        {
                            gcProductVariants.First(gcpv => gcpv.Id == gcProductVariants[i].Id).EtsyProductId = etsyListingProducts.First(elp => elp.Size == gcProductVariants[i].Size && elp.Colour == gcProductVariants[i].Colour).Id;
                            etsyListingProducts.First(elp => elp.Id == etsyListingProducts[i].Id).GcvariantId = gcProductVariants.First(gcpv => gcpv.Size == etsyListingProducts[i].Size && gcpv.Colour == etsyListingProducts[i].Colour).Id;

                            if (crossPlatformProductListing.MctproductId != null)
                            {
                                List<Models.MctproductVariant> mctProductVariants = dataContext.MctproductVariants.Where(mctpv => mctpv.ProductId == crossPlatformProductListing.MctproductId).ToList();

                                mctProductVariants.First(mctpv => mctpv.Id == mctProductVariants[i].Id).GcvariantId = gcProductVariants.First(gcpv => gcpv.Size == mctProductVariants[i].Size && gcpv.Colour == mctProductVariants[i].Colour).Id;
                                mctProductVariants.First(mctpv => mctpv.Id == mctProductVariants[i].Id).EtsyProductId = etsyListingProducts.First(elp => elp.Size == mctProductVariants[i].Size && elp.Colour == mctProductVariants[i].Colour).Id;
                                gcProductVariants.First(gcpv => gcpv.Id == gcProductVariants[i].Id).MctvariantId = mctProductVariants.First(mctpv => mctpv.Size == gcProductVariants[i].Size && mctpv.Colour == gcProductVariants[i].Colour).Id;
                                etsyListingProducts.First(elp => elp.Id == etsyListingProducts[i].Id).MctvariantId = mctProductVariants.First(mctpv => mctpv.Size == etsyListingProducts[i].Size && mctpv.Colour == etsyListingProducts[i].Colour).Id;
                            }
                        }
                    }

                    dataContext.SaveChanges();
                }

                etsyMatched = true;
                _MatchedEtsyListingIds = new List<long>
                {
                    _CorrespondingListing.ID
                };
                listBoxEtsyListings.Items.Clear();
                listBoxEtsyListings.Items.Add(_CorrespondingListing.Title);
                listBoxEtsyListings.SetSelected(0, true);
            }

            EnableControls();
        }

        private void buttonUpdateEtsy_Click(object sender, EventArgs e)
        {
            DisableControls();

            UpdateEtsy();

            UpdateEtsyData();

            EnableControls();
        }

        private void UpdateEtsy()
        {
            //_CorrespondingListing.Title = _SelectedProduct.Title;
            //_CorrespondingListing.UpdateTitle(_SelectedProduct.Title);
            _CorrespondingListing.UpdateProducts(ref _CorrespondingListing, _SelectedProduct.Variants, checkBoxOverwritePrice.Checked);

            using (Models.CrookedPortalContext dataContext = new Models.CrookedPortalContext())
            {
                foreach (Models.EtsyListingProduct product in dataContext.EtsyListingProducts.Where(elp => elp.ListingId == _CorrespondingListing.ID))
                {
                    dataContext.Remove(product);
                }

                dataContext.SaveChanges();

                List<EtsyListingProduct> etsyListingProducts = JsonSerializer.Deserialize<EtsyListingProductsJSONRoot>(Utilities.GetJSONFromEtsy(Properties.Resources.EtsyListingProductsURL + _CorrespondingListing.ID + "/inventory")).Products;

                foreach (EtsyListingProduct listingProduct in etsyListingProducts)
                {
                    dataContext.Add(new Models.EtsyListingProduct()
                    {
                        Id = listingProduct.ID,
                        ListingId = _CorrespondingListing.ID,
                        Price = listingProduct.Offerings[0].Price.Value,
                        Quantity = listingProduct.Offerings[0].Quantity,
                        Size = listingProduct.PropertyValues.Count > 0 ? listingProduct.PropertyValues[0].Values[0] : null,
                        Colour = listingProduct.PropertyValues.Count > 1 ? listingProduct.PropertyValues[1].Values[0] : null
                    });
                }

                dataContext.SaveChanges();
            }

            //labelEtsyState.Text = _CorrespondingListing.State == "edit" ? "Inactive" : _CorrespondingListing.State[0].ToString().ToUpper() + _CorrespondingListing.State[1..];
            //listBoxEtsyListings.Items.Clear();
            //listBoxEtsyListings.Items.Add(_CorrespondingListing.Title);
            //listBoxEtsyListings.SetSelected(0, true);
            //listViewEtsyProducts.Items.Clear();
            //foreach (EtsyListingProduct product in JsonSerializer.Deserialize<EtsyListingProductsJSONRoot>(Utilities.GetJSONFromEtsy(Properties.Resources.EtsyListingProductsURL + _CorrespondingListing.ID + "/inventory")).Products)
            //{
            //    listViewEtsyProducts.Items.Add(new ListViewItem(new string[]
            //    {
            //        product.PropertyValues.Count > 0 ? product.PropertyValues[0].Values[0] : "",
            //        product.PropertyValues.Count > 1 ? product.PropertyValues[1].Values[0] : "",
            //        "£" + product.Offerings[0].Price.Value.ToString("0.00"),
            //        product.Offerings.Count > 0 ? product.Offerings[0].Quantity.ToString() : ""
            //    }));
            //}
        }

        private void UpdateEtsyData()
        {
            if (etsyMatched)
            {
                using (Models.CrookedPortalContext dataContext = new Models.CrookedPortalContext())
                {
                    if (brand == ShopifyBrand.MCT)
                    {
                        Models.CrossPlatformProductListing crossPlatformProductListing = dataContext.CrossPlatformProductListings.First(cppl => cppl.MctproductId == _SelectedProduct.ID);
                        List<Models.MctproductVariant> mctProductVariants = dataContext.MctproductVariants.Where(mctpv => mctpv.ProductId == crossPlatformProductListing.MctproductId).ToList();
                        List<Models.EtsyListingProduct> etsyListingProducts = dataContext.EtsyListingProducts.Where(elp => elp.ListingId == crossPlatformProductListing.EtsyListingId).ToList();

                        for (int i = 0; i < mctProductVariants.Count; i++)
                        {
                            mctProductVariants.First(mctpv => mctpv.Id == mctProductVariants[i].Id).EtsyProductId = etsyListingProducts.First(elp => elp.Size == mctProductVariants[i].Size && elp.Colour == mctProductVariants[i].Colour).Id;
                            etsyListingProducts.First(elp => elp.Id == etsyListingProducts[i].Id).MctvariantId = mctProductVariants.First(mctpv => mctpv.Size == etsyListingProducts[i].Size && mctpv.Colour == etsyListingProducts[i].Colour).Id;

                            if (crossPlatformProductListing.GcproductId != null)
                            {
                                List<Models.GcproductVariant> gcProductVariants = dataContext.GcproductVariants.Where(gcpv => gcpv.ProductId == crossPlatformProductListing.GcproductId).ToList();

                                mctProductVariants.First(mctpv => mctpv.Id == mctProductVariants[i].Id).GcvariantId = gcProductVariants.First(gcpv => gcpv.Size == mctProductVariants[i].Size && gcpv.Colour == mctProductVariants[i].Colour).Id;
                                gcProductVariants.First(gcpv => gcpv.Id == gcProductVariants[i].Id).MctvariantId = mctProductVariants.First(mctpv => mctpv.Size == gcProductVariants[i].Size && mctpv.Colour == gcProductVariants[i].Colour).Id;
                                gcProductVariants.First(gcpv => gcpv.Id == gcProductVariants[i].Id).EtsyProductId = etsyListingProducts.First(elp => elp.Size == gcProductVariants[i].Size && elp.Colour == gcProductVariants[i].Colour).Id;
                                etsyListingProducts.First(elp => elp.Id == etsyListingProducts[i].Id).GcvariantId = gcProductVariants.First(gcv => gcv.Size == etsyListingProducts[i].Size && gcv.Colour == etsyListingProducts[i].Colour).Id;
                            }
                        }
                    }
                    else if (brand == ShopifyBrand.GC)
                    {
                        Models.CrossPlatformProductListing crossPlatformProductListing = dataContext.CrossPlatformProductListings.First(cppl => cppl.GcproductId == _SelectedProduct.ID);
                        List<Models.GcproductVariant> gcProductVariants = dataContext.GcproductVariants.Where(gcpv => gcpv.ProductId == crossPlatformProductListing.GcproductId).ToList();
                        List<Models.EtsyListingProduct> etsyListingProducts = dataContext.EtsyListingProducts.Where(elp => elp.ListingId == crossPlatformProductListing.EtsyListingId).ToList();

                        for (int i = 0; i < gcProductVariants.Count; i++)
                        {
                            gcProductVariants.First(gcpv => gcpv.Id == gcProductVariants[i].Id).EtsyProductId = etsyListingProducts.First(elp => elp.Size == gcProductVariants[i].Size && elp.Colour == gcProductVariants[i].Colour).Id;
                            etsyListingProducts.First(elp => elp.Id == etsyListingProducts[i].Id).GcvariantId = gcProductVariants.First(gcpv => gcpv.Size == etsyListingProducts[i].Size && gcpv.Colour == etsyListingProducts[i].Colour).Id;

                            if (crossPlatformProductListing.MctproductId != null)
                            {
                                List<Models.MctproductVariant> mctProductVariants = dataContext.MctproductVariants.Where(mctpv => mctpv.ProductId == crossPlatformProductListing.MctproductId).ToList();

                                gcProductVariants.First(gcpv => gcpv.Id == gcProductVariants[i].Id).MctvariantId = mctProductVariants.First(mctpv => mctpv.Size == gcProductVariants[i].Size && mctpv.Colour == gcProductVariants[i].Colour).Id;
                                mctProductVariants.First(mctpv => mctpv.Id == mctProductVariants[i].Id).GcvariantId = gcProductVariants.First(gcpv => gcpv.Size == mctProductVariants[i].Size && gcpv.Colour == mctProductVariants[i].Colour).Id;
                                mctProductVariants.First(mctpv => mctpv.Id == mctProductVariants[i].Id).EtsyProductId = etsyListingProducts.First(elp => elp.Size == mctProductVariants[i].Size && elp.Colour == mctProductVariants[i].Colour).Id;
                                etsyListingProducts.First(elp => elp.Id == etsyListingProducts[i].Id).MctvariantId = mctProductVariants.First(mctv => mctv.Size == etsyListingProducts[i].Size && mctv.Colour == etsyListingProducts[i].Colour).Id;
                            }
                        }
                    }

                    dataContext.SaveChanges();
                }
            }
        }

        private void linkLabelEditMCT_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (FormEditCopy form = new FormEditCopy(ref _SelectedProduct, ShopifyBrand.MCT, true))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    labelTitle.Text = _SelectedProduct.Title;
                }
            }
        }

        private void linkLabelEditMCTDetails_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (FormEditDetails form = new FormEditDetails(ShopifyBrand.MCT, ref _SelectedProduct, ref _CorrespondingProduct, ref _CorrespondingListing))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    listViewMCTVariants.Items.Clear();
                    foreach (ShopifyProductVariant variant in _SelectedProduct.Variants)
                    {
                        listViewMCTVariants.Items.Add(new ListViewItem(new[]
                        {
                            variant.Size,
                            variant.Colour,
                            "£" + decimal.Parse(variant.Price).ToString("0.00"),
                            "£" + variant.CompareAtPrice ?? "0.00",
                            variant.Quantity.ToString(),
                            variant.InventoryPolicy
                        }));
                    }

                    if (gcMatched)
                    {
                        listViewGCVariants.Items.Clear();
                        foreach (ShopifyProductVariant variant in _CorrespondingProduct.Variants)
                        {
                            listViewGCVariants.Items.Add(new ListViewItem(new[]
                            {
                                variant.Size,
                                variant.Colour,
                                "£" + decimal.Parse(variant.Price).ToString("0.00"),
                                "£" + variant.CompareAtPrice ?? "0.00",
                                variant.Quantity.ToString(),
                                variant.InventoryPolicy
                            }));
                        }
                    }

                    if (etsyMatched)
                    {
                        UpdateEtsyData();

                        labelEtsyState.Text = _CorrespondingListing.State == "edit" ? "Inactive" : _CorrespondingListing.State[0].ToString().ToUpper() + _CorrespondingListing.State[1..];
                        listViewEtsyProducts.Items.Clear();
                        foreach (EtsyListingProduct product in JsonSerializer.Deserialize<EtsyListingProductsJSONRoot>(Utilities.GetJSONFromEtsy(Properties.Resources.EtsyListingProductsURL + _CorrespondingListing.ID + "/inventory")).Products)
                        {
                            listViewEtsyProducts.Items.Add(new ListViewItem(new string[]
                            {
                            product.PropertyValues.Count > 0 ? product.PropertyValues[0].Values[0] : "",
                            product.PropertyValues.Count > 1 ? product.PropertyValues[1].Values[0] : "",
                            "£" + product.Offerings[0].Price.Value.ToString("0.00"),
                            product.Offerings.Count > 0 ? product.Offerings[0].Quantity.ToString() : ""
                            }));
                        }
                    }
                }
            }
        }

        private void linkLabelEditGC_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            int index = listBoxGCProducts.SelectedIndex;

            using (FormEditCopy form = new FormEditCopy(ref _CorrespondingProduct, ShopifyBrand.GC, true))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    listViewGCVariants.Items.Clear();
                    listBoxGCProducts.Items[index] = _CorrespondingProduct.Title;
                    typeof(System.Windows.Forms.ListBox).InvokeMember("RefreshItems", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod, null, listBoxGCProducts, new object[] { });
                    listBoxGCProducts.SelectedIndex = index;
                }
            }
        }

        private void buttonMatchGC_Click(object sender, EventArgs e)
        {
            DisableControls();

            UpdateGC();

            using (Models.CrookedPortalContext dataContext = new Models.CrookedPortalContext())
            {
                Models.CrossPlatformProductListing crossPlatformProductListing;

                if (dataContext.CrossPlatformProductListings.Any(cppl => cppl.MctproductId == _SelectedProduct.ID))
                {
                    crossPlatformProductListing = dataContext.CrossPlatformProductListings.First(cppl => cppl.MctproductId == _SelectedProduct.ID);
                    crossPlatformProductListing.GcproductId = _CorrespondingProduct.ID;

                    if (!dataContext.Gcproducts.Any(product => product.Id == _CorrespondingProduct.ID))
                    {
                        dataContext.Add(new Models.Gcproduct()
                        {
                            Id = _CorrespondingProduct.ID,
                            Title = _CorrespondingProduct.Title,
                            Description = _CorrespondingProduct.Description,
                            Tags = _CorrespondingProduct.Tags,
                            Status = _CorrespondingProduct.Status,
                            LastUpdated = DateTime.Now,
                            PublishedScope = _CorrespondingProduct.PublishedScope
                        });

                        foreach (ShopifyProductVariant variant in _CorrespondingProduct.Variants)
                        {
                            dataContext.Add(new Models.GcproductVariant()
                            {
                                Id = variant.ID,
                                ProductId = _CorrespondingProduct.ID,
                                InventoryItemId = variant.InventoryItemID,
                                InventoryPolicy = variant.InventoryPolicy,
                                Price = decimal.Parse(variant.Price),
                                CompareAtPrice = decimal.TryParse(variant.CompareAtPrice, out decimal compareAtPrice) ? decimal.Parse(variant.CompareAtPrice) : 0.00m,
                                Quantity = variant.Quantity,
                                Size = variant.Size,
                                Colour = variant.Colour
                            });
                        }

                        foreach (ShopifyProductImage image in _CorrespondingProduct.Images)
                        {
                            dataContext.Add(new Models.GcproductImage()
                            {
                                Id = image.ID,
                                ProductId = _CorrespondingProduct.ID,
                                Position = image.Position,
                                Url = image.URL
                            });
                        }
                    }

                    crossPlatformProductListing.LastUpdated = DateTime.Now;
                }
                else
                {
                    crossPlatformProductListing = new Models.CrossPlatformProductListing
                    {
                        MctproductId = _SelectedProduct.ID,
                        GcproductId = _CorrespondingProduct.ID
                    };

                    if (!dataContext.Mctproducts.Any(product => product.Id == _SelectedProduct.ID))
                    {
                        dataContext.Add(new Models.Mctproduct()
                        {
                            Id = _SelectedProduct.ID,
                            Title = _SelectedProduct.Title,
                            Description = _SelectedProduct.Description,
                            Tags = _SelectedProduct.Tags,
                            Status = _SelectedProduct.Status,
                            LastUpdated = DateTime.Now,
                            PublishedScope = _SelectedProduct.PublishedScope
                        });

                        foreach (ShopifyProductVariant variant in _SelectedProduct.Variants)
                        {
                            dataContext.Add(new Models.MctproductVariant()
                            {
                                Id = variant.ID,
                                ProductId = _SelectedProduct.ID,
                                InventoryItemId = variant.InventoryItemID,
                                InventoryPolicy = variant.InventoryPolicy,
                                Price = decimal.Parse(variant.Price),
                                CompareAtPrice = decimal.TryParse(variant.CompareAtPrice, out decimal compareAtPrice) ? decimal.Parse(variant.CompareAtPrice) : 0.00m,
                                Quantity = variant.Quantity,
                                Size = variant.Size,
                                Colour = variant.Colour
                            });
                        }

                        foreach (ShopifyProductImage image in _SelectedProduct.Images)
                        {
                            dataContext.Add(new Models.MctproductImage()
                            {
                                Id = image.ID,
                                ProductId = _SelectedProduct.ID,
                                Position = image.Position,
                                Url = image.URL
                            });
                        }
                    }

                    if (!dataContext.Gcproducts.Any(product => product.Id == _CorrespondingProduct.ID))
                    {
                        dataContext.Add(new Models.Gcproduct()
                        {
                            Id = _CorrespondingProduct.ID,
                            Title = _CorrespondingProduct.Title,
                            Description = _CorrespondingProduct.Description,
                            Tags = _CorrespondingProduct.Tags,
                            Status = _CorrespondingProduct.Status,
                            LastUpdated = DateTime.Now,
                            PublishedScope = _CorrespondingProduct.PublishedScope
                        });

                        foreach (ShopifyProductVariant variant in _CorrespondingProduct.Variants)
                        {
                            dataContext.Add(new Models.GcproductVariant()
                            {
                                Id = variant.ID,
                                ProductId = _CorrespondingProduct.ID,
                                InventoryItemId = variant.InventoryItemID,
                                InventoryPolicy = variant.InventoryPolicy,
                                Price = decimal.Parse(variant.Price),
                                CompareAtPrice = decimal.TryParse(variant.CompareAtPrice, out decimal compareAtPrice) ? decimal.Parse(variant.CompareAtPrice) : 0.00m,
                                Quantity = variant.Quantity,
                                Size = variant.Size,
                                Colour = variant.Colour
                            });
                        }

                        foreach (ShopifyProductImage image in _CorrespondingProduct.Images)
                        {
                            dataContext.Add(new Models.GcproductImage()
                            {
                                Id = image.ID,
                                ProductId = _CorrespondingProduct.ID,
                                Position = image.Position,
                                Url = image.URL
                            });
                        }
                    }

                    crossPlatformProductListing.LastUpdated = DateTime.Now;

                    dataContext.Add(crossPlatformProductListing);
                }

                List<Models.MctproductVariant> mctProductVariants = dataContext.MctproductVariants.Where(mctpv => mctpv.ProductId == crossPlatformProductListing.MctproductId).ToList(); ;
                List<Models.GcproductVariant> gcProductVariants = dataContext.GcproductVariants.Where(gcpv => gcpv.ProductId == crossPlatformProductListing.GcproductId).ToList();

                for (int i = 0; i < mctProductVariants.Count; i++)
                {
                    mctProductVariants.First(mctpv => mctpv.Id == mctProductVariants[i].Id).GcvariantId = gcProductVariants.First(gcpv => gcpv.Size == mctProductVariants[i].Size && gcpv.Colour == mctProductVariants[i].Colour).Id;
                    gcProductVariants.First(gcpv => gcpv.Id == gcProductVariants[i].Id).MctvariantId = mctProductVariants.First(mctpv => mctpv.Size == gcProductVariants[i].Size && mctpv.Colour == gcProductVariants[i].Colour).Id;

                    if (crossPlatformProductListing.EtsyListingId != null)
                    {
                        List<Models.EtsyListingProduct> etsyListingProducts = dataContext.EtsyListingProducts.Where(elp => elp.ListingId == crossPlatformProductListing.EtsyListingId).ToList();

                        mctProductVariants.First(mctpv => mctpv.Id == mctProductVariants[i].Id).EtsyProductId = etsyListingProducts.First(elp => elp.Size == mctProductVariants[i].Size && elp.Colour == mctProductVariants[i].Colour).Id;
                        gcProductVariants.First(gcpv => gcpv.Id == gcProductVariants[i].Id).EtsyProductId = etsyListingProducts.First(elp => elp.Size == gcProductVariants[i].Size && elp.Colour == gcProductVariants[i].Colour).Id;
                        etsyListingProducts.First(elp => elp.Id == etsyListingProducts[i].Id).MctvariantId = mctProductVariants.First(mctpv => mctpv.Size == etsyListingProducts[i].Size && mctpv.Colour == etsyListingProducts[i].Colour).Id;
                        etsyListingProducts.First(elp => elp.Id == etsyListingProducts[i].Id).GcvariantId = gcProductVariants.First(gcpv => gcpv.Size == etsyListingProducts[i].Size && gcpv.Colour == etsyListingProducts[i].Colour).Id;
                    }
                }

                dataContext.SaveChanges();
            }

            gcMatched = true;
            listBoxGCProducts.Items.Clear();
            listBoxGCProducts.Items.Add(_CorrespondingProduct.Title);
            listBoxGCProducts.SetSelected(0, true);

            EnableControls();
        }

        private void buttonUnmatchGC_Click(object sender, EventArgs e)
        {
            DisableControls();

            using (Models.CrookedPortalContext dataContext = new Models.CrookedPortalContext())
            {
                Models.CrossPlatformProductListing crossPlatformProductListing = dataContext.CrossPlatformProductListings.First(cppl => cppl.GcproductId == _CorrespondingProduct.ID);

                List<Models.GcproductVariant> gcVariants = dataContext.GcproductVariants.Where(gcpv => gcpv.ProductId == crossPlatformProductListing.GcproductId).ToList();

                foreach (Models.GcproductVariant variant in gcVariants)
                {
                    dataContext.MctproductVariants.Where(mctpv => mctpv.GcvariantId == variant.Id).First().GcvariantId = null;
                    variant.MctvariantId = null;

                    if (crossPlatformProductListing.EtsyListingId != null)
                    {
                        dataContext.EtsyListingProducts.Where(elp => elp.GcvariantId == variant.Id).First().GcvariantId = null;
                        variant.EtsyProductId = null;
                    }
                }

                crossPlatformProductListing.GcproductId = null;
                crossPlatformProductListing.LastUpdated = DateTime.Now;

                dataContext.SaveChanges();
            }

            gcMatched = false;

            listBoxGCProducts.Items.Clear();
            _SelectedProduct.GetProductMatches(ShopifyBrand.MCT, _GCProducts).ForEach(delegate (ShopifyProduct product)
            {
                _MatchedShopifyProductIds.Add(product.ID);
                listBoxGCProducts.Items.Add(product.Title);
            });
            pictureBoxGC.Image = null;
            listViewGCVariants.Items.Clear();

            EnableControls();
        }

        private void buttonClearGC_Click(object sender, EventArgs e)
        {
            listBoxGCProducts.ClearSelected();
            listViewGCVariants.Items.Clear();
        }

        private void listBoxGCProducts_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisableControls();

            if (listBoxGCProducts.SelectedItems.Count > 0)
            {
                _CorrespondingProduct = _GCProducts.First(product => product.ID == _MatchedShopifyProductIds[listBoxGCProducts.SelectedIndex]);

                labelGCStatus.Text = _CorrespondingProduct.Status[0].ToString().ToUpper() + _CorrespondingProduct.Status[1..];

                pictureBoxGC.LoadAsync(_CorrespondingProduct.Images[0].URL);

                listViewGCVariants.Items.Clear();
                foreach (ShopifyProductVariant variant in _CorrespondingProduct.Variants)
                {
                    listViewGCVariants.Items.Add(new ListViewItem(new[]
                    {
                        variant.Size,
                        variant.Colour,
                        "£" + decimal.Parse(variant.Price).ToString("0.00"),
                        "£" + variant.CompareAtPrice ?? "0.00",
                        variant.Quantity.ToString(),
                        variant.InventoryPolicy }));
                }
            }
            else
            {
                pictureBoxGC.Image = null;
                labelGCStatus.Text = String.Empty;
            }

            EnableControls();
        }

        private void linkLabelEditEtsy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            int index = listBoxEtsyListings.SelectedIndex;

            using (FormEditCopy form = new FormEditCopy(ref _CorrespondingListing))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    listViewEtsyProducts.Items.Clear();
                    listBoxEtsyListings.Items[index] = _CorrespondingListing.Title;
                    typeof(System.Windows.Forms.ListBox).InvokeMember("RefreshItems", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod, null, listBoxEtsyListings, new object[] { });
                    listBoxEtsyListings.SelectedIndex = index;
                }
            }
        }

        private void buttonMatchEtsy_Click(object sender, EventArgs e)
        {
            DisableControls();

            using (Models.CrookedPortalContext dataContext = new Models.CrookedPortalContext())
            {
                Models.CrossPlatformProductListing crossPlatformProductListing;

                if (dataContext.CrossPlatformProductListings.Any(cppl => cppl.MctproductId == _SelectedProduct.ID))
                {
                    UpdateEtsy();

                    UpdateEtsyData();

                    crossPlatformProductListing = dataContext.CrossPlatformProductListings.First(cppl => cppl.MctproductId == _SelectedProduct.ID);
                    crossPlatformProductListing.EtsyListingId = _CorrespondingListing.ID;

                    if (!dataContext.EtsyListings.Any(listing => listing.Id == _CorrespondingListing.ID))
                    {
                        dataContext.Add(new Models.EtsyListing()
                        {
                            Id = _CorrespondingListing.ID,
                            Title = _CorrespondingListing.Title,
                            Description = _CorrespondingListing.Description,
                            State = _CorrespondingListing.State,
                            LastUpdated = DateTime.Now
                        });

                        List<EtsyListingProduct> listingProducts = JsonSerializer.Deserialize<EtsyListingProductsJSONRoot>(Utilities.GetJSONFromEtsy(Properties.Resources.EtsyListingProductsURL + _CorrespondingListing.ID + "/inventory")).Products;

                        foreach (EtsyListingProduct product in listingProducts)
                        {
                            dataContext.Add(new Models.EtsyListingProduct()
                            {
                                Id = product.ID,
                                ListingId = _CorrespondingListing.ID,
                                Price = product.Offerings[0].Price.Value,
                                Quantity = product.Offerings[0].Quantity,
                                Size = product.PropertyValues.Count > 0 ? product.PropertyValues[0].Values[0] : null,
                                Colour = product.PropertyValues.Count > 1 ? product.PropertyValues[1].Values[0] : null
                            });
                        }

                        List<EtsyListingImage> images = JsonSerializer.Deserialize<EtsyListingImagesJSONRoot>(Utilities.GetJSONFromEtsy(Properties.Resources.EtsyListingImagesURL + _CorrespondingListing.ID + "/images")).Images;

                        foreach (EtsyListingImage image in images)
                        {
                            dataContext.Add(new Models.EtsyListingImage()
                            {
                                ImageId = image.ID,
                                ListingId = _CorrespondingListing.ID,
                                Rank = image.Rank,
                                Url = image.URL
                            });
                        }
                    }

                    crossPlatformProductListing.LastUpdated = DateTime.Now;
                }
                else
                {
                    crossPlatformProductListing = new Models.CrossPlatformProductListing
                    {
                        MctproductId = _SelectedProduct.ID,
                        EtsyListingId = _CorrespondingListing.ID
                    };

                    if (!dataContext.Mctproducts.Any(product => product.Id == _SelectedProduct.ID))
                    {
                        dataContext.Add(new Models.Mctproduct()
                        {
                            Id = _SelectedProduct.ID,
                            Title = _SelectedProduct.Title,
                            Description = _SelectedProduct.Description,
                            Tags = _SelectedProduct.Tags,
                            Status = _SelectedProduct.Status,
                            LastUpdated = DateTime.Now,
                            PublishedScope = _SelectedProduct.PublishedScope
                        });

                        foreach (ShopifyProductVariant variant in _SelectedProduct.Variants)
                        {
                            dataContext.Add(new Models.MctproductVariant()
                            {
                                Id = variant.ID,
                                ProductId = _SelectedProduct.ID,
                                InventoryItemId = variant.InventoryItemID,
                                InventoryPolicy = variant.InventoryPolicy,
                                Price = decimal.Parse(variant.Price),
                                CompareAtPrice = decimal.TryParse(variant.CompareAtPrice, out decimal compareAtPrice) ? decimal.Parse(variant.CompareAtPrice) : 0.00m,
                                Quantity = variant.Quantity,
                                Size = variant.Size,
                                Colour = variant.Colour
                            });
                        }

                        foreach (ShopifyProductImage image in _SelectedProduct.Images)
                        {
                            dataContext.Add(new Models.MctproductImage()
                            {
                                Id = image.ID,
                                ProductId = _SelectedProduct.ID,
                                Position = image.Position,
                                Url = image.URL
                            });
                        }
                    }

                    dataContext.SaveChanges();

                    UpdateEtsy();

                    if (!dataContext.EtsyListings.Any(listing => listing.Id == _CorrespondingListing.ID))
                    {
                        dataContext.Add(new Models.EtsyListing()
                        {
                            Id = _CorrespondingListing.ID,
                            Title = _CorrespondingListing.Title,
                            Description = _CorrespondingListing.Description,
                            State = _CorrespondingListing.State,
                            LastUpdated = DateTime.Now
                        });

                        List<EtsyListingProduct> listingProducts = JsonSerializer.Deserialize<EtsyListingProductsJSONRoot>(Utilities.GetJSONFromEtsy(Properties.Resources.EtsyListingProductsURL + _CorrespondingListing.ID + "/inventory")).Products;

                        foreach (EtsyListingProduct product in listingProducts)
                        {
                            dataContext.Add(new Models.EtsyListingProduct()
                            {
                                Id = product.ID,
                                ListingId = _CorrespondingListing.ID,
                                Price = product.Offerings[0].Price.Value,
                                Quantity = product.Offerings[0].Quantity,
                                Size = product.PropertyValues.Count > 0 ? product.PropertyValues[0].Values[0] : null,
                                Colour = product.PropertyValues.Count > 1 ? product.PropertyValues[1].Values[0] : null
                            });
                        }

                        List<EtsyListingImage> images = JsonSerializer.Deserialize<EtsyListingImagesJSONRoot>(Utilities.GetJSONFromEtsy(Properties.Resources.EtsyListingImagesURL + _CorrespondingListing.ID + "/images")).Images;

                        foreach (EtsyListingImage image in images)
                        {
                            dataContext.Add(new Models.EtsyListingImage()
                            {
                                ImageId = image.ID,
                                ListingId = _CorrespondingListing.ID,
                                Rank = image.Rank,
                                Url = image.URL
                            });
                        }
                    }

                    crossPlatformProductListing.LastUpdated = DateTime.Now;

                    dataContext.Add(crossPlatformProductListing);

                    UpdateEtsyData();
                }

                List<Models.MctproductVariant> mctProductVariants = dataContext.MctproductVariants.Where(mctpv => mctpv.ProductId == crossPlatformProductListing.MctproductId).ToList(); ;
                List<Models.EtsyListingProduct> etsyListingProducts = dataContext.EtsyListingProducts.Where(elp => elp.ListingId == crossPlatformProductListing.EtsyListingId).ToList();

                for (int i = 0; i < mctProductVariants.Count; i++)
                {
                    mctProductVariants.First(mctpv => mctpv.Id == mctProductVariants[i].Id).EtsyProductId = etsyListingProducts.First(elp => elp.Size == mctProductVariants[i].Size && elp.Colour == mctProductVariants[i].Colour).Id;
                    etsyListingProducts.First(elp => elp.Id == etsyListingProducts[i].Id).MctvariantId = mctProductVariants.First(mctpv => mctpv.Size == etsyListingProducts[i].Size && mctpv.Colour == etsyListingProducts[i].Colour).Id;

                    if (crossPlatformProductListing.GcproductId != null)
                    {
                        List<Models.GcproductVariant> gcProductVariants = dataContext.GcproductVariants.Where(gcpv => gcpv.ProductId == crossPlatformProductListing.GcproductId).ToList();

                        mctProductVariants.First(mctpv => mctpv.Id == mctProductVariants[i].Id).GcvariantId = gcProductVariants.First(gcpv => gcpv.Size == mctProductVariants[i].Size && gcpv.Colour == mctProductVariants[i].Colour).Id;
                        gcProductVariants.First(gcpv => gcpv.Id == gcProductVariants[i].Id).MctvariantId = mctProductVariants.First(mctpv => mctpv.Size == gcProductVariants[i].Size && mctpv.Colour == gcProductVariants[i].Colour).Id;
                        gcProductVariants.First(gcpv => gcpv.Id == gcProductVariants[i].Id).EtsyProductId = etsyListingProducts.First(elp => elp.Size == gcProductVariants[i].Size && elp.Colour == gcProductVariants[i].Colour).Id;
                        etsyListingProducts.First(elp => elp.Id == etsyListingProducts[i].Id).GcvariantId = gcProductVariants.First(gcpv => gcpv.Size == etsyListingProducts[i].Size && gcpv.Colour == etsyListingProducts[i].Colour).Id;
                    }
                }

                dataContext.SaveChanges();
            }

            etsyMatched = true;
            listBoxEtsyListings.Items.Clear();
            listBoxEtsyListings.Items.Add(_CorrespondingListing.Title);
            listBoxEtsyListings.SetSelected(0, true);

            EnableControls();
        }

        private void buttonUnmatchEtsy_Click(object sender, EventArgs e)
        {
            DisableControls();

            using (Models.CrookedPortalContext dataContext = new Models.CrookedPortalContext())
            {
                Models.CrossPlatformProductListing crossPlatformProductListing = dataContext.CrossPlatformProductListings.First(cppl => cppl.EtsyListingId == _CorrespondingListing.ID);

                List<Models.EtsyListingProduct> etsyProducts = dataContext.EtsyListingProducts.Where(elp => elp.ListingId == crossPlatformProductListing.EtsyListingId).ToList();

                foreach (Models.EtsyListingProduct product in etsyProducts)
                {
                    dataContext.MctproductVariants.Where(mctpv => mctpv.EtsyProductId == product.Id).First().EtsyProductId = null;
                    product.MctvariantId = null;

                    if (crossPlatformProductListing.GcproductId != null)
                    {
                        dataContext.GcproductVariants.Where(gcpv => gcpv.EtsyProductId == product.Id).First().EtsyProductId = null;
                        product.GcvariantId = null;
                    }
                }

                crossPlatformProductListing.EtsyListingId = null;
                crossPlatformProductListing.LastUpdated = DateTime.Now;

                dataContext.SaveChanges();
            }

            etsyMatched = false;

            listBoxEtsyListings.Items.Clear();
            _SelectedProduct.GetListingMatches(ShopifyBrand.MCT, _EtsyListings).ForEach(delegate (EtsyListing listing)
            {
                _MatchedEtsyListingIds.Add(listing.ID);
                listBoxEtsyListings.Items.Add(listing.Title);
            });
            pictureBoxEtsy.Image = null;
            listViewEtsyProducts.Items.Clear();

            EnableControls();
        }

        private void buttonClearEtsy_Click(object sender, EventArgs e)
        {
            listBoxEtsyListings.ClearSelected();
            listViewEtsyProducts.Items.Clear();
        }

        private void listBoxEtsyListings_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisableControls();

            if (listBoxEtsyListings.SelectedItems.Count > 0)
            {
                _CorrespondingListing = _EtsyListings.First(listing => listing.ID == _MatchedEtsyListingIds[listBoxEtsyListings.SelectedIndex]);

                labelEtsyState.Text = _CorrespondingListing.State == "edit" ? "Inactive" : _CorrespondingListing.State[0].ToString().ToUpper() + _CorrespondingListing.State[1..];

                EtsyListingImage image = JsonSerializer.Deserialize<EtsyListingImagesJSONRoot>(Utilities.GetJSONFromEtsy(Properties.Resources.EtsyListingImagesURL + _CorrespondingListing.ID + "/images")).Images.OrderBy(i => i.Rank).First();

                pictureBoxEtsy.LoadAsync(image.URL);

                listViewEtsyProducts.Items.Clear();
                foreach (EtsyListingProduct product in JsonSerializer.Deserialize<EtsyListingProductsJSONRoot>(Utilities.GetJSONFromEtsy(Properties.Resources.EtsyListingProductsURL + _CorrespondingListing.ID + "/inventory")).Products)
                {
                    listViewEtsyProducts.Items.Add(new ListViewItem(new string[]
                    {
                        product.PropertyValues.Count > 0 ? product.PropertyValues[0].Values[0] : "",
                        product.PropertyValues.Count > 1 ? product.PropertyValues[1].Values[0] : "",
                        "£" + product.Offerings[0].Price.Value.ToString("0.00"),
                        product.Offerings.Count > 0 ? product.Offerings[0].Quantity.ToString() : ""
                    }));
                }
            }
            else
            {
                labelEtsyState.Text = String.Empty;
                pictureBoxEtsy.Image = null;
            }

            EnableControls();
        }

        private void buttonMatchBoth_Click(object sender, EventArgs e)
        {
            DisableControls();

            UpdateGC();
            UpdateEtsy();

            using (Models.CrookedPortalContext dataContext = new Models.CrookedPortalContext())
            {
                Models.CrossPlatformProductListing crossPlatformProductListing;

                if (dataContext.CrossPlatformProductListings.Any(cppl => cppl.MctproductId == _SelectedProduct.ID))
                {
                    crossPlatformProductListing = dataContext.CrossPlatformProductListings.First(cppl => cppl.MctproductId == _SelectedProduct.ID);
                    crossPlatformProductListing.GcproductId = _CorrespondingProduct.ID;
                    crossPlatformProductListing.EtsyListingId = _CorrespondingListing.ID;

                    if (!dataContext.Gcproducts.Any(product => product.Id == _CorrespondingProduct.ID))
                    {
                        dataContext.Add(new Models.Gcproduct()
                        {
                            Id = _CorrespondingProduct.ID,
                            Title = _CorrespondingProduct.Title,
                            Description = _CorrespondingProduct.Description,
                            Tags = _CorrespondingProduct.Tags,
                            Status = _CorrespondingProduct.Status,
                            LastUpdated = DateTime.Now,
                            PublishedScope = _CorrespondingProduct.PublishedScope
                        });

                        foreach (ShopifyProductVariant variant in _CorrespondingProduct.Variants)
                        {
                            dataContext.Add(new Models.GcproductVariant()
                            {
                                Id = variant.ID,
                                ProductId = _CorrespondingProduct.ID,
                                InventoryItemId = variant.InventoryItemID,
                                InventoryPolicy = variant.InventoryPolicy,
                                Price = decimal.Parse(variant.Price),
                                CompareAtPrice = decimal.TryParse(variant.CompareAtPrice, out decimal compareAtPrice) ? decimal.Parse(variant.CompareAtPrice) : 0.00m,
                                Quantity = variant.Quantity,
                                Size = variant.Size,
                                Colour = variant.Colour
                            });
                        }

                        foreach (ShopifyProductImage image in _CorrespondingProduct.Images)
                        {
                            dataContext.Add(new Models.GcproductImage()
                            {
                                Id = image.ID,
                                ProductId = _CorrespondingProduct.ID,
                                Position = image.Position,
                                Url = image.URL
                            });
                        }
                    }

                    if (!dataContext.EtsyListings.Any(listing => listing.Id == _CorrespondingListing.ID))
                    {
                        dataContext.Add(new Models.EtsyListing()
                        {
                            Id = _CorrespondingListing.ID,
                            Title = _CorrespondingListing.Title,
                            Description = _CorrespondingListing.Description,
                            State = _CorrespondingListing.State,
                            LastUpdated = DateTime.Now
                        });

                        List<EtsyListingProduct> listingProducts = JsonSerializer.Deserialize<EtsyListingProductsJSONRoot>(Utilities.GetJSONFromEtsy(Properties.Resources.EtsyListingProductsURL + _CorrespondingListing.ID + "/inventory")).Products;

                        foreach (EtsyListingProduct product in listingProducts)
                        {
                            dataContext.Add(new Models.EtsyListingProduct()
                            {
                                Id = product.ID,
                                ListingId = _CorrespondingListing.ID,
                                Price = product.Offerings[0].Price.Value,
                                Quantity = product.Offerings[0].Quantity,
                                Size = product.PropertyValues.Count > 0 ? product.PropertyValues[0].Values[0] : null,
                                Colour = product.PropertyValues.Count > 1 ? product.PropertyValues[1].Values[0] : null
                            });
                        }

                        List<EtsyListingImage> images = JsonSerializer.Deserialize<EtsyListingImagesJSONRoot>(Utilities.GetJSONFromEtsy(Properties.Resources.EtsyListingImagesURL + _CorrespondingListing.ID + "/images")).Images;

                        foreach (EtsyListingImage image in images)
                        {
                            dataContext.Add(new Models.EtsyListingImage()
                            {
                                ImageId = image.ID,
                                ListingId = _CorrespondingListing.ID,
                                Rank = image.Rank,
                                Url = image.URL
                            });
                        }
                    }

                    crossPlatformProductListing.LastUpdated = DateTime.Now;
                }
                else
                {
                    crossPlatformProductListing = new Models.CrossPlatformProductListing
                    {
                        MctproductId = _SelectedProduct.ID,
                        GcproductId = _CorrespondingProduct.ID,
                        EtsyListingId = _CorrespondingListing.ID
                    };

                    if (!dataContext.Mctproducts.Any(product => product.Id == _SelectedProduct.ID))
                    {
                        dataContext.Add(new Models.Mctproduct()
                        {
                            Id = _SelectedProduct.ID,
                            Title = _SelectedProduct.Title,
                            Description = _SelectedProduct.Description,
                            Tags = _SelectedProduct.Tags,
                            Status = _SelectedProduct.Status,
                            LastUpdated = DateTime.Now,
                            PublishedScope = _SelectedProduct.PublishedScope
                        });

                        foreach (ShopifyProductVariant variant in _SelectedProduct.Variants)
                        {
                            dataContext.Add(new Models.MctproductVariant()
                            {
                                Id = variant.ID,
                                ProductId = _SelectedProduct.ID,
                                InventoryItemId = variant.InventoryItemID,
                                InventoryPolicy = variant.InventoryPolicy,
                                Price = decimal.Parse(variant.Price),
                                CompareAtPrice = decimal.TryParse(variant.CompareAtPrice, out decimal compareAtPrice) ? decimal.Parse(variant.CompareAtPrice) : 0.00m,
                                Quantity = variant.Quantity,
                                Size = variant.Size,
                                Colour = variant.Colour
                            });
                        }

                        foreach (ShopifyProductImage image in _SelectedProduct.Images)
                        {
                            dataContext.Add(new Models.MctproductImage()
                            {
                                Id = image.ID,
                                ProductId = _SelectedProduct.ID,
                                Position = image.Position,
                                Url = image.URL
                            });
                        }
                    }

                    if (!dataContext.Gcproducts.Any(product => product.Id == _CorrespondingProduct.ID))
                    {
                        dataContext.Add(new Models.Gcproduct()
                        {
                            Id = _CorrespondingProduct.ID,
                            Title = _CorrespondingProduct.Title,
                            Description = _CorrespondingProduct.Description,
                            Tags = _CorrespondingProduct.Tags,
                            Status = _CorrespondingProduct.Status,
                            LastUpdated = DateTime.Now,
                            PublishedScope = _CorrespondingProduct.PublishedScope
                        });

                        foreach (ShopifyProductVariant variant in _CorrespondingProduct.Variants)
                        {
                            dataContext.Add(new Models.GcproductVariant()
                            {
                                Id = variant.ID,
                                ProductId = _CorrespondingProduct.ID,
                                InventoryItemId = variant.InventoryItemID,
                                InventoryPolicy = variant.InventoryPolicy,
                                Price = decimal.Parse(variant.Price),
                                CompareAtPrice = decimal.TryParse(variant.CompareAtPrice, out decimal compareAtPrice) ? decimal.Parse(variant.CompareAtPrice) : 0.00m,
                                Quantity = variant.Quantity,
                                Size = variant.Size,
                                Colour = variant.Colour
                            });
                        }

                        foreach (ShopifyProductImage image in _CorrespondingProduct.Images)
                        {
                            dataContext.Add(new Models.GcproductImage()
                            {
                                Id = image.ID,
                                ProductId = _CorrespondingProduct.ID,
                                Position = image.Position,
                                Url = image.URL
                            });
                        }
                    }

                    if (!dataContext.EtsyListings.Any(listing => listing.Id == _CorrespondingListing.ID))
                    {
                        dataContext.Add(new Models.EtsyListing()
                        {
                            Id = _CorrespondingListing.ID,
                            Title = _CorrespondingListing.Title,
                            Description = _CorrespondingListing.Description,
                            State = _CorrespondingListing.State,
                            LastUpdated = DateTime.Now
                        });

                        List<EtsyListingProduct> listingProducts = JsonSerializer.Deserialize<EtsyListingProductsJSONRoot>(Utilities.GetJSONFromEtsy(Properties.Resources.EtsyListingProductsURL + _CorrespondingListing.ID + "/inventory")).Products;

                        foreach (EtsyListingProduct product in listingProducts)
                        {
                            dataContext.Add(new Models.EtsyListingProduct()
                            {
                                Id = product.ID,
                                ListingId = _CorrespondingListing.ID,
                                Price = product.Offerings[0].Price.Value,
                                Quantity = product.Offerings[0].Quantity,
                                Size = product.PropertyValues.Count > 0 ? product.PropertyValues[0].Values[0] : null,
                                Colour = product.PropertyValues.Count > 1 ? product.PropertyValues[1].Values[0] : null
                            });
                        }

                        List<EtsyListingImage> images = JsonSerializer.Deserialize<EtsyListingImagesJSONRoot>(Utilities.GetJSONFromEtsy(Properties.Resources.EtsyListingImagesURL + _CorrespondingListing.ID + "/images")).Images;

                        foreach (EtsyListingImage image in images)
                        {
                            dataContext.Add(new Models.EtsyListingImage()
                            {
                                ImageId = image.ID,
                                ListingId = _CorrespondingListing.ID,
                                Rank = image.Rank,
                                Url = image.URL
                            });
                        }
                    }

                    crossPlatformProductListing.LastUpdated = DateTime.Now;

                    dataContext.Add(crossPlatformProductListing);
                }

                List<Models.MctproductVariant> mctProductVariants = dataContext.MctproductVariants.Where(mctpv => mctpv.ProductId == crossPlatformProductListing.MctproductId).ToList(); ;
                List<Models.GcproductVariant> gcProductVariants = dataContext.GcproductVariants.Where(gcpv => gcpv.ProductId == crossPlatformProductListing.GcproductId).ToList();
                List<Models.EtsyListingProduct> etsyListingProducts = dataContext.EtsyListingProducts.Where(elp => elp.ListingId == crossPlatformProductListing.EtsyListingId).ToList();

                for (int i = 0; i < mctProductVariants.Count; i++)
                {
                    mctProductVariants.First(mctpv => mctpv.Id == mctProductVariants[i].Id).GcvariantId = gcProductVariants.First(gcpv => gcpv.Size == mctProductVariants[i].Size && gcpv.Colour == mctProductVariants[i].Colour).Id;
                    mctProductVariants.First(mctpv => mctpv.Id == mctProductVariants[i].Id).EtsyProductId = etsyListingProducts.First(elp => elp.Size == mctProductVariants[i].Size && elp.Colour == mctProductVariants[i].Colour).Id;
                    gcProductVariants.First(gcpv => gcpv.Id == gcProductVariants[i].Id).MctvariantId = mctProductVariants.First(mctpv => mctpv.Size == gcProductVariants[i].Size && mctpv.Colour == gcProductVariants[i].Colour).Id;
                    gcProductVariants.First(gcpv => gcpv.Id == gcProductVariants[i].Id).EtsyProductId = etsyListingProducts.First(elp => elp.Size == gcProductVariants[i].Size && elp.Colour == gcProductVariants[i].Colour).Id;
                    etsyListingProducts.First(elp => elp.Id == etsyListingProducts[i].Id).MctvariantId = mctProductVariants.First(mctpv => mctpv.Size == etsyListingProducts[i].Size && mctpv.Colour == etsyListingProducts[i].Colour).Id;
                    etsyListingProducts.First(elp => elp.Id == etsyListingProducts[i].Id).GcvariantId = gcProductVariants.First(gcpv => gcpv.Size == etsyListingProducts[i].Size && gcpv.Colour == etsyListingProducts[i].Colour).Id;
                }

                dataContext.SaveChanges();
            }

            gcMatched = true;
            listBoxGCProducts.Items.Clear();
            listBoxGCProducts.Items.Add(_CorrespondingProduct.Title);
            listBoxGCProducts.SetSelected(0, true);

            etsyMatched = true;
            listBoxEtsyListings.Items.Clear();
            listBoxEtsyListings.Items.Add(_CorrespondingListing.Title);
            listBoxEtsyListings.SetSelected(0, true);

            EnableControls();
        }

        private void linkLabelEditGCDetails_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (FormEditDetails form = new FormEditDetails(ShopifyBrand.GC, ref _CorrespondingProduct, ref _SelectedProduct, ref _CorrespondingListing))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    labelGCStatus.Text = _CorrespondingProduct.Status[0].ToString().ToUpper() + _CorrespondingProduct.Status[1..];
                    listViewGCVariants.Items.Clear();
                    foreach (ShopifyProductVariant variant in _CorrespondingProduct.Variants)
                    {
                        listViewGCVariants.Items.Add(new ListViewItem(new string[]
                        {
                            variant.Size,
                            variant.Colour,
                            "£" + variant.Price,
                            variant.CompareAtPrice ?? "0.00",
                            variant.Quantity.ToString(),
                            variant.InventoryPolicy
                        }));
                    }

                    if (gcMatched)
                    {
                        listViewMCTVariants.Items.Clear();
                        foreach (ShopifyProductVariant variant in _SelectedProduct.Variants)
                        {
                            listViewMCTVariants.Items.Add(new ListViewItem(new[]
                            {
                                variant.Size,
                                variant.Colour,
                                "£" + decimal.Parse(variant.Price).ToString("0.00"),
                                "£" + variant.CompareAtPrice ?? "0.00",
                                variant.Quantity.ToString(),
                                variant.InventoryPolicy
                            }));
                        }
                    }

                    if (etsyMatched)
                    {
                        UpdateEtsyData();

                        labelEtsyState.Text = _CorrespondingListing.State == "edit" ? "Inactive" : _CorrespondingListing.State[0].ToString().ToUpper() + _CorrespondingListing.State[1..];
                        listViewEtsyProducts.Items.Clear();
                        foreach (EtsyListingProduct product in JsonSerializer.Deserialize<EtsyListingProductsJSONRoot>(Utilities.GetJSONFromEtsy(Properties.Resources.EtsyListingProductsURL + _CorrespondingListing.ID + "/inventory")).Products)
                        {
                            listViewEtsyProducts.Items.Add(new ListViewItem(new string[]
                            {
                                product.PropertyValues.Count > 0 ? product.PropertyValues[0].Values[0] : "",
                                product.PropertyValues.Count > 1 ? product.PropertyValues[1].Values[0] : "",
                                "£" + product.Offerings[0].Price.Value.ToString("0.00"),
                                product.Offerings.Count > 0 ? product.Offerings[0].Quantity.ToString() : ""
                            }));
                        }
                    }
                }
            }
        }

        private void linkLabelEditEtsyDetails_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (FormEditDetails form = new FormEditDetails(ref _CorrespondingListing, ref _SelectedProduct, ref _CorrespondingProduct))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    UpdateEtsyData();

                    labelEtsyState.Text = _CorrespondingListing.State == "edit" ? "Inactive" : _CorrespondingListing.State[0].ToString().ToUpper() + _CorrespondingListing.State[1..];
                    listViewEtsyProducts.Items.Clear();
                    foreach (EtsyListingProduct product in JsonSerializer.Deserialize<EtsyListingProductsJSONRoot>(Utilities.GetJSONFromEtsy(Properties.Resources.EtsyListingProductsURL + _CorrespondingListing.ID + "/inventory")).Products)
                    {
                        listViewEtsyProducts.Items.Add(new ListViewItem(new string[]
                        {
                            product.PropertyValues.Count > 0 ? product.PropertyValues[0].Values[0] : "",
                            product.PropertyValues.Count > 1 ? product.PropertyValues[1].Values[0] : "",
                            "£" + product.Offerings[0].Price.Value.ToString("0.00"),
                            product.Offerings.Count > 0 ? product.Offerings[0].Quantity.ToString() : ""
                        }));
                    }

                    if (etsyMatched)
                    {
                        listViewMCTVariants.Items.Clear();
                        foreach (ShopifyProductVariant variant in _SelectedProduct.Variants)
                        {
                            listViewMCTVariants.Items.Add(new ListViewItem(new[]
                            {
                                variant.Size,
                                variant.Colour,
                                "£" + decimal.Parse(variant.Price).ToString("0.00"),
                                "£" + variant.CompareAtPrice ?? "0.00",
                                variant.Quantity.ToString(),
                                variant.InventoryPolicy
                            }));
                        }
                    }

                    if (gcMatched)
                    {
                        listViewGCVariants.Items.Clear();
                        foreach (ShopifyProductVariant variant in _CorrespondingProduct.Variants)
                        {
                            listViewGCVariants.Items.Add(new ListViewItem(new[]
                            {
                                variant.Size,
                                variant.Colour,
                                "£" + decimal.Parse(variant.Price).ToString("0.00"),
                                "£" + variant.CompareAtPrice ?? "0.00",
                                variant.Quantity.ToString(),
                                variant.InventoryPolicy
                            }));
                        }
                    }
                }
            }
        }

        private void buttonAddData_Click(object sender, EventArgs e)
        {
            using (FormAddData form = new FormAddData())
            {
                form.Show();
                form.AddData(_NewMCTProducts, _NewGCProducts, _NewEtsyListings);
                form.Focus();
            }

            labelNewMCT.Text = "0 new MCT products to add";
            labelNewGC.Text = "0 new GC products to add";
            labelNewEtsy.Text = "0 new Etsy listings to add";

            buttonAddData.Enabled = false;
        }

        private void GenerateExcel(object sender, EventArgs e)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Properties.Resources.MCTAPIURL + "orders.json?status=any&limit=250&" + 
                                      "created_at_min=" + DateTime.Parse("02/02/2022").ToString("s", System.Globalization.CultureInfo.CurrentCulture));

            request.ContentType = "application/json";
            request.Credentials = new NetworkCredential(Properties.Resources.MCTAPIKey, Properties.Resources.MCTPassword);
            request.Method = "GET";

            WebResponse response = request.GetResponse();

            List<ShopifyOrder> orders = null;

            using (System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream()))
            {
                orders = JsonSerializer.Deserialize<ShopifyOrderRoot>(reader.ReadToEnd()).Orders;
            }

            List<long> itemIDs = new List<long>();

            foreach (ShopifyOrder order in orders)
            {
                foreach(ShopifyOrderItem item in order.OrderItems)
                {
                    itemIDs.Add(item.ProductID);
                }
            }

            request = (HttpWebRequest)WebRequest.Create(Properties.Resources.GCAPIURL + "orders.json?status=any&limit=250&" +
                                      "created_at_min=" + DateTime.Parse("02/02/2022").ToString("s", System.Globalization.CultureInfo.CurrentCulture));

            request.ContentType = "application/json";
            request.Credentials = new NetworkCredential(Properties.Resources.GCAPIKey, Properties.Resources.GCPassword);
            request.Method = "GET";

            response = request.GetResponse();

            using (System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream()))
            {
                orders = JsonSerializer.Deserialize<ShopifyOrderRoot>(reader.ReadToEnd()).Orders;
            }

            foreach (ShopifyOrder order in orders)
            {
                foreach (ShopifyOrderItem item in order.OrderItems)
                {
                    itemIDs.Add(item.ProductID);
                }
            }

            List<long> ids = new List<long>();

            using (Models.CrookedPortalContext dataContext = new Models.CrookedPortalContext())
            {
                List<long> pcs = dataContext.ProductChanges.Select(pc => pc.ProductId).Distinct().ToList();

                foreach (long pc in pcs)
                {
                    if (!itemIDs.Contains(pc))
                    {
                        ids.Add(pc);
                    }
                }
            }

            Excel.Application app = new Excel.Application();

            Excel._Workbook workbook = (Excel._Workbook)(app.Workbooks.Add(Missing.Value));
            Excel._Worksheet worksheet = (Excel._Worksheet)workbook.Worksheets.get_Item(1);
            StringBuilder variantsString;
            StringBuilder pricesString;
            int count = 0;

            for (int i = 0; i < _MCTProducts.Count; i++)
            {
                if (!ids.Contains(_MCTProducts[i].ID))
                {
                    count++;
                    worksheet.Cells[count, 1] = _MCTProducts[i].Title;
                }
                
                //if (_MCTProducts[i].Variants.Count > 1)
                //{
                //    if (_MCTProducts[i].Variants.TrueForAll(variant => variant.Price == _MCTProducts[i].Variants[0].Price))
                //    {
                //        worksheet.Cells[i + 1, 2] = "All Variants";
                //        worksheet.Cells[i + 1, 3] = "£" + _MCTProducts[i].Variants[0].Price;
                //    }
                //    else
                //    {
                //        variantsString = new StringBuilder();
                //        pricesString = new StringBuilder();

                //        for (int j = 0; j < _MCTProducts[i].Variants.Count; j++)
                //        {
                //            variantsString.Append(_MCTProducts[i].Variants[j].Size + " " + (_MCTProducts[i].Variants[j].Colour ?? _MCTProducts[i].Variants[0].Colour) + (j < _MCTProducts[i].Variants.Count - 1 ? "\n" : ""));
                //            pricesString.Append("£" + _MCTProducts[i].Variants[j].Price + (j < _MCTProducts[i].Variants.Count - 1 ? "\n" : ""));
                //        }
                //        worksheet.Cells[i + 1, 2] = variantsString.ToString();
                //        worksheet.Cells[i + 1, 3] = pricesString.ToString();
                //    }
                //}
                //else
                //{
                //    worksheet.Cells[i + 1, 2] = "One Variant";
                //    worksheet.Cells[i + 1, 3] = "£" + _MCTProducts[i].Variants[0].Price;
                //}
            }

            Excel.Range range = worksheet.Range["A1", "C" + count];

            range.Columns.EntireColumn.AutoFit();
            range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;

            workbook.SaveAs("C:\\Users\\Craig\\OneDrive\\Desktop\\MCT_products_to_update.xlsx");
            workbook.Close(false, Type.Missing, Type.Missing);

            app.Quit();




            app = new Excel.Application();

            workbook = (Excel._Workbook)(app.Workbooks.Add(Missing.Value));
            worksheet = (Excel._Worksheet)workbook.Worksheets.get_Item(1);
            count = 0;

            for (int i = 0; i < _GCProducts.Count; i++)
            {
                if (!ids.Contains(_GCProducts[i].ID))
                {
                    count++;
                    worksheet.Cells[count, 1] = _GCProducts[i].Title;
                }

                //if (_GCProducts[i].Variants.Count > 1)
                //{
                //    if (_GCProducts[i].Variants.TrueForAll(variant => variant.Price == _GCProducts[i].Variants[0].Price))
                //    {
                //        worksheet.Cells[i + 1 + _MCTProducts.Count, 2] = "All Variants";
                //        worksheet.Cells[i + 1 + _MCTProducts.Count, 3] = "£" + _GCProducts[i].Variants[0].Price;
                //    }
                //    else
                //    {
                //        variantsString = new StringBuilder();
                //        pricesString = new StringBuilder();

                //        for (int j = 0; j < _GCProducts[i].Variants.Count; j++)
                //        {
                //            variantsString.Append(_GCProducts[i].Variants[j].Size + " " + (_GCProducts[i].Variants[j].Colour ?? _GCProducts[i].Variants[0].Colour) + (j < _GCProducts[i].Variants.Count - 1 ? "\n" : ""));
                //            pricesString.Append("£" + _GCProducts[i].Variants[j].Price + (j < _GCProducts[i].Variants.Count - 1 ? "\n" : ""));
                //        }
                //        worksheet.Cells[i + 1 + _MCTProducts.Count, 2] = variantsString.ToString();
                //        worksheet.Cells[i + 1 + _MCTProducts.Count, 3] = pricesString.ToString();
                //    }
                //}
                //else
                //{
                //    worksheet.Cells[i + 1 + _MCTProducts.Count, 2] = "One Variant";
                //    worksheet.Cells[i + 1 + _MCTProducts.Count, 3] = "£" + _GCProducts[i].Variants[0].Price;
                //}
            }

            range = worksheet.Range["A1", "C" + count];

            range.Columns.EntireColumn.AutoFit();
            range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;

            workbook.SaveAs("C:\\Users\\Craig\\OneDrive\\Desktop\\GC_products_to_update.xlsx");
            workbook.Close(false, Type.Missing, Type.Missing);

            app.Quit();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Left)
            {
                if (buttonPrevious.Enabled)
                {
                    buttonPrevious_Click(this, new EventArgs());
                }
                return true;
            }

            if (keyData == Keys.Right)
            {
                if (buttonNext.Enabled)
                {
                    buttonNext_Click(this, new EventArgs());
                }
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void UpdateEtsyStockLevels()
        {
            using (Models.CrookedPortalContext dataContext = new Models.CrookedPortalContext())
            {
                int i = 1;
                Models.CrossPlatformProductListing crossPlatformProductListing = null;
                List<long> ids = new List<long>();

                foreach (long id in dataContext.ProductChanges.Select(pc => pc.ProductId).Distinct())
                {
                    ids.Add(id);
                }

                foreach (long id in ids)
                {
                    labelNumber.Text = String.Format("CPPL {0} of {1}", i, ids.Count);
                    this.Refresh();

                    try
                    {
                        if (dataContext.CrossPlatformProductListings.Any(cppl => cppl.MctproductId == id))
                        {
                            crossPlatformProductListing = dataContext.CrossPlatformProductListings.First(cppl => cppl.MctproductId == id);
                            brand = ShopifyBrand.MCT;

                            _SelectedProduct = _MCTProducts.First(mctp => mctp.ID == crossPlatformProductListing.MctproductId);
                            _CorrespondingListing = _EtsyListings.First(el => el.ID == crossPlatformProductListing.EtsyListingId);

                            UpdateEtsy();
                            UpdateEtsyData();

                            if (crossPlatformProductListing.GcproductId != null)
                            {
                                _CorrespondingProduct = _GCProducts.First(gcp => gcp.ID == crossPlatformProductListing.GcproductId);

                                UpdateGC();
                                UpdateGCData();
                            }
                        }
                        else if (dataContext.CrossPlatformProductListings.Any(cppl => cppl.GcproductId == id))
                        {
                            crossPlatformProductListing = dataContext.CrossPlatformProductListings.First(cppl => cppl.GcproductId == id);
                            brand = ShopifyBrand.GC;

                            _SelectedProduct = _GCProducts.First(gcp => gcp.ID == crossPlatformProductListing.GcproductId);
                            _CorrespondingListing = _EtsyListings.First(el => el.ID == crossPlatformProductListing.EtsyListingId);

                            UpdateEtsy();
                            UpdateEtsyData();

                            if (crossPlatformProductListing.MctproductId != null)
                            {
                                _CorrespondingProduct = _MCTProducts.First(mctp => mctp.ID == crossPlatformProductListing.MctproductId);

                                UpdateMCT();
                                UpdateMCTData();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        dataContext.ChangedProducts.Add(new Models.ChangedProduct() 
                        { 
                            ProductId = id,
                            Message = e.Message
                        });

                        dataContext.SaveChanges();
                    }

                    i++;
                }
            }
        }

        private void AlignCrossPlatformProductListings()
        {
            using (Models.CrookedPortalContext dataContext = new Models.CrookedPortalContext())
            {
                List<Models.MctproductVariant> mctCollection = dataContext.MctproductVariants.ToList();
                List<Models.GcproductVariant> gcCollection = dataContext.GcproductVariants.ToList();
                List<Models.EtsyListingProduct> etsyCollection = dataContext.EtsyListingProducts.ToList();

                List<Models.MctproductVariant> mctProductVariants;
                List<Models.GcproductVariant> gcProductVariants;
                List<Models.EtsyListingProduct> etsyListingProducts;

                foreach (Models.CrossPlatformProductListing crossPlatformProductListing in dataContext.CrossPlatformProductListings)
                {
                    mctProductVariants = mctCollection.Where(mctpv => mctpv.ProductId == crossPlatformProductListing.MctproductId).ToList();
                    gcProductVariants = gcCollection.Where(gcpv => gcpv.ProductId == crossPlatformProductListing.GcproductId).ToList(); ;
                    etsyListingProducts = etsyCollection.Where(elp => elp.ListingId == crossPlatformProductListing.EtsyListingId).ToList();

                    if (crossPlatformProductListing.MctproductId != null)
                    {
                        if (crossPlatformProductListing.GcproductId != null)
                        {
                            if (mctProductVariants.Count != gcProductVariants.Count)
                            {
                                throw new Exception("MCT & GC\n\nMCTProductVariant: " + mctProductVariants[0].Id.ToString());
                            }

                            for (int i = 0; i < mctProductVariants.Count; i++)
                            {
                                mctProductVariants.Where(mctpv => mctpv.Id == mctProductVariants[i].Id).First().GcvariantId = gcProductVariants.Where(gcpv => gcpv.Size == mctProductVariants[i].Size && gcpv.Colour == mctProductVariants[i].Colour).FirstOrDefault().Id;
                                gcProductVariants.Where(gcpv => gcpv.Id == gcProductVariants[i].Id).First().MctvariantId = mctProductVariants.Where(mctpv => mctpv.Size == gcProductVariants[i].Size && mctpv.Colour == gcProductVariants[i].Colour).FirstOrDefault().Id;
                            }

                            if (crossPlatformProductListing.EtsyListingId != null)
                            {
                                if (mctProductVariants.Count != etsyListingProducts.Count)
                                {
                                    throw new Exception("MCT & GC & ETSY\n\nMCTProductVariant: " + mctProductVariants[0].Id.ToString());
                                }

                                for (int i = 0; i < mctProductVariants.Count; i++)
                                {
                                    mctProductVariants.Where(mctpv => mctpv.Id == mctProductVariants[i].Id).First().EtsyProductId = etsyListingProducts.Where(elp => elp.Size == mctProductVariants[i].Size && elp.Colour == mctProductVariants[i].Colour).FirstOrDefault().Id;
                                    gcProductVariants.Where(gcpv => gcpv.Id == gcProductVariants[i].Id).First().EtsyProductId = etsyListingProducts.Where(elp => elp.Size == gcProductVariants[i].Size && elp.Colour == gcProductVariants[i].Colour).FirstOrDefault().Id;
                                    etsyListingProducts.Where(elp => elp.Id == etsyListingProducts[i].Id).First().MctvariantId = mctProductVariants.Where(mctpv => mctpv.Size == etsyListingProducts[i].Size && mctpv.Colour == etsyListingProducts[i].Colour).FirstOrDefault().Id;
                                    etsyListingProducts.Where(elp => elp.Id == etsyListingProducts[i].Id).First().GcvariantId = gcProductVariants.Where(gcpv => gcpv.Size == etsyListingProducts[i].Size && gcpv.Colour == etsyListingProducts[i].Colour).FirstOrDefault().Id;
                                }
                            }
                        }
                        else if (crossPlatformProductListing.EtsyListingId != null)
                        {
                            if (mctProductVariants.Count != etsyListingProducts.Count)
                            {
                                throw new Exception("MCT & ETSY\n\nMCTProductVariant: " + mctProductVariants[0].Id.ToString());
                            }

                            for (int i = 0; i < mctProductVariants.Count; i++)
                            {
                                mctProductVariants.Where(mctpv => mctpv.Id == mctProductVariants[i].Id).First().EtsyProductId = etsyListingProducts.Where(elp => elp.Size == mctProductVariants[i].Size && elp.Colour == mctProductVariants[i].Colour).FirstOrDefault().Id;
                                etsyListingProducts.Where(elp => elp.Id == etsyListingProducts[i].Id).First().MctvariantId = mctProductVariants.Where(mctpv => mctpv.Size == etsyListingProducts[i].Size && mctpv.Colour == etsyListingProducts[i].Colour).FirstOrDefault().Id;
                            }
                        }
                    }
                    else if (crossPlatformProductListing.GcproductId != null)
                    {
                        if (crossPlatformProductListing.EtsyListingId != null)
                        {
                            if (gcProductVariants.Count != etsyListingProducts.Count)
                            {
                                throw new Exception("GC & ETSY\n\nGCProductVariant: " + gcProductVariants[0].Id.ToString());
                            }

                            for (int i = 0; i < mctProductVariants.Count; i++)
                            {
                                gcProductVariants.Where(gcpv => gcpv.Id == gcProductVariants[i].Id).First().EtsyProductId = etsyListingProducts.Where(elp => elp.Size == gcProductVariants[i].Size && elp.Colour == gcProductVariants[i].Colour).FirstOrDefault().Id;
                                etsyListingProducts.Where(elp => elp.Id == etsyListingProducts[i].Id).First().GcvariantId = gcProductVariants.Where(gcpv => gcpv.Size == etsyListingProducts[i].Size && gcpv.Colour == etsyListingProducts[i].Colour).FirstOrDefault().Id;
                            }
                        }
                    }
                }

                dataContext.SaveChanges();
            }
        }

        private void UpdateEtsyPrices()
        {
            using (Models.CrookedPortalContext dataContext = new Models.CrookedPortalContext())
            {
                Models.CrossPlatformProductListing crossPlatformProductListing = null;
                EtsyListing etsyListing = null;

                for (int i = 0; i < _EtsyListings.Count; i++)
                {
                    labelNumber.Text = String.Format("CPPL {0} of {1}", i + 1, _EtsyListings.Count);
                    this.Refresh();

                    if (dataContext.CrossPlatformProductListings.Any(cppl => cppl.EtsyListingId == _EtsyListings[i].ID))
                    {
                        crossPlatformProductListing = dataContext.CrossPlatformProductListings.First(cppl => cppl.EtsyListingId == _EtsyListings[i].ID);
                        etsyListing = _EtsyListings[i];

                        if (crossPlatformProductListing.MctproductId != null)
                        {
                            ShopifyProduct mctProduct = _MCTProducts.First(mctp => mctp.ID == crossPlatformProductListing.MctproductId);

                            etsyListing.UpdateProducts(ref etsyListing, mctProduct.Variants, true);

                            List<Models.MctproductVariant> mctProductVariants = dataContext.MctproductVariants.Where(mctpv => mctpv.ProductId == mctProduct.ID).ToList();

                            foreach (Models.MctproductVariant variant in mctProductVariants)
                            {
                                dataContext.EtsyListingProducts.First(elp => elp.Size == variant.Size && elp.Colour == variant.Colour).Price = variant.Price;
                                dataContext.EtsyListingProducts.First(elp => elp.Size == variant.Size && elp.Colour == variant.Colour).Quantity = variant.Quantity;
                            }
                        }
                        else if (crossPlatformProductListing.GcproductId != null)
                        {
                            ShopifyProduct gcProduct = _GCProducts.First(gcp => gcp.ID == crossPlatformProductListing.GcproductId);

                            etsyListing.UpdateProducts(ref etsyListing, gcProduct.Variants, true);

                            List<Models.GcproductVariant> gcProductVariants = dataContext.GcproductVariants.Where(gcpv => gcpv.ProductId == gcProduct.ID).ToList();

                            foreach (Models.GcproductVariant variant in gcProductVariants)
                            {
                                dataContext.EtsyListingProducts.First(elp => elp.Size == variant.Size && elp.Colour == variant.Colour).Price = variant.Price;
                                dataContext.EtsyListingProducts.First(elp => elp.Size == variant.Size && elp.Colour == variant.Colour).Quantity = variant.Quantity;
                            }
                        }
                    }

                    dataContext.SaveChanges();
                }
            }
        }

        private void ResetCrossPlatformProductListingsDatabase()
        {
            using (Models.CrookedPortalContext dataContext = new Models.CrookedPortalContext())
            {
                List<Models.CrossPlatformProductListing> crossPlatformProductListings = dataContext.CrossPlatformProductListings.ToList();

                for (int i = 0; i < crossPlatformProductListings.Count(); i++)
                {
                    labelNumber.Text = String.Format("CPPL {0} of {1}", i + 1, crossPlatformProductListings.Count);
                    this.Refresh();

                    EtsyListing etsyListing = _EtsyListings.First(el => el.ID == crossPlatformProductListings[i].EtsyListingId);
                    List<EtsyListingProduct> etsyListingProducts = JsonSerializer.Deserialize<EtsyListingProductsJSONRoot>(Utilities.GetJSONFromEtsy(Properties.Resources.EtsyListingProductsURL + etsyListing.ID + "/inventory")).Products;

                    foreach (Models.EtsyListingProduct listingProduct in dataContext.EtsyListingProducts.Where(elp => elp.ListingId == etsyListing.ID))
                    {
                        dataContext.Remove(listingProduct);
                    }

                    dataContext.SaveChanges();

                    foreach (EtsyListingProduct etsyListingProduct in etsyListingProducts)
                    {
                        Models.EtsyListingProduct listingProduct = new Models.EtsyListingProduct() 
                        { 
                            Id = etsyListingProduct.ID,
                            ListingId = etsyListing.ID,
                            Price = etsyListingProduct.Offerings[0].Price.Value,
                            Quantity = etsyListingProduct.Offerings[0].Quantity,
                            Size = etsyListingProduct.PropertyValues.Count > 0 ? etsyListingProduct.PropertyValues[0].Values[0] : null,
                            Colour = etsyListingProduct.PropertyValues.Count > 1 ? etsyListingProduct.PropertyValues[1].Values[0] : null
                        };

                        if (crossPlatformProductListings[i].MctproductId != null)
                        {
                            ShopifyProductVariant mctProductVariant = 
                                _MCTProducts.First(mctp => mctp.ID == crossPlatformProductListings[i].MctproductId).Variants
                                            .First(mctpv => mctpv.Size == listingProduct.Size && mctpv.Colour == listingProduct.Colour);

                            listingProduct.MctvariantId = mctProductVariant.ID;
                            dataContext.MctproductVariants.First(mctpv => mctpv.Id == mctProductVariant.ID).EtsyProductId = listingProduct.Id;

                            if (crossPlatformProductListings[i].GcproductId != null)
                            {
                                ShopifyProductVariant gcProductVariant =
                                    _GCProducts.First(gcp => gcp.ID == crossPlatformProductListings[i].GcproductId).Variants
                                               .First(gcpv => gcpv.Size == listingProduct.Size && gcpv.Colour == listingProduct.Colour);

                                listingProduct.GcvariantId = gcProductVariant.ID;
                                dataContext.MctproductVariants.First(mctpv => mctpv.Id == mctProductVariant.ID).GcvariantId = gcProductVariant.ID;
                                dataContext.GcproductVariants.First(gcpv => gcpv.Id == gcProductVariant.ID).EtsyProductId = listingProduct.Id;
                                dataContext.GcproductVariants.First(gcpv => gcpv.Id == gcProductVariant.ID).MctvariantId = mctProductVariant.ID;
                            }
                        }
                        else if (crossPlatformProductListings[i].GcproductId != null)
                        {
                            ShopifyProductVariant gcProductVariant =
                                _GCProducts.First(gcp => gcp.ID == crossPlatformProductListings[i].GcproductId).Variants
                                           .First(gcpv => gcpv.Size == listingProduct.Size && gcpv.Colour == listingProduct.Colour);

                            listingProduct.GcvariantId = gcProductVariant.ID;
                            dataContext.GcproductVariants.First(gcpv => gcpv.Id == gcProductVariant.ID).EtsyProductId = listingProduct.Id;
                        }

                        dataContext.Add(listingProduct);
                    }
                }

                dataContext.SaveChanges();
            }
        }
    }
}