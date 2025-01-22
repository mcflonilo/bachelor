
namespace Ultra_Bend
{
    partial class Main
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
            this.telerikMetroBlueTheme1 = new Telerik.WinControls.Themes.TelerikMetroBlueTheme();
            this.radButtonDesignOptimalBS = new Telerik.WinControls.UI.RadButton();
            this.radButtonCheckExistingBS = new Telerik.WinControls.UI.RadButton();
            this.radButtonDesignOptimal = new Telerik.WinControls.UI.RadButton();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            ((System.ComponentModel.ISupportInitialize)(this.radButtonDesignOptimalBS)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButtonCheckExistingBS)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButtonDesignOptimal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radButtonDesignOptimalBS
            // 
            this.radButtonDesignOptimalBS.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.radButtonDesignOptimalBS.Location = new System.Drawing.Point(12, 67);
            this.radButtonDesignOptimalBS.Name = "radButtonDesignOptimalBS";
            this.radButtonDesignOptimalBS.Size = new System.Drawing.Size(469, 63);
            this.radButtonDesignOptimalBS.TabIndex = 0;
            this.radButtonDesignOptimalBS.Text = "Design Optimal Bend Stiffener";
            this.radButtonDesignOptimalBS.Click += new System.EventHandler(this.radButtonDesignOptimalBS_Click);
            // 
            // radButtonCheckExistingBS
            // 
            this.radButtonCheckExistingBS.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.radButtonCheckExistingBS.Location = new System.Drawing.Point(12, 136);
            this.radButtonCheckExistingBS.Name = "radButtonCheckExistingBS";
            this.radButtonCheckExistingBS.Size = new System.Drawing.Size(469, 63);
            this.radButtonCheckExistingBS.TabIndex = 1;
            this.radButtonCheckExistingBS.Text = "Check If Existing Bend Stiffener Designs are Suitable";
            // 
            // radButtonDesignOptimal
            // 
            this.radButtonDesignOptimal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.radButtonDesignOptimal.Location = new System.Drawing.Point(12, 205);
            this.radButtonDesignOptimal.Name = "radButtonDesignOptimal";
            this.radButtonDesignOptimal.Size = new System.Drawing.Size(469, 63);
            this.radButtonDesignOptimal.TabIndex = 2;
            this.radButtonDesignOptimal.Text = "Design Optimal Bend Stiffener";
            // 
            // radLabel1
            // 
            this.radLabel1.Location = new System.Drawing.Point(12, 43);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(53, 18);
            this.radLabel1.TabIndex = 3;
            this.radLabel1.Text = "I want to:";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(493, 300);
            this.Controls.Add(this.radLabel1);
            this.Controls.Add(this.radButtonDesignOptimal);
            this.Controls.Add(this.radButtonCheckExistingBS);
            this.Controls.Add(this.radButtonDesignOptimalBS);
            this.Name = "Main";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.Text = "Ultra Bend";
            this.ThemeName = "TelerikMetroBlue";
            ((System.ComponentModel.ISupportInitialize)(this.radButtonDesignOptimalBS)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButtonCheckExistingBS)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButtonDesignOptimal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.Themes.TelerikMetroBlueTheme telerikMetroBlueTheme1;
        private Telerik.WinControls.UI.RadButton radButtonDesignOptimalBS;
        private Telerik.WinControls.UI.RadButton radButtonCheckExistingBS;
        private Telerik.WinControls.UI.RadButton radButtonDesignOptimal;
        private Telerik.WinControls.UI.RadLabel radLabel1;
    }
}