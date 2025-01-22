namespace UltraBend.ViewForms
{
    partial class BendStiffenerDesignView
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
            Telerik.WinControls.UI.RadTreeNode radTreeNode1 = new Telerik.WinControls.UI.RadTreeNode();
            this.radSplitContainer1 = new Telerik.WinControls.UI.RadSplitContainer();
            this.splitPanel1 = new Telerik.WinControls.UI.SplitPanel();
            this.radTreeView1 = new Telerik.WinControls.UI.RadTreeView();
            this.splitPanel2 = new Telerik.WinControls.UI.SplitPanel();
            this.radDiagram1 = new Telerik.WinControls.UI.RadDiagram();
            ((System.ComponentModel.ISupportInitialize)(this.radSplitContainer1)).BeginInit();
            this.radSplitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitPanel1)).BeginInit();
            this.splitPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radTreeView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitPanel2)).BeginInit();
            this.splitPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radDiagram1)).BeginInit();
            this.SuspendLayout();
            // 
            // radSplitContainer1
            // 
            this.radSplitContainer1.Controls.Add(this.splitPanel1);
            this.radSplitContainer1.Controls.Add(this.splitPanel2);
            this.radSplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radSplitContainer1.Location = new System.Drawing.Point(0, 0);
            this.radSplitContainer1.Name = "radSplitContainer1";
            // 
            // 
            // 
            this.radSplitContainer1.RootElement.MinSize = new System.Drawing.Size(25, 25);
            this.radSplitContainer1.Size = new System.Drawing.Size(700, 575);
            this.radSplitContainer1.TabIndex = 0;
            this.radSplitContainer1.TabStop = false;
            // 
            // splitPanel1
            // 
            this.splitPanel1.Controls.Add(this.radTreeView1);
            this.splitPanel1.Location = new System.Drawing.Point(0, 0);
            this.splitPanel1.Name = "splitPanel1";
            // 
            // 
            // 
            this.splitPanel1.RootElement.MinSize = new System.Drawing.Size(25, 25);
            this.splitPanel1.Size = new System.Drawing.Size(348, 575);
            this.splitPanel1.TabIndex = 0;
            this.splitPanel1.TabStop = false;
            this.splitPanel1.Text = "splitPanel1";
            // 
            // radTreeView1
            // 
            this.radTreeView1.AllowDragDrop = true;
            this.radTreeView1.BackColor = System.Drawing.SystemColors.Control;
            this.radTreeView1.Cursor = System.Windows.Forms.Cursors.Default;
            this.radTreeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radTreeView1.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.radTreeView1.ForeColor = System.Drawing.Color.Black;
            this.radTreeView1.Location = new System.Drawing.Point(0, 0);
            this.radTreeView1.Name = "radTreeView1";
            radTreeNode1.Name = "NodeSections";
            radTreeNode1.Text = "Bend Stiffener Sections";
            this.radTreeView1.Nodes.AddRange(new Telerik.WinControls.UI.RadTreeNode[] {
            radTreeNode1});
            this.radTreeView1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.radTreeView1.ShowLines = true;
            this.radTreeView1.Size = new System.Drawing.Size(348, 575);
            this.radTreeView1.SpacingBetweenNodes = -1;
            this.radTreeView1.TabIndex = 0;
            this.radTreeView1.DragEnding += new Telerik.WinControls.UI.RadTreeView.DragEndingHandler(this.radTreeView1_DragEnding);
            this.radTreeView1.DragEnded += new Telerik.WinControls.UI.RadTreeView.DragEndedHandler(this.radTreeView1_DragEnded);
            this.radTreeView1.SelectedNodeChanged += new Telerik.WinControls.UI.RadTreeView.RadTreeViewEventHandler(this.radTreeView1_SelectedNodeChanged);
            // 
            // splitPanel2
            // 
            this.splitPanel2.Controls.Add(this.radDiagram1);
            this.splitPanel2.Location = new System.Drawing.Point(352, 0);
            this.splitPanel2.Name = "splitPanel2";
            // 
            // 
            // 
            this.splitPanel2.RootElement.MinSize = new System.Drawing.Size(25, 25);
            this.splitPanel2.Size = new System.Drawing.Size(348, 575);
            this.splitPanel2.TabIndex = 1;
            this.splitPanel2.TabStop = false;
            this.splitPanel2.Text = "splitPanel2";
            // 
            // radDiagram1
            // 
            this.radDiagram1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radDiagram1.IsBackgroundSurfaceVisible = false;
            this.radDiagram1.IsConnectorsManipulationEnabled = false;
            this.radDiagram1.IsDraggingEnabled = false;
            this.radDiagram1.IsEditable = false;
            this.radDiagram1.IsInformationAdornerVisible = false;
            this.radDiagram1.IsManipulationAdornerVisible = false;
            this.radDiagram1.IsPanEnabled = false;
            this.radDiagram1.IsResizingEnabled = false;
            this.radDiagram1.IsRotationEnabled = false;
            this.radDiagram1.IsSettingsPaneEnabled = false;
            this.radDiagram1.IsSnapToGridEnabled = false;
            this.radDiagram1.IsSnapToItemsEnabled = false;
            this.radDiagram1.IsZoomEnabled = false;
            this.radDiagram1.Location = new System.Drawing.Point(0, 0);
            this.radDiagram1.Name = "radDiagram1";
            this.radDiagram1.Size = new System.Drawing.Size(348, 575);
            this.radDiagram1.TabIndex = 0;
            this.radDiagram1.Text = "radDiagram1";
            this.radDiagram1.SizeChanged += new System.EventHandler(this.radDiagram1_SizeChanged);
            // 
            // BendStiffenerDesignView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.radSplitContainer1);
            this.Name = "BendStiffenerDesignView";
            this.Size = new System.Drawing.Size(700, 575);
            this.Load += new System.EventHandler(this.Design_Load);
            ((System.ComponentModel.ISupportInitialize)(this.radSplitContainer1)).EndInit();
            this.radSplitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitPanel1)).EndInit();
            this.splitPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.radTreeView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitPanel2)).EndInit();
            this.splitPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.radDiagram1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadSplitContainer radSplitContainer1;
        private Telerik.WinControls.UI.SplitPanel splitPanel1;
        private Telerik.WinControls.UI.SplitPanel splitPanel2;
        private Telerik.WinControls.UI.RadDiagram radDiagram1;
        private Telerik.WinControls.UI.RadTreeView radTreeView1;
    }
}
