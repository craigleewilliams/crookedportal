
namespace CrookedPortal
{
    partial class FormInitialise
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
            this.labelMCTBold = new System.Windows.Forms.Label();
            this.labelGCBold = new System.Windows.Forms.Label();
            this.labelEtsyBold = new System.Windows.Forms.Label();
            this.labelMCT = new System.Windows.Forms.Label();
            this.labelGC = new System.Windows.Forms.Label();
            this.labelEtsy = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelMCTBold
            // 
            this.labelMCTBold.AutoSize = true;
            this.labelMCTBold.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.labelMCTBold.Location = new System.Drawing.Point(12, 27);
            this.labelMCTBold.Name = "labelMCTBold";
            this.labelMCTBold.Size = new System.Drawing.Size(94, 15);
            this.labelMCTBold.TabIndex = 1;
            this.labelMCTBold.Text = "Initialising MCT:";
            // 
            // labelGCBold
            // 
            this.labelGCBold.AutoSize = true;
            this.labelGCBold.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.labelGCBold.Location = new System.Drawing.Point(12, 58);
            this.labelGCBold.Name = "labelGCBold";
            this.labelGCBold.Size = new System.Drawing.Size(85, 15);
            this.labelGCBold.TabIndex = 2;
            this.labelGCBold.Text = "Initialising GC:";
            // 
            // labelEtsyBold
            // 
            this.labelEtsyBold.AutoSize = true;
            this.labelEtsyBold.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.labelEtsyBold.Location = new System.Drawing.Point(12, 88);
            this.labelEtsyBold.Name = "labelEtsyBold";
            this.labelEtsyBold.Size = new System.Drawing.Size(91, 15);
            this.labelEtsyBold.TabIndex = 3;
            this.labelEtsyBold.Text = "Initialising Etsy:";
            // 
            // labelMCT
            // 
            this.labelMCT.AutoSize = true;
            this.labelMCT.Location = new System.Drawing.Point(112, 27);
            this.labelMCT.Name = "labelMCT";
            this.labelMCT.Size = new System.Drawing.Size(38, 15);
            this.labelMCT.TabIndex = 4;
            this.labelMCT.Text = "label1";
            // 
            // labelGC
            // 
            this.labelGC.AutoSize = true;
            this.labelGC.Location = new System.Drawing.Point(103, 58);
            this.labelGC.Name = "labelGC";
            this.labelGC.Size = new System.Drawing.Size(38, 15);
            this.labelGC.TabIndex = 5;
            this.labelGC.Text = "label2";
            // 
            // labelEtsy
            // 
            this.labelEtsy.AutoSize = true;
            this.labelEtsy.Location = new System.Drawing.Point(109, 88);
            this.labelEtsy.Name = "labelEtsy";
            this.labelEtsy.Size = new System.Drawing.Size(38, 15);
            this.labelEtsy.TabIndex = 6;
            this.labelEtsy.Text = "label3";
            // 
            // FormInitialise
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 136);
            this.ControlBox = false;
            this.Controls.Add(this.labelEtsy);
            this.Controls.Add(this.labelGC);
            this.Controls.Add(this.labelMCT);
            this.Controls.Add(this.labelEtsyBold);
            this.Controls.Add(this.labelGCBold);
            this.Controls.Add(this.labelMCTBold);
            this.Name = "FormInitialise";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Initialising Collections";
            this.Load += new System.EventHandler(this.FormInitialise_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label labelMCTBold;
        private System.Windows.Forms.Label labelGCBold;
        private System.Windows.Forms.Label labelEtsyBold;
        private System.Windows.Forms.Label labelMCT;
        private System.Windows.Forms.Label labelGC;
        private System.Windows.Forms.Label labelEtsy;
    }
}