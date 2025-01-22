namespace UltraBend.ViewForms.Modals
{
    partial class CapacityCurves
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
            this.radChartView1 = new Telerik.WinControls.UI.RadChartView();
            ((System.ComponentModel.ISupportInitialize)(this.radChartView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radChartView1
            // 
            this.radChartView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            cartesianArea1.GridDesign.AlternatingHorizontalColor = false;
            cartesianArea1.GridDesign.AlternatingVerticalColor = false;
            cartesianArea1.ShowGrid = true;
            this.radChartView1.AreaDesign = cartesianArea1;
            this.radChartView1.Location = new System.Drawing.Point(12, 12);
            this.radChartView1.Name = "radChartView1";
            this.radChartView1.ShowLegend = true;
            this.radChartView1.ShowTitle = true;
            this.radChartView1.Size = new System.Drawing.Size(878, 659);
            this.radChartView1.TabIndex = 0;
            this.radChartView1.Title = "Umbilical Capacity Curves";
            // 
            // CapacityCurves
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(902, 683);
            this.Controls.Add(this.radChartView1);
            this.Name = "CapacityCurves";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.Text = "Capacity Curves";
            this.Load += new System.EventHandler(this.CapacityCurves_Load);
            ((System.ComponentModel.ISupportInitialize)(this.radChartView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadChartView radChartView1;
    }
}
