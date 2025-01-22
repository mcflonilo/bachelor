namespace Ultra_Bend
{
    partial class RunAnalysis
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
            this.radButtonChangeRiserModelParameters = new Telerik.WinControls.UI.RadButton();
            this.radButtonChangeFEAnalysisParameters = new Telerik.WinControls.UI.RadButton();
            this.radButtonCancel = new Telerik.WinControls.UI.RadButton();
            this.radButtonFindOptimalBS = new Telerik.WinControls.UI.RadButton();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButtonChangeRiserModelParameters)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButtonChangeFEAnalysisParameters)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButtonCancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButtonFindOptimalBS)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radLabel1
            // 
            this.radLabel1.Location = new System.Drawing.Point(12, 37);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(71, 18);
            this.radLabel1.TabIndex = 5;
            this.radLabel1.Text = "Run Analysis:";
            // 
            // radButtonChangeRiserModelParameters
            // 
            this.radButtonChangeRiserModelParameters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.radButtonChangeRiserModelParameters.Location = new System.Drawing.Point(12, 61);
            this.radButtonChangeRiserModelParameters.Name = "radButtonChangeRiserModelParameters";
            this.radButtonChangeRiserModelParameters.Size = new System.Drawing.Size(459, 63);
            this.radButtonChangeRiserModelParameters.TabIndex = 6;
            this.radButtonChangeRiserModelParameters.Text = "Change Riser Model Parameters";
            this.radButtonChangeRiserModelParameters.Click += new System.EventHandler(this.radButtonChangeRiserModelParameters_Click);
            // 
            // radButtonChangeFEAnalysisParameters
            // 
            this.radButtonChangeFEAnalysisParameters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.radButtonChangeFEAnalysisParameters.Location = new System.Drawing.Point(12, 130);
            this.radButtonChangeFEAnalysisParameters.Name = "radButtonChangeFEAnalysisParameters";
            this.radButtonChangeFEAnalysisParameters.Size = new System.Drawing.Size(459, 63);
            this.radButtonChangeFEAnalysisParameters.TabIndex = 7;
            this.radButtonChangeFEAnalysisParameters.Text = "Change Finite Element Analysis Parameters";
            this.radButtonChangeFEAnalysisParameters.Click += new System.EventHandler(this.radButtonChangeFEAnalysisParameters_Click);
            // 
            // radButtonCancel
            // 
            this.radButtonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.radButtonCancel.Location = new System.Drawing.Point(12, 438);
            this.radButtonCancel.Name = "radButtonCancel";
            this.radButtonCancel.Size = new System.Drawing.Size(459, 63);
            this.radButtonCancel.TabIndex = 10;
            this.radButtonCancel.Text = "Cancel";
            this.radButtonCancel.Click += new System.EventHandler(this.radButtonCancel_Click);
            // 
            // radButtonFindOptimalBS
            // 
            this.radButtonFindOptimalBS.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.radButtonFindOptimalBS.Location = new System.Drawing.Point(12, 369);
            this.radButtonFindOptimalBS.Name = "radButtonFindOptimalBS";
            this.radButtonFindOptimalBS.Size = new System.Drawing.Size(459, 63);
            this.radButtonFindOptimalBS.TabIndex = 11;
            this.radButtonFindOptimalBS.Text = "Find Optimal Bend Stiffener!";
            this.radButtonFindOptimalBS.Click += new System.EventHandler(this.radButtonFindOptimalBS_Click);
            // 
            // RunAnalysis
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(483, 513);
            this.Controls.Add(this.radButtonFindOptimalBS);
            this.Controls.Add(this.radButtonCancel);
            this.Controls.Add(this.radButtonChangeFEAnalysisParameters);
            this.Controls.Add(this.radButtonChangeRiserModelParameters);
            this.Controls.Add(this.radLabel1);
            this.Name = "RunAnalysis";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.Text = "Run Analysis";
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButtonChangeRiserModelParameters)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButtonChangeFEAnalysisParameters)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButtonCancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButtonFindOptimalBS)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadButton radButtonChangeRiserModelParameters;
        private Telerik.WinControls.UI.RadButton radButtonChangeFEAnalysisParameters;
        private Telerik.WinControls.UI.RadButton radButtonCancel;
        private Telerik.WinControls.UI.RadButton radButtonFindOptimalBS;
    }
}
