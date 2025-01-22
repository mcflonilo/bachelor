using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.Shapes;
using Telerik.WinControls.UI;
using Telerik.Windows.Diagrams.Core;
using UltraBend.Common.DomainObjects.Geometry;
using UltraBend.Controls;
using UltraBend.Services.DomainObjects;
using UltraBend.Views;
using PostSharp.Patterns.Model;

namespace UltraBend.ViewForms
{
    public partial class BendStiffenerDesignView : UserControl, IBendStiffenerDesignView
    {

        public BendStiffenerDesignView()
        {
            Dock = DockStyle.Fill;
            VisibleChanged += (sender, args) => OnViewVisibleChanged(this.Visible);
            InitializeComponent();
        }

        public void Draw(List<DesignSection> sections, DesignSection selectedSection)
        {
            radDiagram1.Items.Clear();
            radDiagram1.Update();

            radDiagram1.PerformLayout();

            sections = sections.OrderBy(i => i.Index).ToList();
            var currentY = 0.0;
            var textShapes = new List<RadDiagramTextShape>();
            var lengthTextShapes = new List<RadDiagramTextShape>();
            float scale = 100;

            var selectedSectionId = selectedSection?.Id;

            foreach (var section in sections)
            {
                var shape = new RadDiagramShape()
                {
                    ElementShape = new ConicShape((float)section.Length, (float)section.RootOuterDiameter, (float)section.TipOuterDiameter, scale),
                    BackColor = section.Id == selectedSectionId ? Color.Green : Color.Yellow,
                    BorderBrush = new SolidBrush(Color.Black),
                    DrawBorder = true,
                    Position = new Telerik.Windows.Diagrams.Core.Point(0, currentY),
                    Enabled = false,
                    IsEditable = false,
                    Height = scale * section.Length
                };


                var textShape = new RadDiagramTextShape()
                {
                    Text = section.Name,
                    Font = new Font("Arial", 15),
                    Position = new Telerik.Windows.Diagrams.Core.Point(0, currentY + section.Length * scale / 2.0 )
                };

                var lengthTextShape = new RadDiagramTextShape()
                {
                    Text = section.Length*1000 + " mm",
                    Font = new Font("Arial", 15),
                    Position = new Telerik.Windows.Diagrams.Core.Point(0, currentY + section.Length * scale / 2.0)
                };

                if (section.Material != null)
                    lengthTextShape.Text += $"\n{section.Material.Name}";

                currentY += section.Length * scale;

                radDiagram1.Items.Add(shape);

                textShapes.Add(textShape);
                radDiagram1.Items.Add(textShape);

                lengthTextShapes.Add(lengthTextShape);
                radDiagram1.Items.Add(lengthTextShape);
            }

            var boundsWithoutText = radDiagram1.DiagramElement.CalculateEnclosingBounds();

            using (var graphics = radDiagram1.CreateGraphics())
            {
                foreach (var textShape in textShapes)
                {
                    var textWidth = graphics.MeasureString(textShape.Text, textShape.Font);
                    textShape.Width = textWidth.Width + 25;
                    textShape.Alignment = ContentAlignment.MiddleRight;
                    textShape.AutoSize = false;
                    textShape.X = -(boundsWithoutText.Width / 2) - textWidth.Width - 25;
                    textShape.Y -= textShape.Height / 2;
                }

                foreach (var textShape in lengthTextShapes)
                {
                    var textWidth = graphics.MeasureString(textShape.Text, textShape.Font);
                    textShape.Width = textWidth.Width + 25;
                    textShape.Alignment = ContentAlignment.MiddleLeft;
                    textShape.AutoSize = false;
                    textShape.X = (boundsWithoutText.Width / 2);
                    textShape.Y -= textShape.Height / 2;
                }
            }
        }

        public void CenterDiagramOnComputedBounds()
        {
            if (radDiagram1.Items.Any())
            {
                // this method is bugged:
                radDiagram1.DiagramElement.CalculateEnclosingBounds();

                // calculate bounds manually
                var list = radDiagram1.Items.Select(i => i as IShape).ToList();
                if (list.Any())
                {
                    var bounds = new Rect(0,0,0,0);
                    foreach (var shape in list)
                    {
                        var shapeBounds = shape.Bounds;
                        bounds.Union(new Rect(shapeBounds.X, shapeBounds.Y, shapeBounds.Width, shapeBounds.Height));
                    }
                    radDiagram1.DiagramElement.BringIntoView(bounds, false);
                }

            }
        }

        public class ConicShape : ElementShape
        {
            public float Length { get; set; }
            public float RootDiameter { get; set; }
            public float TipDiameter { get; set; }
            public float Scale { get; set; }

            public ConicShape(float length, float rootDiameter, float tipDiameter, float scale)
            {
                Length = length;
                RootDiameter = rootDiameter;
                TipDiameter = tipDiameter;
                Scale = scale;
            }
            public override GraphicsPath CreatePath(Rectangle bounds)
            {
                var path = new GraphicsPath();
                path.AddPolygon(new PointF[] { new PointF(-Scale * RootDiameter/2, 0), new PointF(Scale * RootDiameter / 2, 0), new PointF(Scale * TipDiameter /2, Scale * Length), new PointF(-Scale * TipDiameter /2, Scale * Length) });
                return path;
            }
        }
        
        private void Design_Load(object sender, EventArgs e)
        {
            OnViewLoading();
        }

        public void UpdateTreeView(List<DesignSection> sections)
        {
            var rootNode = radTreeView1.Nodes["NodeSections"];
            rootNode.Nodes.Clear();
            foreach (var section in sections.OrderBy(i => i.Index).ToList())
            {
                rootNode.Nodes.Add(new RadTreeNode() {Value = section, Text = section.Name, AllowDrop = false});
            }
            rootNode.Expanded = true;
        }

        public void SetVisible(bool visible)
        {
            this.Visible = visible;
            OnViewVisibleChanged(visible);
        }

        private void radTreeView1_SelectedNodeChanged(object sender, RadTreeViewEventArgs e)
        {
            if (e.Node != null && e.Node.Value != null)
            {
                var selection = e.Node.Value as DesignSection;

                OnPropertySelected(selection);
            }
        }

        public void OnSizeChanged()
        {
            SizedChanged?.Invoke(this, EventArgs.Empty);
        }

        [WeakEvent]
        public event EventHandler SizedChanged;


        private void radDiagram1_SizeChanged(object sender, EventArgs e)
        {
            OnSizeChanged();

        }

        private void radTreeView1_DragEnding(object sender, RadTreeViewDragCancelEventArgs e)
        {
            if (e.TargetNode.Level != e.Node.Level)
            {
                e.Cancel = true;
            }
        }

        private void radTreeView1_DragEnded(object sender, RadTreeViewDragEventArgs e)
        {
            var sortedSections = new List<DesignSection>();
            var rootNode = radTreeView1.Nodes["NodeSections"];
            int index = 0;
            foreach (var node in rootNode.Nodes)
            {
                if (node.Value is DesignSection section)
                {
                    section.Index = index;
                    sortedSections.Add(section);
                    index++;
                }
            }
            OnSectionsSorted(sortedSections);
        }

        public void OnSectionsSorted(List<DesignSection> sections)
        {
            SectionsSorted?.Invoke(this, sections);
        }

        [WeakEvent]
        public event EventHandler<List<DesignSection>> SectionsSorted;

        protected virtual void OnViewLoading()
        {
            ViewLoading?.Invoke(this, EventArgs.Empty);
        }

        public DialogResult ShowDialog()
        {
            throw new NotImplementedException();
        }

        [WeakEvent]
        public event EventHandler ViewLoading;

        public void OnViewVisibleChanged(bool visible)
        {
            ViewVisibleChanged?.Invoke(this, visible);
        }

        [WeakEvent]
        public event EventHandler<bool> ViewVisibleChanged;

        public void OnPropertySelected(DesignSection selection)
        {
            PropertySelected?.Invoke(this, selection);
        }

        public bool GetVisible()
        {
            return this.Visible;
        }

        [WeakEvent]
        public event EventHandler<object> PropertySelected;
    }
}
