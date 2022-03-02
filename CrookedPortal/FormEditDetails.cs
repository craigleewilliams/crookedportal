using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CrookedPortal
{
    public partial class FormEditDetails : Form
    {
        private ShopifyProduct _ShopifyProduct;
        private ShopifyProduct _CorrespondingMCTProduct;
        private ShopifyProduct _CorrespondingGCProduct;
        private EtsyListing _CorrespondingListing;
        private ShopifyBrand _ShopifyBrand;
        private EtsyListing _EtsyListing;
        private List<EtsyListingProduct> _EtsyListingProducts;
        private Control[,] _Controls;

        public FormEditDetails(ShopifyBrand brand, ref ShopifyProduct product, ref ShopifyProduct correspondingProduct, ref EtsyListing correspondingListing)
        {
            InitializeComponent();
            this.Text = String.Format("Edit {0} Product Variants", _ShopifyBrand);
            _ShopifyBrand = brand;
            _ShopifyProduct = product;
            _CorrespondingMCTProduct = brand == ShopifyBrand.GC ? correspondingProduct : null;
            _CorrespondingGCProduct = brand == ShopifyBrand.MCT ? correspondingProduct : null;
            _CorrespondingListing = correspondingListing;
            _Controls = new Control[product.Variants.Count, 6];
        }

        public FormEditDetails(ref EtsyListing listing, ref ShopifyProduct correspondingMCTProduct, ref ShopifyProduct correspondingGCProduct)
        {
            InitializeComponent();
            this.Text = "Edit Etsy Listing Products";
            labelCompare.Visible = false;
            labelPolicy.Location = labelQuantity.Location;
            labelPolicy.Text = "";
            labelQuantity.Location = labelCompare.Location;
            _ShopifyProduct = null;
            _EtsyListing = listing;
            _EtsyListingProducts = JsonSerializer.Deserialize<EtsyListingProductsJSONRoot>(Utilities.GetJSONFromEtsy(Properties.Resources.EtsyListingProductsURL + listing.ID + "/inventory")).Products;
            _CorrespondingMCTProduct = correspondingMCTProduct;
            _CorrespondingGCProduct = correspondingGCProduct;
            _Controls = new Control[_EtsyListingProducts.Count, 4];
        }

        private void FormEditDetails_Load(object sender, EventArgs e)
        {
            if (_ShopifyProduct != null)
            {
                TextBox textBoxSize;
                TextBox textBoxColour;
                TextBox textBoxPrice;
                TextBox textBoxCompare;
                TextBox textBoxQuantity;
                Panel panelPolicy;
                RadioButton radioButtonContinue;
                RadioButton radioButtonDeny;

                labelTitle.Text = _ShopifyProduct.Title;
                labelStatus.Text = _ShopifyProduct.Status[0].ToString().ToUpper() + _ShopifyProduct.Status[1..];
                labelEditStatus.Text = "Edit Status";
                comboBoxStatus.Items.Add("Active");
                comboBoxStatus.Items.Add("Archived");
                comboBoxStatus.SelectedIndexChanged += control_StateChanged;

                for (int i = 0; i < _ShopifyProduct.Variants.Count; i++)
                {
                    textBoxSize = new TextBox();
                    textBoxColour = new TextBox();
                    textBoxPrice = new TextBox();
                    textBoxCompare = new TextBox();
                    textBoxQuantity = new TextBox();
                    panelPolicy = new Panel();
                    radioButtonContinue = new RadioButton();
                    radioButtonDeny = new RadioButton();

                    textBoxSize.Location = new Point(12, 92+ (i * 29));
                    textBoxSize.Text = _ShopifyProduct.Variants[i].Size;
                    textBoxSize.TextChanged += control_StateChanged;
                    textBoxColour.Location = new Point(118, 92 + (i * 29));
                    textBoxColour.Text = _ShopifyProduct.Variants[i].Colour;
                    textBoxColour.TextChanged += control_StateChanged;
                    textBoxPrice.Location = new Point(224, 92 + (i * 29));
                    textBoxPrice.Text = _ShopifyProduct.Variants[i].Price;
                    textBoxPrice.TextChanged += control_StateChanged;
                    textBoxCompare.Location = new Point(330, 92 + (i * 29));
                    textBoxCompare.Text = _ShopifyProduct.Variants[i].CompareAtPrice ?? "0.00";
                    textBoxCompare.TextChanged += control_StateChanged;
                    textBoxQuantity.Location = new Point(436, 92 + (i * 29));
                    textBoxQuantity.Text = _ShopifyProduct.Variants[i].Quantity.ToString();
                    textBoxQuantity.TextChanged += control_StateChanged;
                    panelPolicy.Size = new Size(140, 23);
                    panelPolicy.Location = new Point(542, 92 + (i * 29));
                    radioButtonContinue.Width = 80;
                    radioButtonContinue.Location = new Point(3, 4);
                    radioButtonContinue.Text = "Continue";
                    radioButtonContinue.Checked = _ShopifyProduct.Variants[i].InventoryPolicy == "continue";
                    radioButtonContinue.CheckedChanged += control_StateChanged;
                    radioButtonDeny.Location = new Point(83, 4);
                    radioButtonDeny.Text = "Deny";
                    radioButtonDeny.Checked = _ShopifyProduct.Variants[i].InventoryPolicy == "deny";
                    radioButtonDeny.CheckedChanged += control_StateChanged;

                    this.Controls.Add(textBoxSize);
                    _Controls[i, 0] = textBoxSize;
                    this.Controls.Add(textBoxColour);
                    _Controls[i, 1] = textBoxColour;
                    this.Controls.Add(textBoxPrice);
                    _Controls[i, 2] = textBoxPrice;
                    this.Controls.Add(textBoxCompare);
                    _Controls[i, 3] = textBoxCompare;
                    this.Controls.Add(textBoxQuantity);
                    _Controls[i, 4] = textBoxQuantity;
                    panelPolicy.Controls.Add(radioButtonContinue);
                    panelPolicy.Controls.Add(radioButtonDeny);
                    this.Controls.Add(panelPolicy);
                    _Controls[i, 5] = panelPolicy;

                    if (_ShopifyProduct.Variants[0].Size == "Default Title")
                    {
                        labelColour.Visible = false;
                        labelPrice.Left -= 106;
                        labelCompare.Left -= 106;
                        labelQuantity.Left -= 106;
                        labelPolicy.Left -= 106;
                        _Controls[i, 1].Visible = false;
                        _Controls[i, 2].Left -= 106;
                        _Controls[i, 3].Left -= 106;
                        _Controls[i, 4].Left -= 106;
                        _Controls[i, 5].Left -= 106;
                    }
                }
            }
            else
            {
                TextBox textBoxSize;
                TextBox textBoxColour;
                TextBox textBoxPrice;
                TextBox textBoxCompare;
                TextBox textBoxQuantity;

                labelTitle.Text = _EtsyListing.Title;
                labelStatus.Text = _EtsyListing.State == "edit" ? "Inactive" : _EtsyListing.State[0].ToString().ToUpper() + _EtsyListing.State[1..];
                labelEditStatus.Text = "Edit State";
                comboBoxStatus.Items.Add("Active");
                comboBoxStatus.Items.Add("Inactive");
                comboBoxStatus.SelectedIndexChanged += control_StateChanged;
                for (int i = 0; i < _EtsyListingProducts.Count; i++)
                {
                    textBoxSize = new TextBox();
                    textBoxColour = new TextBox();
                    textBoxPrice = new TextBox();
                    textBoxCompare = new TextBox();
                    textBoxQuantity = new TextBox();

                    textBoxSize.Location = new Point(12, 92 + (i * 29));
                    textBoxSize.Text = _EtsyListingProducts[i].PropertyValues.Count > 0 ? _EtsyListingProducts[i].PropertyValues[0].Values[0] : "";
                    textBoxSize.TextChanged += control_StateChanged;
                    textBoxColour.Location = new Point(118, 92 + (i * 29));
                    textBoxColour.Text = _EtsyListingProducts[i].PropertyValues.Count > 1 ? _EtsyListingProducts[i].PropertyValues[1].Values[0] : "";
                    textBoxColour.TextChanged += control_StateChanged;
                    textBoxPrice.Location = new Point(224, 92 + (i * 29));
                    textBoxPrice.Text = _EtsyListingProducts[i].Offerings[0].Price.Value.ToString("F");
                    textBoxPrice.TextChanged += control_StateChanged;
                    textBoxQuantity.Location = new Point(330, 92 + (i * 29));
                    textBoxQuantity.Text = _EtsyListingProducts[i].Offerings.Count > 0 ? _EtsyListingProducts[i].Offerings[0].Quantity.ToString() : "";
                    textBoxQuantity.TextChanged += control_StateChanged;

                    this.Controls.Add(textBoxSize);
                    _Controls[i, 0] = textBoxSize;
                    this.Controls.Add(textBoxColour);
                    _Controls[i, 1] = textBoxColour;
                    this.Controls.Add(textBoxPrice);
                    _Controls[i, 2] = textBoxPrice;
                    this.Controls.Add(textBoxQuantity);
                    _Controls[i, 3] = textBoxQuantity;
                }
            }

            buttonSave.Enabled = false;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            buttonSave.Enabled = false;

            using (Models.CrookedPortalContext dataContext = new Models.CrookedPortalContext())
            {
                if (_ShopifyProduct != null)
                {
                    Models.CrossPlatformProductListing crossPlatformProductListing = null;
                    RadioButton rbPolicy;

                    for (int i = 0; i < _ShopifyProduct.Variants.Count; i++)
                    {
                        _ShopifyProduct.Variants[i].Size = _Controls[i, 0].Text;
                        _ShopifyProduct.Variants[i].Colour = _Controls[i, 1].Text;
                        _ShopifyProduct.Variants[i].Price = _Controls[i, 2].Text;
                        _ShopifyProduct.Variants[i].CompareAtPrice = _Controls[i, 3].Text;
                        _ShopifyProduct.Variants[i].Quantity = int.Parse(_Controls[i, 4].Text);

                        rbPolicy = (RadioButton)_Controls[i, 5].Controls[0];
                        _ShopifyProduct.Variants[i].InventoryPolicy = rbPolicy.Checked ? "continue" : "deny";

                        _ShopifyProduct.Variants[i].UpdateInventory(_ShopifyBrand, _ShopifyProduct,
                                                                    _ShopifyProduct.Variants[i].Size,
                                                                    _ShopifyProduct.Variants[i].Colour,
                                                                    _ShopifyProduct.Variants[i].Price,
                                                                    _ShopifyProduct.Variants[i].CompareAtPrice,
                                                                    _ShopifyProduct.Variants[i].Quantity,
                                                                    _ShopifyProduct.Variants[i].InventoryPolicy);

                        if (_ShopifyBrand == ShopifyBrand.MCT)
                        {
                            Models.MctproductVariant mctProductVariant = dataContext.MctproductVariants.First(mctpv => mctpv.Id == _ShopifyProduct.Variants[i].ID);

                            mctProductVariant.Size = _ShopifyProduct.Variants[i].Size;
                            mctProductVariant.Colour = !String.IsNullOrEmpty(_ShopifyProduct.Variants[i].Colour) ? _ShopifyProduct.Variants[i].Colour : null;
                            mctProductVariant.Price = decimal.Parse(_ShopifyProduct.Variants[i].Price);
                            mctProductVariant.CompareAtPrice = decimal.Parse(_ShopifyProduct.Variants[i].CompareAtPrice);
                            mctProductVariant.Quantity = _ShopifyProduct.Variants[i].Quantity;
                            mctProductVariant.InventoryPolicy = _ShopifyProduct.Variants[i].InventoryPolicy;

                            if (dataContext.CrossPlatformProductListings.Any(cppl => cppl.MctproductId == _ShopifyProduct.ID))
                            {
                                crossPlatformProductListing = dataContext.CrossPlatformProductListings.First(cppl => cppl.MctproductId == _ShopifyProduct.ID);

                                if (crossPlatformProductListing.GcproductId != null)
                                {
                                    Models.GcproductVariant gcProductVariant = dataContext.GcproductVariants.First(gcpv => gcpv.MctvariantId == mctProductVariant.Id);

                                    gcProductVariant.Size = _ShopifyProduct.Variants[i].Size;
                                    gcProductVariant.Colour = !String.IsNullOrEmpty(_ShopifyProduct.Variants[i].Colour) ? _ShopifyProduct.Variants[i].Colour : null;
                                    gcProductVariant.Price = decimal.Parse(_ShopifyProduct.Variants[i].Price);
                                    gcProductVariant.CompareAtPrice = decimal.Parse(_ShopifyProduct.Variants[i].CompareAtPrice);
                                    gcProductVariant.Quantity = _ShopifyProduct.Variants[i].Quantity;
                                    gcProductVariant.InventoryPolicy = _ShopifyProduct.Variants[i].InventoryPolicy;
                                }
                            }
                        }
                        else
                        {
                            Models.GcproductVariant gcProductVariant = dataContext.GcproductVariants.First(variant => variant.Id == _ShopifyProduct.Variants[i].ID);

                            gcProductVariant.Size = _ShopifyProduct.Variants[i].Size;
                            gcProductVariant.Colour = !String.IsNullOrEmpty(_ShopifyProduct.Variants[i].Colour) ? _ShopifyProduct.Variants[i].Colour : null;
                            gcProductVariant.Price = decimal.Parse(_ShopifyProduct.Variants[i].Price);
                            gcProductVariant.CompareAtPrice = decimal.Parse(_ShopifyProduct.Variants[i].CompareAtPrice);
                            gcProductVariant.Quantity = _ShopifyProduct.Variants[i].Quantity;
                            gcProductVariant.InventoryPolicy = _ShopifyProduct.Variants[i].InventoryPolicy;

                            if (dataContext.CrossPlatformProductListings.Any(cppl => cppl.GcproductId == _ShopifyProduct.ID))
                            {
                                crossPlatformProductListing = dataContext.CrossPlatformProductListings.First(cppl => cppl.GcproductId == _ShopifyProduct.ID);

                                if (crossPlatformProductListing.MctproductId != null)
                                {
                                    Models.MctproductVariant mctProductVariant = dataContext.MctproductVariants.First(mctpv => mctpv.GcvariantId == gcProductVariant.Id);

                                    mctProductVariant.Size = _ShopifyProduct.Variants[i].Size;
                                    mctProductVariant.Colour = !String.IsNullOrEmpty(_ShopifyProduct.Variants[i].Colour) ? _ShopifyProduct.Variants[i].Colour : null;
                                    mctProductVariant.Price = decimal.Parse(_ShopifyProduct.Variants[i].Price);
                                    mctProductVariant.CompareAtPrice = decimal.Parse(_ShopifyProduct.Variants[i].CompareAtPrice);
                                    mctProductVariant.Quantity = _ShopifyProduct.Variants[i].Quantity;
                                    mctProductVariant.InventoryPolicy = _ShopifyProduct.Variants[i].InventoryPolicy;
                                }
                            }
                        }

                        dataContext.SaveChanges();
                    }

                    if (_ShopifyBrand == ShopifyBrand.MCT)
                    {
                        Models.Mctproduct mctProduct = dataContext.Mctproducts.First(mctp => mctp.Id == _ShopifyProduct.ID);

                        mctProduct.LastUpdated = DateTime.Now;

                        if (dataContext.CrossPlatformProductListings.Any(cppl => cppl.MctproductId == _ShopifyProduct.ID) && crossPlatformProductListing.GcproductId != null)
                        {
                            _CorrespondingGCProduct.UpdateFromCorrespondingProduct(ShopifyBrand.GC, _ShopifyProduct);

                            Models.Gcproduct gcProduct = dataContext.Gcproducts.First(gcp => gcp.Id == _CorrespondingGCProduct.ID);

                            gcProduct.LastUpdated = DateTime.Now;
                        }
                    }
                    else
                    {
                        Models.Gcproduct product = dataContext.Gcproducts.First(gcp => gcp.Id == _ShopifyProduct.ID);

                        product.LastUpdated = DateTime.Now;

                        if (dataContext.CrossPlatformProductListings.Any(cppl => cppl.GcproductId == _ShopifyProduct.ID) && crossPlatformProductListing.MctproductId != null)
                        {
                            _CorrespondingMCTProduct.UpdateFromCorrespondingProduct(ShopifyBrand.MCT, _ShopifyProduct);

                            Models.Mctproduct mctProduct = dataContext.Mctproducts.First(mctp => mctp.Id == _CorrespondingMCTProduct.ID);

                            mctProduct.LastUpdated = DateTime.Now;
                        }
                    }

                    if (crossPlatformProductListing.EtsyListingId != null)
                    {
                        _CorrespondingListing.UpdateProducts(ref _CorrespondingListing, _ShopifyProduct.Variants, true);
                        _EtsyListingProducts = JsonSerializer.Deserialize<EtsyListingProductsJSONRoot>(Utilities.GetJSONFromEtsy(Properties.Resources.EtsyListingProductsURL + _CorrespondingListing.ID + "/inventory")).Products;

                        foreach (Models.EtsyListingProduct product in dataContext.EtsyListingProducts.Where(elp => elp.ListingId == _CorrespondingListing.ID))
                        {
                            dataContext.Remove(product);
                        }

                        dataContext.SaveChanges();

                        foreach (EtsyListingProduct listingProduct in _EtsyListingProducts)
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

                        for (int i = 0; i < _EtsyListingProducts.Count; i++)
                        {
                            if (crossPlatformProductListing.MctproductId != null)
                            {
                                List<Models.MctproductVariant> mctProductVariants = dataContext.MctproductVariants.Where(mctpv => mctpv.ProductId == crossPlatformProductListing.MctproductId).ToList();
                                List<Models.EtsyListingProduct> etsyListingProducts = dataContext.EtsyListingProducts.Where(elp => elp.ListingId == crossPlatformProductListing.EtsyListingId).ToList();

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
                            else if (crossPlatformProductListing.GcproductId != null)
                            {
                                List<Models.GcproductVariant> gcProductVariants = dataContext.GcproductVariants.Where(gcpv => gcpv.ProductId == crossPlatformProductListing.GcproductId).ToList();
                                List<Models.EtsyListingProduct> etsyListingProducts = dataContext.EtsyListingProducts.Where(elp => elp.ListingId == crossPlatformProductListing.EtsyListingId).ToList();

                                gcProductVariants.First(gcpv => gcpv.Id == gcProductVariants[i].Id).EtsyProductId = etsyListingProducts.First(elp => elp.Size == gcProductVariants[i].Size && elp.Colour == gcProductVariants[i].Colour).Id;
                                etsyListingProducts.First(elp => elp.Id == etsyListingProducts[i].Id).GcvariantId = gcProductVariants.First(gcv => gcv.Size == etsyListingProducts[i].Size && gcv.Colour == etsyListingProducts[i].Colour).Id;
                            }
                        }

                        Models.EtsyListing etsyListing = dataContext.EtsyListings.First(el => el.Id == _CorrespondingListing.ID);

                        etsyListing.LastUpdated = DateTime.Now;
                    }

                    dataContext.SaveChanges();
                }
                else
                {
                    Models.CrossPlatformProductListing crossPlatformProductListing = null;
                    List<ShopifyProductVariant> variants = new List<ShopifyProductVariant>();

                    for (int i = 0; i < _EtsyListingProducts.Count; i++)
                    {
                        variants.Add(new ShopifyProductVariant(_Controls[i, 0].Text,
                                                               _Controls[i, 1].Text,
                                                               _Controls[i, 2].Text,
                                                               int.Parse(_Controls[i, 3].Text)));

                        if (dataContext.CrossPlatformProductListings.Any(cppl => cppl.EtsyListingId == _EtsyListing.ID))
                        {
                            crossPlatformProductListing = dataContext.CrossPlatformProductListings.First(cppl => cppl.EtsyListingId == _EtsyListing.ID);
                        }

                        if (crossPlatformProductListing.MctproductId != null)
                        {
                            Models.MctproductVariant mctProductVariant = dataContext.MctproductVariants.First(mctpv => mctpv.EtsyProductId == _EtsyListingProducts[i].ID);

                            mctProductVariant.Size = variants[i].Size;
                            mctProductVariant.Colour = !String.IsNullOrEmpty(variants[i].Colour) ? variants[i].Colour : null;
                            mctProductVariant.Price = decimal.Parse(variants[i].Price);
                            mctProductVariant.Quantity = variants[i].Quantity;
                        }

                        if (crossPlatformProductListing.GcproductId != null)
                        {
                            Models.GcproductVariant gcProductVariant = dataContext.GcproductVariants.First(gcpv => gcpv.EtsyProductId == _EtsyListingProducts[i].ID);

                            gcProductVariant.Size = variants[i].Size;
                            gcProductVariant.Colour = !String.IsNullOrEmpty(variants[i].Colour) ? variants[i].Colour : null;
                            gcProductVariant.Price = decimal.Parse(variants[i].Price);
                            gcProductVariant.Quantity = variants[i].Quantity;
                        }
                    }

                    _EtsyListing.UpdateProducts(ref _EtsyListing, variants, true);
                    _EtsyListingProducts = JsonSerializer.Deserialize<EtsyListingProductsJSONRoot>(Utilities.GetJSONFromEtsy(Properties.Resources.EtsyListingProductsURL + _EtsyListing.ID + "/inventory")).Products;

                    foreach (Models.EtsyListingProduct product in dataContext.EtsyListingProducts.Where(elp => elp.ListingId == _EtsyListing.ID))
                    {
                        dataContext.Remove(product);
                    }

                    dataContext.SaveChanges();

                    foreach (EtsyListingProduct listingProduct in _EtsyListingProducts)
                    {
                        dataContext.Add(new Models.EtsyListingProduct()
                        {
                            Id = listingProduct.ID,
                            ListingId = _EtsyListing.ID,
                            Price = listingProduct.Offerings[0].Price.Value,
                            Quantity = listingProduct.Offerings[0].Quantity,
                            Size = listingProduct.PropertyValues.Count > 0 ? listingProduct.PropertyValues[0].Values[0] : null,
                            Colour = listingProduct.PropertyValues.Count > 1 ? listingProduct.PropertyValues[1].Values[0] : null
                        });
                    }

                    dataContext.SaveChanges();

                    for (int i = 0; i < _EtsyListingProducts.Count; i++)
                    {
                        if (crossPlatformProductListing.MctproductId != null)
                        {
                            List<Models.MctproductVariant> mctProductVariants = dataContext.MctproductVariants.Where(mctpv => mctpv.ProductId == crossPlatformProductListing.MctproductId).ToList();
                            List<Models.EtsyListingProduct> etsyListingProducts = dataContext.EtsyListingProducts.Where(elp => elp.ListingId == crossPlatformProductListing.EtsyListingId).ToList();

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
                        else if (crossPlatformProductListing.GcproductId != null)
                        {
                            List<Models.GcproductVariant> gcProductVariants = dataContext.GcproductVariants.Where(gcpv => gcpv.ProductId == crossPlatformProductListing.GcproductId).ToList();
                            List<Models.EtsyListingProduct> etsyListingProducts = dataContext.EtsyListingProducts.Where(elp => elp.ListingId == crossPlatformProductListing.EtsyListingId).ToList();

                            gcProductVariants.First(gcpv => gcpv.Id == gcProductVariants[i].Id).EtsyProductId = etsyListingProducts.First(elp => elp.Size == gcProductVariants[i].Size && elp.Colour == gcProductVariants[i].Colour).Id;
                            etsyListingProducts.First(elp => elp.Id == etsyListingProducts[i].Id).GcvariantId = gcProductVariants.First(gcv => gcv.Size == etsyListingProducts[i].Size && gcv.Colour == etsyListingProducts[i].Colour).Id;
                        }
                    }

                    Models.EtsyListing listing = dataContext.EtsyListings.First(el => el.Id == _EtsyListing.ID);

                    listing.LastUpdated = DateTime.Now;

                    if (crossPlatformProductListing.MctproductId != null)
                    {
                        string url = Properties.Resources.MCTAPIURL + Properties.Resources.ShopifyProductsURL + "/" +
                                     crossPlatformProductListing.MctproductId + Properties.Resources.ShopifyProductsURLFields;
                        NetworkCredential credentials = new NetworkCredential(Properties.Resources.MCTAPIKey, Properties.Resources.MCTPassword);

                        _ShopifyProduct = JsonSerializer.Deserialize<ShopifyProductJSONRoot>(Utilities.GetJSONFromShopify(url, credentials)).Product;

                        for (int i = 0; i < variants.Count; i++)
                        {
                            _ShopifyProduct.Variants[i].Size = variants[i].Size;
                            _ShopifyProduct.Variants[i].Colour = !String.IsNullOrEmpty(variants[i].Colour) ? variants[i].Colour : null;
                            _ShopifyProduct.Variants[i].Price = variants[i].Price;
                            _ShopifyProduct.Variants[i].Quantity = variants[i].Quantity;
                        }

                        _CorrespondingMCTProduct.UpdateFromCorrespondingProduct(ShopifyBrand.MCT, _ShopifyProduct);

                        Models.Mctproduct mctProduct = dataContext.Mctproducts.First(mctp => mctp.Id == _CorrespondingMCTProduct.ID);

                        mctProduct.LastUpdated = DateTime.Now;
                    }

                    if (crossPlatformProductListing.GcproductId != null)
                    {
                        string url = Properties.Resources.GCAPIURL + Properties.Resources.ShopifyProductsURL + "/" +
                                     crossPlatformProductListing.GcproductId + Properties.Resources.ShopifyProductsURLFields;
                        NetworkCredential credentials = new NetworkCredential(Properties.Resources.GCAPIKey, Properties.Resources.GCPassword);

                        _ShopifyProduct = JsonSerializer.Deserialize<ShopifyProductJSONRoot>(Utilities.GetJSONFromShopify(url, credentials)).Product;

                        for (int i = 0; i < variants.Count; i++)
                        {
                            _ShopifyProduct.Variants[i].Size = variants[i].Size;
                            _ShopifyProduct.Variants[i].Colour = !String.IsNullOrEmpty(variants[i].Colour) ? variants[i].Colour : null;
                            _ShopifyProduct.Variants[i].Price = variants[i].Price;
                            _ShopifyProduct.Variants[i].Quantity = variants[i].Quantity;
                        }

                        _CorrespondingGCProduct.UpdateFromCorrespondingProduct(ShopifyBrand.GC, _ShopifyProduct);

                        Models.Gcproduct gcProduct = dataContext.Gcproducts.First(gcp => gcp.Id == _CorrespondingGCProduct.ID);

                        gcProduct.LastUpdated = DateTime.Now;
                    }

                    dataContext.SaveChanges();

                    _ShopifyProduct = null;
                }
            }

            if (comboBoxStatus.SelectedIndex > -1 && comboBoxStatus.SelectedItem.ToString() != labelStatus.Text)
            {
                if (MessageBox.Show("Are you sure you want to change " + _ShopifyProduct != null ? "product" : "listing" + " state to " + comboBoxStatus.SelectedItem.ToString(), "State Changed", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    using (Models.CrookedPortalContext dataContext = new Models.CrookedPortalContext())
                    {
                        if (_ShopifyProduct != null)
                        {
                            _ShopifyProduct.UpdateStatus(_ShopifyBrand, comboBoxStatus.SelectedItem.ToString());
                            _ShopifyProduct.Status = comboBoxStatus.SelectedItem.ToString();

                            if (_ShopifyBrand == ShopifyBrand.MCT)
                            {
                                Models.Mctproduct product = (from Models.Mctproduct mctp in dataContext.Mctproducts
                                                             where mctp.Id == _ShopifyProduct.ID
                                                             select mctp).First();

                                product.Status = _ShopifyProduct.Status;
                            }
                            else
                            {
                                Models.Gcproduct product = (from Models.Gcproduct gcp in dataContext.Gcproducts
                                                            where gcp.Id == _ShopifyProduct.ID
                                                            select gcp).First();

                                product.Status = _ShopifyProduct.Status;
                            }
                        }
                        else
                        {
                            _EtsyListing.UpdateState(comboBoxStatus.SelectedItem.ToString());
                            _EtsyListing.State = comboBoxStatus.SelectedItem.ToString();

                            Models.EtsyListing listing = (from Models.EtsyListing el in dataContext.EtsyListings
                                                          where el.Id == _EtsyListing.ID
                                                          select el).First();

                            listing.State = _EtsyListing.State;
                        }

                        dataContext.SaveChanges();
                    }
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    buttonSave.Enabled = true;
                }
            }
            else
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
        
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void control_StateChanged(object sender, EventArgs e)
        {
            buttonSave.Enabled = InputValidated(); 
        }

        private bool InputValidated()
        {
            bool isValid = true;
            int errors = 0;

            if (_ShopifyProduct != null)
            {
                for (int i = 0; i < _ShopifyProduct.Variants.Count; i++)
                {
                    isValid = _Controls[i, 0].Text.Length > 0 &&
                              _Controls[i, 2].Text.Length > 0 && ((int.TryParse(_Controls[i, 2].Text, out int intPrice) && intPrice > 0 ) || ((decimal.TryParse(_Controls[i, 2].Text, out decimal decPrice) && decPrice > 0) && Regex.IsMatch(_Controls[i, 2].Text, @"^[0-9]+\.[0-9]{2}$"))) &&
                              _Controls[i, 3].Text.Length > 0 && ((int.TryParse(_Controls[i, 3].Text, out int intCompare) && intCompare >= 0) || ((decimal.TryParse(_Controls[i, 3].Text, out decimal decCompare) && decCompare >= 0) && Regex.IsMatch(_Controls[i, 3].Text, @"^[0-9]+\.[0-9]{2}$"))) &&
                              _Controls[i, 4].Text.Length > 0 && int.TryParse(_Controls[i, 4].Text, out _) &&
                              (comboBoxStatus.SelectedIndex == -1 || comboBoxStatus.SelectedItem.ToString() != labelStatus.Text);

                    errors = isValid ? errors : ++errors;
                }
            }
            else
            {
                for (int i = 0; i < _EtsyListingProducts.Count; i++)
                {
                    isValid = _Controls[i, 2].Text.Length > 0 && ((int.TryParse(_Controls[i, 2].Text, out int intPrice) && intPrice > 0) || ((decimal.TryParse(_Controls[i, 2].Text, out decimal decPrice) && decPrice > 0) && Regex.IsMatch(_Controls[i, 2].Text, @"^[0-9]+\.[0-9]{2}$"))) &&
                              _Controls[i, 3].Text.Length > 0 && int.TryParse(_Controls[i, 3].Text, out int quantity) && quantity >= 0 &&
                              (comboBoxStatus.SelectedIndex == -1 || comboBoxStatus.SelectedItem.ToString() != labelStatus.Text);

                    errors = isValid ? errors : ++errors;
                }
            }

            return isValid;
        }
    }
}
