namespace UltraBend.ViewForms
{
    partial class ProjectView
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
            Telerik.WinControls.UI.RadTreeNode radTreeNode1 = new Telerik.WinControls.UI.RadTreeNode();
            Telerik.WinControls.UI.RadTreeNode radTreeNode2 = new Telerik.WinControls.UI.RadTreeNode();
            Telerik.WinControls.UI.RadTreeNode radTreeNode3 = new Telerik.WinControls.UI.RadTreeNode();
            Telerik.WinControls.UI.RadTreeNode radTreeNode4 = new Telerik.WinControls.UI.RadTreeNode();
            Telerik.WinControls.UI.RadTreeNode radTreeNode5 = new Telerik.WinControls.UI.RadTreeNode();
            Telerik.WinControls.UI.RadTreeNode radTreeNode6 = new Telerik.WinControls.UI.RadTreeNode();
            Telerik.WinControls.UI.RadTreeNode radTreeNode7 = new Telerik.WinControls.UI.RadTreeNode();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectView));
            this.radTreeView1 = new Telerik.WinControls.UI.RadTreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.radTreeView1)).BeginInit();
            this.SuspendLayout();
            // 
            // radTreeView1
            // 
            this.radTreeView1.BackColor = System.Drawing.SystemColors.Control;
            this.radTreeView1.Cursor = System.Windows.Forms.Cursors.Default;
            this.radTreeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radTreeView1.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.radTreeView1.ForeColor = System.Drawing.Color.Black;
            this.radTreeView1.ImageList = this.imageList1;
            this.radTreeView1.Location = new System.Drawing.Point(0, 0);
            this.radTreeView1.Name = "radTreeView1";
            radTreeNode1.Expanded = true;
            radTreeNode1.Image = ((System.Drawing.Image)(resources.GetObject("radTreeNode1.Image")));
            radTreeNode1.ImageKey = "";
            radTreeNode1.Name = "NodeProject";
            radTreeNode2.Expanded = true;
            radTreeNode2.Image = ((System.Drawing.Image)(resources.GetObject("radTreeNode2.Image")));
            radTreeNode2.ImageIndex = 1;
            radTreeNode2.Name = "NodeMaterials";
            radTreeNode2.Text = "Materials";
            radTreeNode3.Expanded = true;
            radTreeNode3.Image = ((System.Drawing.Image)(resources.GetObject("radTreeNode3.Image")));
            radTreeNode3.ImageIndex = 5;
            radTreeNode3.Name = "NodeUmbilicals";
            radTreeNode3.Text = "Umbilicals";
            radTreeNode4.Expanded = true;
            radTreeNode4.Image = ((System.Drawing.Image)(resources.GetObject("radTreeNode4.Image")));
            radTreeNode4.ImageIndex = 2;
            radTreeNode4.Name = "NodeDesigns";
            radTreeNode4.Text = "Bend Stiffeners";
            radTreeNode5.Expanded = true;
            radTreeNode5.Image = ((System.Drawing.Image)(resources.GetObject("radTreeNode5.Image")));
            radTreeNode5.ImageIndex = 6;
            radTreeNode5.Name = "NodeLoadContours";
            radTreeNode5.Text = "Load Contours";
            radTreeNode6.Expanded = true;
            radTreeNode6.Image = ((System.Drawing.Image)(resources.GetObject("radTreeNode6.Image")));
            radTreeNode6.ImageIndex = 3;
            radTreeNode6.Name = "NodeCases";
            radTreeNode6.Text = "Cases";
            radTreeNode7.Expanded = true;
            radTreeNode7.Image = ((System.Drawing.Image)(resources.GetObject("radTreeNode7.Image")));
            radTreeNode7.ImageIndex = 4;
            radTreeNode7.Name = "NodeStudies";
            radTreeNode7.Text = "Studies";
            radTreeNode1.Nodes.AddRange(new Telerik.WinControls.UI.RadTreeNode[] {
            radTreeNode2,
            radTreeNode3,
            radTreeNode4,
            radTreeNode5,
            radTreeNode6,
            radTreeNode7});
            radTreeNode1.Text = "Project";
            this.radTreeView1.Nodes.AddRange(new Telerik.WinControls.UI.RadTreeNode[] {
            radTreeNode1});
            this.radTreeView1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.radTreeView1.ShowLines = true;
            this.radTreeView1.Size = new System.Drawing.Size(482, 632);
            this.radTreeView1.SpacingBetweenNodes = -1;
            this.radTreeView1.TabIndex = 0;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "application_32.png");
            this.imageList1.Images.SetKeyName(1, "SelectTable_32px.png");
            this.imageList1.Images.SetKeyName(2, "DesignMode_32px.png");
            this.imageList1.Images.SetKeyName(3, "VariableProperty_32x.png");
            this.imageList1.Images.SetKeyName(4, "Watch_32x.png");
            this.imageList1.Images.SetKeyName(5, "ParentChildRelationship_32x.png");
            this.imageList1.Images.SetKeyName(6, "StackedLineChart_24x.png");
            // 
            // ProjectView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.radTreeView1);
            this.Name = "ProjectView";
            this.Size = new System.Drawing.Size(482, 632);
            ((System.ComponentModel.ISupportInitialize)(this.radTreeView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadTreeView radTreeView1;
        private System.Windows.Forms.ImageList imageList1;
    }
}
