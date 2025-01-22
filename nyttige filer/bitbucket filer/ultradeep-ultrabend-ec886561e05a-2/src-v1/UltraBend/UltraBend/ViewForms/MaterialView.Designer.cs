namespace UltraBend.ViewForms
{
    partial class MaterialView
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
            Telerik.WinControls.UI.TableViewDefinition tableViewDefinition1 = new Telerik.WinControls.UI.TableViewDefinition();
            this.radSplitContainer1 = new Telerik.WinControls.UI.RadSplitContainer();
            this.splitPanel1 = new Telerik.WinControls.UI.SplitPanel();
            this.splitPanel2 = new Telerik.WinControls.UI.SplitPanel();
            this.cartesianChart1 = new LiveCharts.WinForms.CartesianChart();
            this.csvGridMaterialData = new UltraBend.Controls.CsvGrid();
            ((System.ComponentModel.ISupportInitialize)(this.radSplitContainer1)).BeginInit();
            this.radSplitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitPanel1)).BeginInit();
            this.splitPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitPanel2)).BeginInit();
            this.splitPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.csvGridMaterialData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.csvGridMaterialData.MasterTemplate)).BeginInit();
            this.SuspendLayout();
            // 
            // radSplitContainer1
            // 
            this.radSplitContainer1.Controls.Add(this.splitPanel1);
            this.radSplitContainer1.Controls.Add(this.splitPanel2);
            this.radSplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radSplitContainer1.Location = new System.Drawing.Point(0, 0);
            this.radSplitContainer1.Name = "radSplitContainer1";
            this.radSplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // 
            // 
            this.radSplitContainer1.RootElement.MinSize = new System.Drawing.Size(25, 25);
            this.radSplitContainer1.Size = new System.Drawing.Size(927, 738);
            this.radSplitContainer1.TabIndex = 0;
            this.radSplitContainer1.TabStop = false;
            this.radSplitContainer1.Text = "radSplitContainer1";
            // 
            // splitPanel1
            // 
            this.splitPanel1.Controls.Add(this.csvGridMaterialData);
            this.splitPanel1.Location = new System.Drawing.Point(0, 0);
            this.splitPanel1.Name = "splitPanel1";
            // 
            // 
            // 
            this.splitPanel1.RootElement.MinSize = new System.Drawing.Size(25, 25);
            this.splitPanel1.Size = new System.Drawing.Size(927, 367);
            this.splitPanel1.TabIndex = 0;
            this.splitPanel1.TabStop = false;
            this.splitPanel1.Text = "splitPanel1";
            // 
            // splitPanel2
            // 
            this.splitPanel2.Controls.Add(this.cartesianChart1);
            this.splitPanel2.Location = new System.Drawing.Point(0, 371);
            this.splitPanel2.Name = "splitPanel2";
            // 
            // 
            // 
            this.splitPanel2.RootElement.MinSize = new System.Drawing.Size(25, 25);
            this.splitPanel2.Size = new System.Drawing.Size(927, 367);
            this.splitPanel2.TabIndex = 1;
            this.splitPanel2.TabStop = false;
            this.splitPanel2.Text = "splitPanel2";
            // 
            // cartesianChart1
            // 
            this.cartesianChart1.BackColor = System.Drawing.Color.White;
            this.cartesianChart1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cartesianChart1.Location = new System.Drawing.Point(0, 0);
            this.cartesianChart1.Name = "cartesianChart1";
            this.cartesianChart1.Size = new System.Drawing.Size(927, 367);
            this.cartesianChart1.TabIndex = 1;
            this.cartesianChart1.Text = "cartesianChart1";
            // 
            // csvGridMaterialData
            // 
            this.csvGridMaterialData.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.csvGridMaterialData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.csvGridMaterialData.EnterKeyMode = Telerik.WinControls.UI.RadGridViewEnterKeyMode.EnterMovesToNextCell;
            this.csvGridMaterialData.Location = new System.Drawing.Point(0, 0);
            // 
            // 
            // 
            this.csvGridMaterialData.MasterTemplate.AddNewRowPosition = Telerik.WinControls.UI.SystemRowPosition.Bottom;
            this.csvGridMaterialData.MasterTemplate.AllowCellContextMenu = false;
            this.csvGridMaterialData.MasterTemplate.AllowColumnChooser = false;
            this.csvGridMaterialData.MasterTemplate.AllowColumnHeaderContextMenu = false;
            this.csvGridMaterialData.MasterTemplate.AllowColumnReorder = false;
            this.csvGridMaterialData.MasterTemplate.AllowDragToGroup = false;
            this.csvGridMaterialData.MasterTemplate.AllowRowHeaderContextMenu = false;
            this.csvGridMaterialData.MasterTemplate.AllowRowResize = false;
            this.csvGridMaterialData.MasterTemplate.AutoGenerateColumns = false;
            this.csvGridMaterialData.MasterTemplate.AutoSizeColumnsMode = Telerik.WinControls.UI.GridViewAutoSizeColumnsMode.Fill;
            this.csvGridMaterialData.MasterTemplate.EnableAlternatingRowColor = true;
            this.csvGridMaterialData.MasterTemplate.EnableGrouping = false;
            this.csvGridMaterialData.MasterTemplate.MultiSelect = true;
            this.csvGridMaterialData.MasterTemplate.SelectionMode = Telerik.WinControls.UI.GridViewSelectionMode.CellSelect;
            this.csvGridMaterialData.MasterTemplate.ShowFilteringRow = false;
            this.csvGridMaterialData.MasterTemplate.ShowHeaderCellButtons = true;
            this.csvGridMaterialData.MasterTemplate.ViewDefinition = tableViewDefinition1;
            this.csvGridMaterialData.Name = "csvGridMaterialData";
            this.csvGridMaterialData.ShowGroupPanel = false;
            this.csvGridMaterialData.ShowHeaderCellButtons = true;
            this.csvGridMaterialData.Size = new System.Drawing.Size(927, 367);
            this.csvGridMaterialData.TabIndex = 2;
            this.csvGridMaterialData.Text = "csvGridMaterialData";
            this.csvGridMaterialData.ThemeName = "VisualStudio2012Light";
            // 
            // MaterialView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.radSplitContainer1);
            this.Name = "MaterialView";
            this.Size = new System.Drawing.Size(927, 738);
            this.Load += new System.EventHandler(this.MaterialView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.radSplitContainer1)).EndInit();
            this.radSplitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitPanel1)).EndInit();
            this.splitPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitPanel2)).EndInit();
            this.splitPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.csvGridMaterialData.MasterTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.csvGridMaterialData)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadSplitContainer radSplitContainer1;
        private Telerik.WinControls.UI.SplitPanel splitPanel1;
        private Telerik.WinControls.UI.SplitPanel splitPanel2;
        private Controls.CsvGrid csvGridMaterialData;
        private LiveCharts.WinForms.CartesianChart cartesianChart1;
    }
}
