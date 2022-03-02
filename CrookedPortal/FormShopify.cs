using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Windows.Forms;

namespace CrookedPortal
{
    public partial class FormShopify : Form
    {
        ShopifyBrand _ShopifyBrand;
        List<ShopifyProduct> _GCProducts;
        List<ShopifyProduct> _ProductsToUpdate;
        ShopifyProduct _SelectedProduct;
        ShopifyProduct _CorrespondingProduct;

        List<EtsyListing> _EtsyListings;
        EtsyListing _CorrespondingListing;
        List<long> _MatchedEtsyListingIds;
        bool etsyMatched;

        int _Index;

        public FormShopify(ShopifyBrand brand, List<ShopifyProduct> products, List<EtsyListing> listings)
        {
            InitializeComponent();

            _ShopifyBrand = brand;
            _GCProducts = products;
            _EtsyListings = listings;
        }

        private void FormGetCrooked_Load(object sender, EventArgs e)
        {
            _CorrespondingProduct = null;
            _CorrespondingListing = null;
            
            etsyMatched = false;

            _Index = 0;
        }

        private void FormGetCrooked_Shown(object sender, EventArgs e)
        {
            _ProductsToUpdate = _GCProducts;

            _SelectedProduct = _ProductsToUpdate[_Index];

            LoadProductDetails();
        }

        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            string searchTerms = textBoxSearch.Text.ToLower();

            listBoxSearch.Visible = false;
            if (String.IsNullOrEmpty(searchTerms))
            {
                return;
            }

            List<ShopifyProduct> matches = Utilities.GetProductMatches(searchTerms, ShopifyBrand.GC, _GCProducts);

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
            _SelectedProduct = _GCProducts.First(product => product.Title == listBoxSearch.SelectedItem.ToString());
            
            textBoxSearch.Text = listBoxSearch.SelectedItem.ToString();
            listBoxSearch.Visible = false;

            LoadProductDetails();
        }

        private void DisableControls()
        {
            buttonPrevious.Enabled = false;
            buttonNext.Enabled = false;
            buttonCopyToEtsy.Enabled = false;
            buttonUpdateEtsy.Enabled = false;
            linkLabelEditGC.Enabled = false;
            linkLabelEditEtsy.Enabled = false;
            buttonMatchEtsy.Enabled = false;
            buttonUnmatchEtsy.Enabled = false;
            buttonClearEtsy.Enabled = false;
            checkBoxOverwritePrice.Enabled = false;
        }

        private void EnableControls()
        {
            buttonPrevious.Enabled = _Index > 0;
            buttonNext.Enabled = _Index < _ProductsToUpdate.Count;

            buttonCopyToEtsy.Enabled = !etsyMatched;
            buttonUpdateEtsy.Enabled = listBoxEtsyListings.SelectedIndex > -1;
            checkBoxOverwritePrice.Enabled = listBoxEtsyListings.SelectedIndex > -1;
            linkLabelEditGC.Enabled = true;
            linkLabelEditGCVariants.Enabled = true;

            linkLabelEditEtsy.Enabled = listBoxEtsyListings.SelectedIndex > -1;
            buttonMatchEtsy.Enabled = !etsyMatched && listBoxEtsyListings.SelectedIndex > -1;
            buttonUnmatchEtsy.Enabled = etsyMatched;
            buttonClearEtsy.Enabled = listBoxEtsyListings.SelectedIndex > -1;
            linkLabelEditEtsyDetails.Enabled = listBoxEtsyListings.SelectedIndex > -1;
        }

        private void LoadProductDetails()
        {
            DisableControls();

            labelTitle.Text = _SelectedProduct.Title;
            labelNumber.Text = String.Format("Get Crooked - {0} of {1}", _Index + 1, _ProductsToUpdate.Count);
            if (_SelectedProduct.Images.Count > 0)
            {
                pictureBoxMain.LoadAsync(_SelectedProduct.Images[0].URL);
            }
            labelStatus.Text = _SelectedProduct.Status[0].ToString().ToUpper() + _SelectedProduct.Status[1..];
            labelPublished.Text = "Sales Channels: " + (_SelectedProduct.PublishedScope != null ? _SelectedProduct.PublishedScope[0].ToString().ToUpper() + _SelectedProduct.PublishedScope[1..] : "None");
            listViewGCVariants.Items.Clear();
            foreach (ShopifyProductVariant variant in _SelectedProduct.Variants)
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

            labelEtsyState.Text = String.Empty;
            _MatchedEtsyListingIds = new List<long>();
            listBoxEtsyListings.Items.Clear();
            using (Models.CrookedPortalContext dataContext = new Models.CrookedPortalContext())
            {
                if (dataContext.CrossPlatformProductListings.Any(cppl => cppl.GcproductId == _SelectedProduct.ID && cppl.EtsyListingId != null))
                {
                    etsyMatched = true;

                    Models.CrossPlatformProductListing record = dataContext.CrossPlatformProductListings.First(cppl => cppl.GcproductId == _SelectedProduct.ID);

                    _CorrespondingListing = _EtsyListings.First(listing => listing.ID == record.EtsyListingId);

                    _MatchedEtsyListingIds.Add(_CorrespondingListing.ID);
                    listBoxEtsyListings.Items.Add(_CorrespondingListing.Title);
                    listBoxEtsyListings.SetSelected(0, true);

                    EtsyListingImage image = JsonSerializer.Deserialize<EtsyListingImagesJSONRoot>(Utilities.GetJSONFromEtsy(Properties.Resources.EtsyListingImagesURL + _CorrespondingListing.ID + "/images")).Images.OrderBy(i => i.Rank).First();

                    pictureBoxEtsy.LoadAsync(image.URL);

                    labelEtsyState.Text = _CorrespondingListing.State == "edit" ? "Inactive" : _CorrespondingListing.State[0].ToString().ToUpper() + _CorrespondingListing.State[1..];
                    listViewEtsyDetails.Items.Clear();
                    foreach (EtsyListingProduct product in JsonSerializer.Deserialize<EtsyListingProductsJSONRoot>(Utilities.GetJSONFromEtsy(Properties.Resources.EtsyListingProductsURL + _CorrespondingListing.ID + "/inventory")).Products)
                    {
                        listViewEtsyDetails.Items.Add(new ListViewItem(new string[]
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
                    etsyMatched = false;

                    _SelectedProduct.GetListingMatches(ShopifyBrand.MCT, _EtsyListings).ForEach(delegate (EtsyListing listing)
                    {
                        _MatchedEtsyListingIds.Add(listing.ID);
                        listBoxEtsyListings.Items.Add(listing.Title);
                    });
                    pictureBoxEtsy.Image = null;
                    listViewEtsyDetails.Items.Clear();
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

                            mctProductVariants.First(mctpv => mctpv.Id == mctProductVariants[i].Id).EtsyProductId = etsyListingProducts.First(elp => elp.Size == mctProductVariants[i].Size && elp.Colour == mctProductVariants[i].Colour).Id;
                            mctProductVariants.First(mctpv => mctpv.Id == mctProductVariants[i].Id).GcvariantId = gcProductVariants.First(gcpv => gcpv.Size == mctProductVariants[i].Size && gcpv.Colour == mctProductVariants[i].Colour).Id;
                            gcProductVariants.First(gcpv => gcpv.Id == gcProductVariants[i].Id).MctvariantId = mctProductVariants.First(mctpv => mctpv.Size == gcProductVariants[i].Size && mctpv.Colour == gcProductVariants[i].Colour).Id;
                            etsyListingProducts.First(elp => elp.Id == etsyListingProducts[i].Id).MctvariantId = mctProductVariants.First(mctpv => mctpv.Size == etsyListingProducts[i].Size && mctpv.Colour == etsyListingProducts[i].Colour).Id;
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
            _CorrespondingListing.Title = _SelectedProduct.Title;
            _CorrespondingListing.UpdateTitle(_SelectedProduct.Title);
            _CorrespondingListing.UpdateProducts(ref _CorrespondingListing, _SelectedProduct.Variants, checkBoxOverwritePrice.Checked);

            using (Models.CrookedPortalContext dataContext = new Models.CrookedPortalContext())
            {
                dataContext.EtsyListings.Where(el => el.Id == _CorrespondingListing.ID).First().Title = _SelectedProduct.Title;

                foreach (Models.EtsyListingProduct listingProduct in dataContext.EtsyListingProducts.Where(elp => elp.ListingId == _CorrespondingListing.ID))
                {
                    dataContext.Remove(listingProduct);
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

            labelEtsyState.Text = _CorrespondingListing.State == "edit" ? "Inactive" : _CorrespondingListing.State[0].ToString().ToUpper() + _CorrespondingListing.State[1..];
            listBoxEtsyListings.Items.Clear();
            listBoxEtsyListings.Items.Add(_CorrespondingListing.Title);
            listBoxEtsyListings.SetSelected(0, true);
            listViewEtsyDetails.Items.Clear();
            foreach (EtsyListingProduct product in JsonSerializer.Deserialize<EtsyListingProductsJSONRoot>(Utilities.GetJSONFromEtsy(Properties.Resources.EtsyListingProductsURL + _CorrespondingListing.ID + "/inventory")).Products)
            {
                listViewEtsyDetails.Items.Add(new ListViewItem(new string[]
                {
                    product.PropertyValues.Count > 0 ? product.PropertyValues[0].Values[0] : "",
                    product.PropertyValues.Count > 1 ? product.PropertyValues[1].Values[0] : "",
                    "£" + product.Offerings[0].Price.Value.ToString("0.00"),
                    product.Offerings.Count > 0 ? product.Offerings[0].Quantity.ToString() : ""
                }));
            }
        }

        private void UpdateEtsyData()
        {
            using (Models.CrookedPortalContext dataContext = new Models.CrookedPortalContext())
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
                        etsyListingProducts.First(elp => elp.Id == etsyListingProducts[i].Id).MctvariantId = mctProductVariants.First(mctpv => mctpv.Size == etsyListingProducts[i].Size && mctpv.Colour == etsyListingProducts[i].Colour).Id;
                    }
                }

                dataContext.SaveChanges();
            }
        }

        private void linkLabelEditShopifyProduct_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (FormEditCopy form = new FormEditCopy(ref _SelectedProduct, ShopifyBrand.GC, true))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    labelTitle.Text = _SelectedProduct.Title;
                }
            }
        }

        private void linkLabelEditShopifyDetails_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShopifyProduct correspondingProduct = null;

            using (Models.CrookedPortalContext dataContext = new Models.CrookedPortalContext())
            {
                if (dataContext.CrossPlatformProductListings.Any(cppl => cppl.GcproductId == _SelectedProduct.ID && cppl.MctproductId != null))
                {
                    Models.CrossPlatformProductListing crossPlatformProductListing = dataContext.CrossPlatformProductListings.First(cppl => cppl.GcproductId == _SelectedProduct.ID);

                    if (crossPlatformProductListing.MctproductId != null)
                    {
                        string url = Properties.Resources.MCTAPIURL + Properties.Resources.ShopifyProductsURL + "/" +
                                     crossPlatformProductListing.MctproductId + Properties.Resources.ShopifyProductsURLFields;
                        NetworkCredential credentials = new NetworkCredential(Properties.Resources.MCTAPIKey, Properties.Resources.MCTPassword);

                        correspondingProduct = JsonSerializer.Deserialize<ShopifyProductJSONRoot>(Utilities.GetJSONFromShopify(url, credentials)).Product;
                    }
                }
            }

            using (FormEditDetails form = new FormEditDetails(ShopifyBrand.GC, ref _SelectedProduct, ref correspondingProduct, ref _CorrespondingListing))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    UpdateEtsyData();

                    listViewGCVariants.Items.Clear();
                    foreach (ShopifyProductVariant variant in _SelectedProduct.Variants)
                    {
                        listViewGCVariants.Items.Add(new ListViewItem(new[]
                        {
                            variant.Size,
                            variant.Colour,
                            "£" + decimal.Parse(variant.Price).ToString("0.00"),
                            variant.CompareAtPrice ?? "0.00",
                            variant.Quantity.ToString(),
                            variant.InventoryPolicy
                        }));
                    }

                    if (etsyMatched)
                    {
                        labelEtsyState.Text = _CorrespondingListing.State == "edit" ? "Inactive" : _CorrespondingListing.State[0].ToString().ToUpper() + _CorrespondingListing.State[1..];
                        listViewEtsyDetails.Items.Clear();
                        foreach (EtsyListingProduct product in JsonSerializer.Deserialize<EtsyListingProductsJSONRoot>(Utilities.GetJSONFromEtsy(Properties.Resources.EtsyListingProductsURL + _CorrespondingListing.ID + "/inventory")).Products)
                        {
                            listViewEtsyDetails.Items.Add(new ListViewItem(new string[]
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

        private void linkLabelEditEtsy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            int index = listBoxEtsyListings.SelectedIndex;

            using (FormEditCopy form = new FormEditCopy(ref _CorrespondingListing))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    listViewEtsyDetails.Items.Clear();
                    listBoxEtsyListings.Items[index] = _CorrespondingListing.Title;
                    typeof(System.Windows.Forms.ListBox).InvokeMember("RefreshItems", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod, null, listBoxEtsyListings, new object[] { });
                    listBoxEtsyListings.SelectedIndex = index;
                }
            }
        }

        private void buttonMatchEtsy_Click(object sender, EventArgs e)
        {
            DisableControls();

            UpdateEtsy();

            using (Models.CrookedPortalContext dataContext = new Models.CrookedPortalContext())
            {
                Models.CrossPlatformProductListing crossPlatformProductListing;

                if (dataContext.CrossPlatformProductListings.Any(cppl => cppl.GcproductId == _SelectedProduct.ID))
                {
                    crossPlatformProductListing = dataContext.CrossPlatformProductListings.First(cppl => cppl.GcproductId == _SelectedProduct.ID);
                    crossPlatformProductListing.EtsyListingId = _CorrespondingListing.ID;

                    if (!dataContext.EtsyListings.Any(listing => listing.Id == _CorrespondingListing.ID))
                    {
                        dataContext.Add(new Models.EtsyListing()
                        {
                            Id = _CorrespondingListing.ID,
                            Title = _SelectedProduct.Title,
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
                    if (dataContext.CrossPlatformProductListings.Any(cppl => cppl.EtsyListingId == _CorrespondingListing.ID))
                    {
                        crossPlatformProductListing = dataContext.CrossPlatformProductListings.First(cppl => cppl.EtsyListingId == _CorrespondingListing.ID);
                        crossPlatformProductListing.GcproductId = _SelectedProduct.ID;
                    }
                    else
                    {
                        crossPlatformProductListing = new Models.CrossPlatformProductListing
                        {
                            GcproductId = _SelectedProduct.ID,
                            EtsyListingId = _CorrespondingListing.ID
                        };
                    }

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
                            Title = _SelectedProduct.Title,
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
                    gcProductVariants.First(gcpv => gcpv.Id == gcProductVariants[i].Id).EtsyProductId = etsyListingProducts.Where(elp => elp.Size == gcProductVariants[i].Size && elp.Colour == gcProductVariants[i].Colour).FirstOrDefault().Id;
                    etsyListingProducts.First(elp => elp.Id == etsyListingProducts[i].Id).GcvariantId = gcProductVariants.Where(gcpv => gcpv.Size == etsyListingProducts[i].Size && gcpv.Colour == etsyListingProducts[i].Colour).FirstOrDefault().Id;

                    if (crossPlatformProductListing.MctproductId != null)
                    {
                        List<Models.MctproductVariant> mctProductVariants = dataContext.MctproductVariants.Where(mctpv => mctpv.ProductId == crossPlatformProductListing.MctproductId).ToList();

                        gcProductVariants.First(gcpv => gcpv.Id == gcProductVariants[i].Id).MctvariantId = mctProductVariants.First(mctpv => mctpv.Size == gcProductVariants[i].Size && mctpv.Colour == gcProductVariants[i].Colour).Id;
                        mctProductVariants.First(mctpv => mctpv.Id == mctProductVariants[i].Id).GcvariantId = gcProductVariants.First(gcpv => gcpv.Size == mctProductVariants[i].Size && gcpv.Colour == mctProductVariants[i].Colour).Id;
                        mctProductVariants.First(mctpv => mctpv.Id == mctProductVariants[i].Id).EtsyProductId = etsyListingProducts.First(elp => elp.Size == mctProductVariants[i].Size && elp.Colour == mctProductVariants[i].Colour).Id;
                        etsyListingProducts.First(elp => elp.Id == etsyListingProducts[i].Id).MctvariantId = mctProductVariants.First(mctpv => mctpv.Size == etsyListingProducts[i].Size && mctpv.Colour == etsyListingProducts[i].Colour).Id;
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

                foreach (Models.EtsyListingProduct listingProduct in etsyProducts)
                {
                    listingProduct.GcvariantId = null;
                    dataContext.GcproductVariants.First(gcpv => gcpv.EtsyProductId == listingProduct.Id).EtsyProductId = null;

                    if (crossPlatformProductListing.MctproductId != null)
                    {
                        listingProduct.MctvariantId = null;
                        dataContext.MctproductVariants.First(mctpv => mctpv.EtsyProductId == listingProduct.Id).EtsyProductId = null;
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
            listViewEtsyDetails.Items.Clear();

            EnableControls();
        }

        private void buttonClearEtsy_Click(object sender, EventArgs e)
        {
            listBoxEtsyListings.ClearSelected();
            listViewEtsyDetails.Items.Clear();
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
                this.Refresh();

                listViewEtsyDetails.Items.Clear();
                foreach (EtsyListingProduct product in JsonSerializer.Deserialize<EtsyListingProductsJSONRoot>(Utilities.GetJSONFromEtsy(Properties.Resources.EtsyListingProductsURL + _CorrespondingListing.ID + "/inventory")).Products)
                {
                    listViewEtsyDetails.Items.Add(new ListViewItem(new string[]
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

        private void linkLabelEditEtsyDetails_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (Models.CrookedPortalContext dataContext = new Models.CrookedPortalContext())
            {
                Models.CrossPlatformProductListing crossPlatformProductListing = dataContext.CrossPlatformProductListings.First(cppl => cppl.EtsyListingId == _CorrespondingListing.ID);

                if (_ShopifyBrand == ShopifyBrand.MCT)
                {
                    if (crossPlatformProductListing.GcproductId != null)
                    {
                        string url = Properties.Resources.GCAPIURL + Properties.Resources.ShopifyProductsURL + "/" +
                                     crossPlatformProductListing.GcproductId + Properties.Resources.ShopifyProductsURLFields;
                        NetworkCredential credentials = new NetworkCredential(Properties.Resources.GCAPIKey, Properties.Resources.GCPassword);

                        _CorrespondingProduct = JsonSerializer.Deserialize<ShopifyProductJSONRoot>(Utilities.GetJSONFromShopify(url, credentials)).Product;

                        using (FormEditDetails form = new FormEditDetails(ref _CorrespondingListing, ref _CorrespondingProduct, ref _SelectedProduct))
                        {
                            if (form.ShowDialog() == DialogResult.OK)
                            {
                                UpdateEtsyData();

                                labelEtsyState.Text = _CorrespondingListing.State == "edit" ? "Inactive" : _CorrespondingListing.State[0].ToString().ToUpper() + _CorrespondingListing.State[1..];
                                listViewEtsyDetails.Items.Clear();
                                foreach (EtsyListingProduct product in JsonSerializer.Deserialize<EtsyListingProductsJSONRoot>(Utilities.GetJSONFromEtsy(Properties.Resources.EtsyListingProductsURL + _CorrespondingListing.ID + "/inventory")).Products)
                                {
                                    listViewEtsyDetails.Items.Add(new ListViewItem(new string[]
                                    {
                                        product.PropertyValues.Count > 0 ? product.PropertyValues[0].Values[0] : "",
                                        product.PropertyValues.Count > 1 ? product.PropertyValues[1].Values[0] : "",
                                        "£" + product.Offerings[0].Price.Value.ToString("0.00"),
                                        product.Offerings.Count > 0 ? product.Offerings[0].Quantity.ToString() : ""
                                    }));
                                }

                                if (etsyMatched)
                                {
                                    listViewGCVariants.Items.Clear();
                                    foreach (ShopifyProductVariant variant in _SelectedProduct.Variants)
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
                }
                else
                {
                    if (crossPlatformProductListing.MctproductId != null)
                    {
                        string url = Properties.Resources.MCTAPIURL + Properties.Resources.ShopifyProductsURL + "/" +
                                     crossPlatformProductListing.MctproductId + Properties.Resources.ShopifyProductsURLFields;
                        NetworkCredential credentials = new NetworkCredential(Properties.Resources.MCTAPIKey, Properties.Resources.MCTPassword);

                        _CorrespondingProduct = JsonSerializer.Deserialize<ShopifyProductJSONRoot>(Utilities.GetJSONFromShopify(url, credentials)).Product;
                    }

                    using (FormEditDetails form = new FormEditDetails(ref _CorrespondingListing, ref _CorrespondingProduct, ref _SelectedProduct))
                    {
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            UpdateEtsyData();

                            labelEtsyState.Text = _CorrespondingListing.State == "edit" ? "Inactive" : _CorrespondingListing.State[0].ToString().ToUpper() + _CorrespondingListing.State[1..];
                            listViewEtsyDetails.Items.Clear();
                            foreach (EtsyListingProduct product in JsonSerializer.Deserialize<EtsyListingProductsJSONRoot>(Utilities.GetJSONFromEtsy(Properties.Resources.EtsyListingProductsURL + _CorrespondingListing.ID + "/inventory")).Products)
                            {
                                listViewEtsyDetails.Items.Add(new ListViewItem(new string[]
                                {
                                        product.PropertyValues.Count > 0 ? product.PropertyValues[0].Values[0] : "",
                                        product.PropertyValues.Count > 1 ? product.PropertyValues[1].Values[0] : "",
                                        "£" + product.Offerings[0].Price.Value.ToString("0.00"),
                                        product.Offerings.Count > 0 ? product.Offerings[0].Quantity.ToString() : ""
                                }));
                            }

                            if (etsyMatched)
                            {
                                listViewGCVariants.Items.Clear();
                                foreach (ShopifyProductVariant variant in _SelectedProduct.Variants)
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
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
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

        private void buttonUpdateDatabase_Click(object sender, EventArgs e)
        {
            using (Models.CrookedPortalContext dataContext = new Models.CrookedPortalContext())
            {
                foreach (Models.GcproductVariant variant in dataContext.GcproductVariants.Where(gcpv => gcpv.ProductId == _SelectedProduct.ID))
                {
                    dataContext.Remove(variant);
                }

                dataContext.SaveChanges();

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

                dataContext.SaveChanges();
            }
        }
    }
}
