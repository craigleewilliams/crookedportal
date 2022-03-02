using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

namespace CrookedPortal
{
    public partial class FormAddData : Form
    {
        public FormAddData()
        {
            InitializeComponent();
        }

        private void FormAddData_Load(object sender, EventArgs e)
        {
            labelGCBold.Visible = false;
            labelGC.Visible = false;
            labelEtsyBold.Visible = false;
            labelEtsy.Visible = false;
        }

        public void AddData(List<ShopifyProduct> mctProducts, List<ShopifyProduct> gcProducts, List<EtsyListing> etsyListings)
        {
            using (Models.CrookedPortalContext dataContext = new Models.CrookedPortalContext())
            {
                int index = 1;

                labelMCTBold.Refresh();
                foreach (ShopifyProduct product in mctProducts)
                {
                    if (!dataContext.Mctproducts.Any(mctp => mctp.Id == product.ID))
                    {
                        labelMCT.Text = "Adding MCT product " + index++ + " of " + mctProducts.Count;
                        labelMCT.Refresh();

                        dataContext.Add(new Models.Mctproduct()
                        {
                            Id = product.ID,
                            Title = product.Title,
                            Description = product.Description,
                            Tags = product.Tags,
                            Status = product.Status,
                            LastUpdated = DateTime.Now,
                            PublishedScope = product.PublishedScope
                        });

                        foreach (ShopifyProductVariant variant in product.Variants)
                        {
                            dataContext.Add(new Models.MctproductVariant()
                            {
                                Id = variant.ID,
                                ProductId = product.ID,
                                InventoryItemId = variant.InventoryItemID,
                                InventoryPolicy = variant.InventoryPolicy,
                                Price = decimal.Parse(variant.Price),
                                CompareAtPrice = decimal.TryParse(variant.CompareAtPrice, out decimal compareAtPrice) ? decimal.Parse(variant.CompareAtPrice) : 0.00m,
                                Quantity = variant.Quantity,
                                Size = variant.Size,
                                Colour = variant.Colour
                            });
                        }

                        foreach (ShopifyProductImage image in product.Images)
                        {
                            dataContext.Add(new Models.MctproductImage()
                            {
                                Id = image.ID,
                                ProductId = product.ID,
                                Position = image.Position,
                                Url = image.URL
                            });
                        }

                        dataContext.SaveChanges();
                    }
                }
                labelMCT.Text = "Done!";
                labelMCT.Refresh();

                index = 1;
                labelGCBold.Visible = true;
                labelGCBold.Refresh();
                labelGC.Visible = true;
                foreach (ShopifyProduct product in gcProducts)
                {
                    if (!dataContext.Gcproducts.Any(gcp => gcp.Id == product.ID))
                    {
                        labelGC.Text = "Adding GC product " + index++ + " of " + gcProducts.Count;
                        labelGC.Refresh();

                        dataContext.Add(new Models.Gcproduct()
                        {
                            Id = product.ID,
                            Title = product.Title,
                            Description = product.Description,
                            Tags = product.Tags,
                            Status = product.Status,
                            LastUpdated = DateTime.Now,
                            PublishedScope = product.PublishedScope
                        });

                        foreach (ShopifyProductVariant variant in product.Variants)
                        {
                            dataContext.Add(new Models.GcproductVariant()
                            {
                                Id = variant.ID,
                                ProductId = product.ID,
                                InventoryItemId = variant.InventoryItemID,
                                InventoryPolicy = variant.InventoryPolicy,
                                Price = decimal.Parse(variant.Price),
                                CompareAtPrice = decimal.TryParse(variant.CompareAtPrice, out decimal compareAtPrice) ? decimal.Parse(variant.CompareAtPrice) : 0.00m,
                                Quantity = variant.Quantity,
                                Size = variant.Size,
                                Colour = variant.Colour
                            });
                        }

                        foreach (ShopifyProductImage image in product.Images)
                        {
                            dataContext.Add(new Models.GcproductImage()
                            {
                                Id = image.ID,
                                ProductId = product.ID,
                                Position = image.Position,
                                Url = image.URL
                            });
                        }

                        dataContext.SaveChanges();
                    }
                }
                labelGC.Text = "Done!";
                labelGC.Refresh();

                index = 1;
                labelEtsyBold.Visible = true;
                labelEtsyBold.Refresh();
                labelEtsy.Visible = true;
                foreach (EtsyListing listing in etsyListings)
                {
                    if (!dataContext.EtsyListings.Any(el => el.Id == listing.ID))
                    {
                        labelEtsy.Text = "Adding Etsy product " + index++ + " of " + etsyListings.Count;
                        labelEtsy.Refresh();

                        dataContext.Add(new Models.EtsyListing()
                        {
                            Id = listing.ID,
                            Title = listing.Title,
                            Description = listing.Description,
                            State = listing.State,
                            LastUpdated = DateTime.Now
                        });

                        List<EtsyListingProduct> listingProducts = JsonSerializer.Deserialize<EtsyListingProductsJSONRoot>(Utilities.GetJSONFromEtsy(Properties.Resources.EtsyListingProductsURL + listing.ID + "/inventory")).Products;

                        foreach (EtsyListingProduct product in listingProducts)
                        {
                            dataContext.Add(new Models.EtsyListingProduct()
                            {
                                Id = product.ID,
                                ListingId = listing.ID,
                                Price = product.Offerings[0].Price.Value,
                                Quantity = product.Offerings[0].Quantity,
                                Size = product.PropertyValues.Count > 0 ? product.PropertyValues[0].Values[0] : null,
                                Colour = product.PropertyValues.Count > 1 ? product.PropertyValues[1].Values[0] : null
                            });
                        }

                        List<EtsyListingImage> images = JsonSerializer.Deserialize<EtsyListingImagesJSONRoot>(Utilities.GetJSONFromEtsy(Properties.Resources.EtsyListingImagesURL + listing.ID + "/images")).Images;

                        foreach (EtsyListingImage image in images)
                        {
                            dataContext.Add(new Models.EtsyListingImage()
                            {
                                ImageId = image.ID,
                                ListingId = listing.ID,
                                Rank = image.Rank,
                                Url = image.URL
                            });
                        }

                        dataContext.SaveChanges();
                    }
                }
                labelEtsy.Text = "Done!";
                labelEtsy.Refresh();

                dataContext.DataTransfers.Add(new Models.DataTransfer()
                {
                    MctproductsTransferred = mctProducts.Count,
                    GcproductsTransferred = gcProducts.Count,
                    EtsyListingsTransferred = etsyListings.Count,
                    DateOfTransfer = DateTime.Now
                });

                dataContext.SaveChanges();
            }
        }
    }
}
