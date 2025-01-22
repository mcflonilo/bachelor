namespace UltraBend.ViewForms
{
    partial class CaseDesignView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.caseBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.radTextBox1 = new Telerik.WinControls.UI.RadTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.caseBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radTextBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // caseBindingSource
            // 
            this.caseBindingSource.DataSource = typeof(UltraBend.Services.DomainObjects.Case);
            // 
            // radTextBox1
            // 
            this.radTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.radTextBox1.AutoSize = false;
            this.radTextBox1.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radTextBox1.Location = new System.Drawing.Point(3, 3);
            this.radTextBox1.MaxLength = 0;
            this.radTextBox1.Multiline = true;
            this.radTextBox1.Name = "radTextBox1";
            this.radTextBox1.ReadOnly = true;
            this.radTextBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.radTextBox1.Size = new System.Drawing.Size(520, 472);
            this.radTextBox1.TabIndex = 0;
            this.radTextBox1.WordWrap = false;
            // 
            // CaseDesignView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.radTextBox1);
            this.Name = "CaseDesignView";
            this.Size = new System.Drawing.Size(523, 475);
            this.Load += new System.EventHandler(this.CaseDesignView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.caseBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radTextBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.BindingSource caseBindingSource;
        private Telerik.WinControls.UI.RadTextBox radTextBox1;
    }
}
