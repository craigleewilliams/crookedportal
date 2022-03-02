namespace CrookedPortal
{
    partial class FormShopify
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelPublished = new System.Windows.Forms.Label();
            this.labelNewMCT = new System.Windows.Forms.Label();
            this.labelNewGC = new System.Windows.Forms.Label();
            this.labelNewEtsy = new System.Windows.Forms.Label();
            this.linkLabelEditEtsyDetails = new System.Windows.Forms.LinkLabel();
            this.linkLabelEditGC = new System.Windows.Forms.LinkLabel();
            this.labelStatus = new System.Windows.Forms.Label();
            this.linkLabelEditGCVariants = new System.Windows.Forms.LinkLabel();
            this.buttonUpdateEtsy = new System.Windows.Forms.Button();
            this.linkLabelEditEtsy = new System.Windows.Forms.LinkLabel();
            this.buttonUnmatchEtsy = new System.Windows.Forms.Button();
            this.labelEtsyState = new System.Windows.Forms.Label();
            this.buttonCopyToEtsy = new System.Windows.Forms.Button();
            this.listViewEtsyDetails = new System.Windows.Forms.ListView();
            this.columnHeaderEtsySize = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderEtsyColour = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderEtsyPrice = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderEtsyQuantity = new System.Windows.Forms.ColumnHeader();
            this.listViewGCVariants = new System.Windows.Forms.ListView();
            this.columnHeaderSize = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderColour = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderPrice = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderCompare = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderQuantity = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderPolicy = new System.Windows.Forms.ColumnHeader();
            this.buttonMatchEtsy = new System.Windows.Forms.Button();
            this.checkBoxOverwritePrice = new System.Windows.Forms.CheckBox();
            this.buttonPrevious = new System.Windows.Forms.Button();
            this.buttonClearEtsy = new System.Windows.Forms.Button();
            this.pictureBoxEtsy = new System.Windows.Forms.PictureBox();
            this.labelEtsy = new System.Windows.Forms.Label();
            this.listBoxEtsyListings = new System.Windows.Forms.ListBox();
            this.labelNumber = new System.Windows.Forms.Label();
            this.buttonNext = new System.Windows.Forms.Button();
            this.labelTitle = new System.Windows.Forms.Label();
            this.pictureBoxMain = new System.Windows.Forms.PictureBox();
            this.buttonClose = new System.Windows.Forms.Button();
            this.textBoxSearch = new System.Windows.Forms.TextBox();
            this.listBoxSearch = new System.Windows.Forms.ListBox();
            this.labelSearch = new System.Windows.Forms.Label();
            this.buttonUpdateDatabase = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxEtsy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMain)).BeginInit();
            this.SuspendLayout();
            // 
            // labelPublished
            // 
            this.labelPublished.AutoSize = true;
            this.labelPublished.Location = new System.Drawing.Point(210, 64);
            this.labelPublished.Name = "labelPublished";
            this.labelPublished.Size = new System.Drawing.Size(87, 15);
            this.labelPublished.TabIndex = 98;
            this.labelPublished.Text = "MCT Published";
            // 
            // labelNewMCT
            // 
            this.labelNewMCT.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelNewMCT.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labelNewMCT.Location = new System.Drawing.Point(706, 390);
            this.labelNewMCT.Name = "labelNewMCT";
            this.labelNewMCT.Size = new System.Drawing.Size(200, 15);
            this.labelNewMCT.TabIndex = 97;
            this.labelNewMCT.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelNewGC
            // 
            this.labelNewGC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelNewGC.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labelNewGC.Location = new System.Drawing.Point(706, 420);
            this.labelNewGC.Name = "labelNewGC";
            this.labelNewGC.Size = new System.Drawing.Size(200, 15);
            this.labelNewGC.TabIndex = 96;
            this.labelNewGC.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelNewEtsy
            // 
            this.labelNewEtsy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelNewEtsy.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labelNewEtsy.Location = new System.Drawing.Point(706, 450);
            this.labelNewEtsy.Name = "labelNewEtsy";
            this.labelNewEtsy.Size = new System.Drawing.Size(200, 15);
            this.labelNewEtsy.TabIndex = 94;
            this.labelNewEtsy.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // linkLabelEditEtsyDetails
            // 
            this.linkLabelEditEtsyDetails.AutoSize = true;
            this.linkLabelEditEtsyDetails.Location = new System.Drawing.Point(807, 318);
            this.linkLabelEditEtsyDetails.Name = "linkLabelEditEtsyDetails";
            this.linkLabelEditEtsyDetails.Size = new System.Drawing.Size(35, 15);
            this.linkLabelEditEtsyDetails.TabIndex = 93;
            this.linkLabelEditEtsyDetails.TabStop = true;
            this.linkLabelEditEtsyDetails.Text = "[Edit]";
            this.linkLabelEditEtsyDetails.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelEditEtsyDetails_LinkClicked);
            // 
            // linkLabelEditGC
            // 
            this.linkLabelEditGC.AutoSize = true;
            this.linkLabelEditGC.Location = new System.Drawing.Point(210, 289);
            this.linkLabelEditGC.Name = "linkLabelEditGC";
            this.linkLabelEditGC.Size = new System.Drawing.Size(35, 15);
            this.linkLabelEditGC.TabIndex = 90;
            this.linkLabelEditGC.TabStop = true;
            this.linkLabelEditGC.Text = "[Edit]";
            this.linkLabelEditGC.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelEditShopifyProduct_LinkClicked);
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labelStatus.Location = new System.Drawing.Point(209, 49);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(67, 15);
            this.labelStatus.TabIndex = 88;
            this.labelStatus.Text = "MCT Status";
            // 
            // linkLabelEditGCVariants
            // 
            this.linkLabelEditGCVariants.AutoSize = true;
            this.linkLabelEditGCVariants.Location = new System.Drawing.Point(357, 491);
            this.linkLabelEditGCVariants.Name = "linkLabelEditGCVariants";
            this.linkLabelEditGCVariants.Size = new System.Drawing.Size(35, 15);
            this.linkLabelEditGCVariants.TabIndex = 87;
            this.linkLabelEditGCVariants.TabStop = true;
            this.linkLabelEditGCVariants.Text = "[Edit]";
            this.linkLabelEditGCVariants.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelEditShopifyDetails_LinkClicked);
            // 
            // buttonUpdateEtsy
            // 
            this.buttonUpdateEtsy.BackColor = System.Drawing.Color.Pink;
            this.buttonUpdateEtsy.Location = new System.Drawing.Point(210, 111);
            this.buttonUpdateEtsy.Name = "buttonUpdateEtsy";
            this.buttonUpdateEtsy.Size = new System.Drawing.Size(111, 23);
            this.buttonUpdateEtsy.TabIndex = 86;
            this.buttonUpdateEtsy.Text = "Update Etsy";
            this.buttonUpdateEtsy.UseVisualStyleBackColor = false;
            this.buttonUpdateEtsy.Click += new System.EventHandler(this.buttonUpdateEtsy_Click);
            // 
            // linkLabelEditEtsy
            // 
            this.linkLabelEditEtsy.AutoSize = true;
            this.linkLabelEditEtsy.Location = new System.Drawing.Point(421, 89);
            this.linkLabelEditEtsy.Name = "linkLabelEditEtsy";
            this.linkLabelEditEtsy.Size = new System.Drawing.Size(35, 15);
            this.linkLabelEditEtsy.TabIndex = 85;
            this.linkLabelEditEtsy.TabStop = true;
            this.linkLabelEditEtsy.Text = "[Edit]";
            this.linkLabelEditEtsy.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelEditEtsy_LinkClicked);
            // 
            // buttonUnmatchEtsy
            // 
            this.buttonUnmatchEtsy.Location = new System.Drawing.Point(382, 136);
            this.buttonUnmatchEtsy.Name = "buttonUnmatchEtsy";
            this.buttonUnmatchEtsy.Size = new System.Drawing.Size(75, 23);
            this.buttonUnmatchEtsy.TabIndex = 83;
            this.buttonUnmatchEtsy.Text = "Unmatch";
            this.buttonUnmatchEtsy.UseVisualStyleBackColor = true;
            this.buttonUnmatchEtsy.Click += new System.EventHandler(this.buttonUnmatchEtsy_Click);
            // 
            // labelEtsyState
            // 
            this.labelEtsyState.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelEtsyState.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labelEtsyState.Location = new System.Drawing.Point(394, 64);
            this.labelEtsyState.Name = "labelEtsyState";
            this.labelEtsyState.Size = new System.Drawing.Size(62, 15);
            this.labelEtsyState.TabIndex = 81;
            this.labelEtsyState.Text = "Etsy State";
            this.labelEtsyState.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // buttonCopyToEtsy
            // 
            this.buttonCopyToEtsy.BackColor = System.Drawing.Color.Pink;
            this.buttonCopyToEtsy.Location = new System.Drawing.Point(209, 82);
            this.buttonCopyToEtsy.Name = "buttonCopyToEtsy";
            this.buttonCopyToEtsy.Size = new System.Drawing.Size(111, 23);
            this.buttonCopyToEtsy.TabIndex = 79;
            this.buttonCopyToEtsy.Text = "Copy To Etsy";
            this.buttonCopyToEtsy.UseVisualStyleBackColor = false;
            this.buttonCopyToEtsy.Click += new System.EventHandler(this.buttonCopyToEtsy_Click);
            // 
            // listViewEtsyDetails
            // 
            this.listViewEtsyDetails.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderEtsySize,
            this.columnHeaderEtsyColour,
            this.columnHeaderEtsyPrice,
            this.columnHeaderEtsyQuantity});
            this.listViewEtsyDetails.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewEtsyDetails.HideSelection = false;
            this.listViewEtsyDetails.Location = new System.Drawing.Point(462, 194);
            this.listViewEtsyDetails.Name = "listViewEtsyDetails";
            this.listViewEtsyDetails.Size = new System.Drawing.Size(339, 139);
            this.listViewEtsyDetails.TabIndex = 77;
            this.listViewEtsyDetails.UseCompatibleStateImageBehavior = false;
            this.listViewEtsyDetails.View = System.Windows.Forms.View.Details;
            // 
            // columnHeaderEtsySize
            // 
            this.columnHeaderEtsySize.Text = "Size";
            this.columnHeaderEtsySize.Width = 55;
            // 
            // columnHeaderEtsyColour
            // 
            this.columnHeaderEtsyColour.Text = "Colour";
            this.columnHeaderEtsyColour.Width = 70;
            // 
            // columnHeaderEtsyPrice
            // 
            this.columnHeaderEtsyPrice.Text = "Price";
            this.columnHeaderEtsyPrice.Width = 56;
            // 
            // columnHeaderEtsyQuantity
            // 
            this.columnHeaderEtsyQuantity.Text = "#";
            this.columnHeaderEtsyQuantity.Width = 25;
            // 
            // listViewGCVariants
            // 
            this.listViewGCVariants.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderSize,
            this.columnHeaderColour,
            this.columnHeaderPrice,
            this.columnHeaderCompare,
            this.columnHeaderQuantity,
            this.columnHeaderPolicy});
            this.listViewGCVariants.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewGCVariants.HideSelection = false;
            this.listViewGCVariants.Location = new System.Drawing.Point(12, 339);
            this.listViewGCVariants.Name = "listViewGCVariants";
            this.listViewGCVariants.Size = new System.Drawing.Size(339, 167);
            this.listViewGCVariants.TabIndex = 76;
            this.listViewGCVariants.UseCompatibleStateImageBehavior = false;
            this.listViewGCVariants.View = System.Windows.Forms.View.Details;
            // 
            // columnHeaderSize
            // 
            this.columnHeaderSize.Text = "Size";
            this.columnHeaderSize.Width = 55;
            // 
            // columnHeaderColour
            // 
            this.columnHeaderColour.Text = "Colour";
            this.columnHeaderColour.Width = 70;
            // 
            // columnHeaderPrice
            // 
            this.columnHeaderPrice.Text = "Price";
            this.columnHeaderPrice.Width = 56;
            // 
            // columnHeaderCompare
            // 
            this.columnHeaderCompare.Text = "Compare";
            this.columnHeaderCompare.Width = 61;
            // 
            // columnHeaderQuantity
            // 
            this.columnHeaderQuantity.Text = "#";
            this.columnHeaderQuantity.Width = 25;
            // 
            // columnHeaderPolicy
            // 
            this.columnHeaderPolicy.Text = "Policy";
            this.columnHeaderPolicy.Width = 68;
            // 
            // buttonMatchEtsy
            // 
            this.buttonMatchEtsy.Location = new System.Drawing.Point(381, 107);
            this.buttonMatchEtsy.Name = "buttonMatchEtsy";
            this.buttonMatchEtsy.Size = new System.Drawing.Size(75, 23);
            this.buttonMatchEtsy.TabIndex = 75;
            this.buttonMatchEtsy.Text = "Match";
            this.buttonMatchEtsy.UseVisualStyleBackColor = true;
            this.buttonMatchEtsy.Click += new System.EventHandler(this.buttonMatchEtsy_Click);
            // 
            // checkBoxOverwritePrice
            // 
            this.checkBoxOverwritePrice.AutoSize = true;
            this.checkBoxOverwritePrice.Location = new System.Drawing.Point(209, 140);
            this.checkBoxOverwritePrice.Name = "checkBoxOverwritePrice";
            this.checkBoxOverwritePrice.Size = new System.Drawing.Size(111, 19);
            this.checkBoxOverwritePrice.TabIndex = 73;
            this.checkBoxOverwritePrice.Text = "Overwrite Price?";
            this.checkBoxOverwritePrice.UseVisualStyleBackColor = true;
            // 
            // buttonPrevious
            // 
            this.buttonPrevious.Location = new System.Drawing.Point(209, 310);
            this.buttonPrevious.Name = "buttonPrevious";
            this.buttonPrevious.Size = new System.Drawing.Size(63, 23);
            this.buttonPrevious.TabIndex = 72;
            this.buttonPrevious.Text = "Previous";
            this.buttonPrevious.UseVisualStyleBackColor = true;
            this.buttonPrevious.Click += new System.EventHandler(this.buttonPrevious_Click);
            // 
            // buttonClearEtsy
            // 
            this.buttonClearEtsy.Location = new System.Drawing.Point(382, 165);
            this.buttonClearEtsy.Name = "buttonClearEtsy";
            this.buttonClearEtsy.Size = new System.Drawing.Size(75, 23);
            this.buttonClearEtsy.TabIndex = 71;
            this.buttonClearEtsy.Text = "Deselect";
            this.buttonClearEtsy.UseVisualStyleBackColor = true;
            this.buttonClearEtsy.Click += new System.EventHandler(this.buttonClearEtsy_Click);
            // 
            // pictureBoxEtsy
            // 
            this.pictureBoxEtsy.Location = new System.Drawing.Point(807, 49);
            this.pictureBoxEtsy.Name = "pictureBoxEtsy";
            this.pictureBoxEtsy.Size = new System.Drawing.Size(99, 139);
            this.pictureBoxEtsy.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxEtsy.TabIndex = 69;
            this.pictureBoxEtsy.TabStop = false;
            // 
            // labelEtsy
            // 
            this.labelEtsy.AutoSize = true;
            this.labelEtsy.Location = new System.Drawing.Point(426, 49);
            this.labelEtsy.Name = "labelEtsy";
            this.labelEtsy.Size = new System.Drawing.Size(31, 15);
            this.labelEtsy.TabIndex = 66;
            this.labelEtsy.Text = "Etsy:";
            // 
            // listBoxEtsyListings
            // 
            this.listBoxEtsyListings.FormattingEnabled = true;
            this.listBoxEtsyListings.ItemHeight = 15;
            this.listBoxEtsyListings.Location = new System.Drawing.Point(462, 49);
            this.listBoxEtsyListings.Name = "listBoxEtsyListings";
            this.listBoxEtsyListings.Size = new System.Drawing.Size(339, 139);
            this.listBoxEtsyListings.TabIndex = 64;
            this.listBoxEtsyListings.SelectedIndexChanged += new System.EventHandler(this.listBoxEtsyListings_SelectedIndexChanged);
            // 
            // labelNumber
            // 
            this.labelNumber.AutoSize = true;
            this.labelNumber.Location = new System.Drawing.Point(12, 31);
            this.labelNumber.Name = "labelNumber";
            this.labelNumber.Size = new System.Drawing.Size(51, 15);
            this.labelNumber.TabIndex = 62;
            this.labelNumber.Text = "Number";
            // 
            // buttonNext
            // 
            this.buttonNext.Location = new System.Drawing.Point(288, 310);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(63, 23);
            this.buttonNext.TabIndex = 61;
            this.buttonNext.Text = "Next";
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.labelTitle.Location = new System.Drawing.Point(12, 10);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(44, 21);
            this.labelTitle.TabIndex = 60;
            this.labelTitle.Text = "Title";
            // 
            // pictureBoxMain
            // 
            this.pictureBoxMain.Location = new System.Drawing.Point(12, 49);
            this.pictureBoxMain.Name = "pictureBoxMain";
            this.pictureBoxMain.Size = new System.Drawing.Size(191, 284);
            this.pictureBoxMain.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxMain.TabIndex = 59;
            this.pictureBoxMain.TabStop = false;
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(831, 482);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 99;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // textBoxSearch
            // 
            this.textBoxSearch.Location = new System.Drawing.Point(618, 12);
            this.textBoxSearch.Name = "textBoxSearch";
            this.textBoxSearch.Size = new System.Drawing.Size(288, 23);
            this.textBoxSearch.TabIndex = 101;
            this.textBoxSearch.TextChanged += new System.EventHandler(this.textBoxSearch_TextChanged);
            // 
            // listBoxSearch
            // 
            this.listBoxSearch.FormattingEnabled = true;
            this.listBoxSearch.ItemHeight = 15;
            this.listBoxSearch.Location = new System.Drawing.Point(618, 34);
            this.listBoxSearch.Name = "listBoxSearch";
            this.listBoxSearch.Size = new System.Drawing.Size(288, 94);
            this.listBoxSearch.TabIndex = 100;
            this.listBoxSearch.Visible = false;
            this.listBoxSearch.SelectedIndexChanged += new System.EventHandler(this.listBoxSearch_SelectedIndexChanged);
            // 
            // labelSearch
            // 
            this.labelSearch.AutoSize = true;
            this.labelSearch.Location = new System.Drawing.Point(567, 20);
            this.labelSearch.Name = "labelSearch";
            this.labelSearch.Size = new System.Drawing.Size(45, 15);
            this.labelSearch.TabIndex = 102;
            this.labelSearch.Text = "Search:";
            // 
            // buttonUpdateDatabase
            // 
            this.buttonUpdateDatabase.BackColor = System.Drawing.Color.Pink;
            this.buttonUpdateDatabase.Location = new System.Drawing.Point(210, 165);
            this.buttonUpdateDatabase.Name = "buttonUpdateDatabase";
            this.buttonUpdateDatabase.Size = new System.Drawing.Size(111, 23);
            this.buttonUpdateDatabase.TabIndex = 103;
            this.buttonUpdateDatabase.Text = "Update Database";
            this.buttonUpdateDatabase.UseVisualStyleBackColor = false;
            this.buttonUpdateDatabase.Click += new System.EventHandler(this.buttonUpdateDatabase_Click);
            // 
            // FormShopify
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(918, 517);
            this.ControlBox = false;
            this.Controls.Add(this.buttonUpdateDatabase);
            this.Controls.Add(this.labelSearch);
            this.Controls.Add(this.textBoxSearch);
            this.Controls.Add(this.listBoxSearch);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.labelPublished);
            this.Controls.Add(this.labelNewMCT);
            this.Controls.Add(this.labelNewGC);
            this.Controls.Add(this.labelNewEtsy);
            this.Controls.Add(this.linkLabelEditEtsyDetails);
            this.Controls.Add(this.linkLabelEditGC);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.linkLabelEditGCVariants);
            this.Controls.Add(this.buttonUpdateEtsy);
            this.Controls.Add(this.linkLabelEditEtsy);
            this.Controls.Add(this.buttonUnmatchEtsy);
            this.Controls.Add(this.labelEtsyState);
            this.Controls.Add(this.buttonCopyToEtsy);
            this.Controls.Add(this.listViewEtsyDetails);
            this.Controls.Add(this.listViewGCVariants);
            this.Controls.Add(this.buttonMatchEtsy);
            this.Controls.Add(this.checkBoxOverwritePrice);
            this.Controls.Add(this.buttonPrevious);
            this.Controls.Add(this.buttonClearEtsy);
            this.Controls.Add(this.pictureBoxEtsy);
            this.Controls.Add(this.labelEtsy);
            this.Controls.Add(this.listBoxEtsyListings);
            this.Controls.Add(this.labelNumber);
            this.Controls.Add(this.buttonNext);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.pictureBoxMain);
            this.MaximizeBox = false;
            this.Name = "FormShopify";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Crooked Portal - Get Crooked";
            this.Load += new System.EventHandler(this.FormGetCrooked_Load);
            this.Shown += new System.EventHandler(this.FormGetCrooked_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxEtsy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMain)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelPublished;
        private System.Windows.Forms.Label labelNewMCT;
        private System.Windows.Forms.Label labelNewGC;
        private System.Windows.Forms.Label labelNewEtsy;
        private System.Windows.Forms.LinkLabel linkLabelEditEtsyDetails;
        private System.Windows.Forms.LinkLabel linkLabelEditGC;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.LinkLabel linkLabelEditGCVariants;
        private System.Windows.Forms.Button buttonUpdateEtsy;
        private System.Windows.Forms.LinkLabel linkLabelEditEtsy;
        private System.Windows.Forms.Button buttonUnmatchEtsy;
        private System.Windows.Forms.Label labelEtsyState;
        private System.Windows.Forms.Button buttonCopyToEtsy;
        private System.Windows.Forms.ListView listViewEtsyDetails;
        private System.Windows.Forms.ColumnHeader columnHeaderEtsySize;
        private System.Windows.Forms.ColumnHeader columnHeaderEtsyColour;
        private System.Windows.Forms.ColumnHeader columnHeaderEtsyPrice;
        private System.Windows.Forms.ColumnHeader columnHeaderEtsyQuantity;
        private System.Windows.Forms.ListView listViewGCVariants;
        private System.Windows.Forms.ColumnHeader columnHeaderSize;
        private System.Windows.Forms.ColumnHeader columnHeaderColour;
        private System.Windows.Forms.ColumnHeader columnHeaderPrice;
        private System.Windows.Forms.ColumnHeader columnHeaderCompare;
        private System.Windows.Forms.ColumnHeader columnHeaderQuantity;
        private System.Windows.Forms.ColumnHeader columnHeaderPolicy;
        private System.Windows.Forms.Button buttonMatchEtsy;
        private System.Windows.Forms.CheckBox checkBoxOverwritePrice;
        private System.Windows.Forms.Button buttonPrevious;
        private System.Windows.Forms.Button buttonClearEtsy;
        private System.Windows.Forms.PictureBox pictureBoxEtsy;
        private System.Windows.Forms.Label labelEtsy;
        private System.Windows.Forms.ListBox listBoxEtsyListings;
        private System.Windows.Forms.Label labelNumber;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.PictureBox pictureBoxMain;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.TextBox textBoxSearch;
        private System.Windows.Forms.ListBox listBoxSearch;
        private System.Windows.Forms.Label labelSearch;
        private System.Windows.Forms.Button buttonUpdateDatabase;
    }
}