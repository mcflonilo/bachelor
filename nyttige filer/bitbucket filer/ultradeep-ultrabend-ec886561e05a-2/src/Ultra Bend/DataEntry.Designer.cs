namespace Ultra_Bend
{
    partial class DataEntry
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
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.radButtonOK = new Telerik.WinControls.UI.RadButton();
            this.radButtonCancel = new Telerik.WinControls.UI.RadButton();
            this.radPropertyGrid = new Telerik.WinControls.UI.RadPropertyGrid();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButtonOK)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButtonCancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPropertyGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radLabel1
            // 
            this.radLabel1.Location = new System.Drawing.Point(12, 28);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(105, 18);
            this.radLabel1.TabIndex = 5;
            this.radLabel1.Text = "Project Information:";
            // 
            // radButtonOK
            // 
            this.radButtonOK.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.radButtonOK.Location = new System.Drawing.Point(12, 375);
            this.radButtonOK.Name = "radButtonOK";
            this.radButtonOK.Size = new System.Drawing.Size(459, 63);
            this.radButtonOK.TabIndex = 12;
            this.radButtonOK.Text = "OK";
            this.radButtonOK.Click += new System.EventHandler(this.radButtonOK_Click);
            // 
            // radButtonCancel
            // 
            this.radButtonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.radButtonCancel.Location = new System.Drawing.Point(12, 444);
            this.radButtonCancel.Name = "radButtonCancel";
            this.radButtonCancel.Size = new System.Drawing.Size(459, 63);
            this.radButtonCancel.TabIndex = 11;
            this.radButtonCancel.Text = "Cancel";
            this.radButtonCancel.Click += new System.EventHandler(this.radButtonCancel_Click);
            // 
            // radPropertyGrid
            // 
            this.radPropertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.radPropertyGrid.ItemHeight = 28;
            this.radPropertyGrid.Location = new System.Drawing.Point(12, 52);
            this.radPropertyGrid.Name = "radPropertyGrid";
            this.radPropertyGrid.PropertySort = System.Windows.Forms.PropertySort.CategorizedAlphabetical;
            this.radPropertyGrid.Size = new System.Drawing.Size(459, 317);
            this.radPropertyGrid.SortOrder = System.Windows.Forms.SortOrder.Ascending;
            this.radPropertyGrid.TabIndex = 13;
            this.radPropertyGrid.ToolbarVisible = true;
            // 
            // DataEntry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(483, 519);
            this.Controls.Add(this.radPropertyGrid);
            this.Controls.Add(this.radButtonOK);
            this.Controls.Add(this.radButtonCancel);
            this.Controls.Add(this.radLabel1);
            this.Name = "DataEntry";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.Text = "Project Information";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DataEntry_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButtonOK)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButtonCancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPropertyGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadButton radButtonOK;
        private Telerik.WinControls.UI.RadButton radButtonCancel;
        private Telerik.WinControls.UI.RadPropertyGrid radPropertyGrid;
    }
}
