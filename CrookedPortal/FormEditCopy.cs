using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System.Windows.Forms;

namespace CrookedPortal
{
    public partial class FormEditCopy : Form
    {
        bool _IsEdit;
        ShopifyProduct _ShopifyProduct;
        ShopifyBrand _ShopifyBrand;
        EtsyListing _EtsyListing;

        public EtsyListing EtsyListing { get { return _EtsyListing; } }

        public ShopifyBrand ShopifyBrand { get { return _ShopifyBrand; } }

        public FormEditCopy(ref ShopifyProduct product, ShopifyBrand brand, bool isEdit)
        {
            InitializeComponent();
            _IsEdit = isEdit;
            _ShopifyProduct = product;
            _ShopifyBrand = brand;
            _EtsyListing = null;
        }

        public FormEditCopy(ref EtsyListing listing)
        {
            InitializeComponent();
            _IsEdit = true;
            _ShopifyProduct = null;
            _EtsyListing = listing;
        }

        private void FormEditCopy_Load(object sender, EventArgs e)
        {
            if (_IsEdit)
            {
                if (_ShopifyProduct != null)
                {
                    this.Text = String.Format("Edit {0} Product Details", _ShopifyBrand);
                    labelTitle.Text = _ShopifyProduct.Title;
                    linkLabelEdit.Left = labelTitle.Location.X + labelTitle.Size.Width + 6;
                    textBoxTitle.Text = _ShopifyProduct.Title;
                    textBoxTitle.Visible = false;
                    linkLabelCancel.Visible = false;
                    textBoxDescription.Text = _ShopifyProduct.Description;
                }
                else
                {
                    this.Text = "Edit Etsy Listing Details";
                    labelTitle.Text = _EtsyListing.Title;
                    linkLabelEdit.Left = labelTitle.Location.X + labelTitle.Size.Width + 6;
                    textBoxTitle.Text = _EtsyListing.Title;
                    textBoxTitle.Visible = false;
                    linkLabelCancel.Visible = false;
                    textBoxDescription.Text = Utilities.RemoveSpecialCharacters(Utilities.CleanString(_EtsyListing.Description));
                }
            }
            else
            {
                this.Text = String.Format("Copy {0} Product To Etsy", _ShopifyBrand);
                labelTitle.Text = _ShopifyProduct.Title;
                linkLabelEdit.Visible = false;
                textBoxTitle.Visible = false;
                linkLabelCancel.Visible = false;
                textBoxDescription.Text = Utilities.RemoveSpecialCharacters(Utilities.CleanString(_ShopifyProduct.Description));
                buttonSave.Visible = false;
            }   
        }

        private void linkLabelEdit_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            labelTitle.Visible = false;
            linkLabelEdit.Visible = false;
            textBoxTitle.Visible = true;
            linkLabelCancel.Visible = true;
        }

        private void linkLabelCancel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            labelTitle.Visible = true;
            linkLabelEdit.Visible = true;
            textBoxTitle.Visible = false;
            linkLabelCancel.Visible = false;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            buttonSave.Enabled = false;

            using (Models.CrookedPortalContext dataContext = new Models.CrookedPortalContext())
            {
                if (_ShopifyProduct != null)
                {
                    if (textBoxTitle.Visible)
                    {
                        _ShopifyProduct.Title = textBoxTitle.Text;
                        _ShopifyProduct.UpdateTitle(_ShopifyBrand, textBoxTitle.Text);
                    }
                    _ShopifyProduct.Description = textBoxDescription.Text;
                    _ShopifyProduct.UpdateDescription(_ShopifyBrand, textBoxDescription.Text);
                    if (_ShopifyBrand == ShopifyBrand.MCT)
                    {
                        Models.Mctproduct product = (from Models.Mctproduct mctp in dataContext.Mctproducts 
                                                     where mctp.Id == _ShopifyProduct.ID 
                                                     select mctp).First();

                        product.Title = _ShopifyProduct.Title;
                        product.Description = _ShopifyProduct.Description;
                        product.LastUpdated = DateTime.Now;
                    }
                    else
                    {
                        Models.Gcproduct product = (from Models.Gcproduct gcp in dataContext.Gcproducts 
                                                    where gcp.Id == _ShopifyProduct.ID 
                                                    select gcp).First();

                        product.Title = _ShopifyProduct.Title;
                        product.Description = _ShopifyProduct.Description;
                        product.LastUpdated = DateTime.Now;
                    }
                }
                else
                {
                    if (textBoxTitle.Visible)
                    {
                        _EtsyListing.Title = textBoxTitle.Text;
                        _EtsyListing.UpdateTitle(textBoxTitle.Text);
                    }
                    _EtsyListing.Description = textBoxDescription.Text;
                    _EtsyListing.UpdateDescription(textBoxDescription.Text);

                    Models.EtsyListing listing = (from Models.EtsyListing el in dataContext.EtsyListings where el.Id == _EtsyListing.ID select el).First();

                    listing.Title = _EtsyListing.Title;
                    listing.Description = _EtsyListing.Description;
                    listing.LastUpdated = DateTime.Now;
                }

                dataContext.SaveChanges();
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            buttonCopy.Enabled = false;

            _EtsyListing = _ShopifyProduct.CopyToEtsy(textBoxDescription.Text);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}