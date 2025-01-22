namespace Ultra_Bend
{
    partial class CapacityCurveViewer
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
            Telerik.WinControls.UI.CartesianArea cartesianArea1 = new Telerik.WinControls.UI.CartesianArea();
            this.radChartViewCapacityCurve = new Telerik.WinControls.UI.RadChartView();
            ((System.ComponentModel.ISupportInitialize)(this.radChartViewCapacityCurve)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radChartViewCapacityCurve
            // 
            this.radChartViewCapacityCurve.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.radChartViewCapacityCurve.AreaDesign = cartesianArea1;
            this.radChartViewCapacityCurve.Location = new System.Drawing.Point(12, 12);
            this.radChartViewCapacityCurve.Name = "radChartViewCapacityCurve";
            this.radChartViewCapacityCurve.ShowGrid = false;
            this.radChartViewCapacityCurve.Size = new System.Drawing.Size(937, 552);
            this.radChartViewCapacityCurve.TabIndex = 0;
            // 
            // CapacityCurveViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(961, 576);
            this.Controls.Add(this.radChartViewCapacityCurve);
            this.Name = "CapacityCurveViewer";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.Text = "CapacityCurveViewer";
            ((System.ComponentModel.ISupportInitialize)(this.radChartViewCapacityCurve)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadChartView radChartViewCapacityCurve;
    }
}
