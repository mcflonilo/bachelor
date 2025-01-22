using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Accord;
using Telerik.WinControls.UI;
using UltraBend.Views;
using PostSharp.Patterns.Model;

namespace UltraBend.ViewForms
{
    public partial class ProjectView : UserControl, IProjectView
    {
        [WeakEvent] public event EventHandler ViewLoading;

        [WeakEvent] public event EventHandler<object> PropertySelected;

        [WeakEvent] public event EventHandler CreateNewUmbilical;

        [WeakEvent] public event EventHandler CreateNewDesign;

        [WeakEvent] public event EventHandler CreateNewMaterial;

        [WeakEvent] public event EventHandler CreateNewLoadContour;

        [WeakEvent] public event EventHandler CreateNewCase;

        [WeakEvent] public event EventHandler CreateNewStudy;

        [WeakEvent] public event EventHandler CloseProject;

        [WeakEvent] public event EventHandler<Guid> NodeSelected;

        protected RadContextMenu UmbilicalsMenu = new RadContextMenu();
        protected RadContextMenu MaterialsMenu = new RadContextMenu();
        protected RadContextMenu DesignsMenu = new RadContextMenu();
        protected RadContextMenu LoadContoursMenu = new RadContextMenu();
        protected RadContextMenu CasesMenu = new RadContextMenu();
        protected RadContextMenu StudiesMenu = new RadContextMenu();


        public ProjectView()
        {
            Dock = DockStyle.Fill;
            this.Load+=OnLoad;

            var newMaterialMenuItem = new RadMenuItem() { Text = "New Material", Image = Properties.Resources.NewFile_32px };
            newMaterialMenuItem.Click += (sender, args) => CreateNewMaterial?.Invoke(null, EventArgs.Empty);
            MaterialsMenu.Items.Add(newMaterialMenuItem);

            var newUmbilicalsMenuItem = new RadMenuItem() { Text = "New Umbilical", Image = Properties.Resources.Umbilical_32x };
            newUmbilicalsMenuItem.Click += (sender, args) => CreateNewUmbilical?.Invoke(null, EventArgs.Empty);
            UmbilicalsMenu.Items.Add(newUmbilicalsMenuItem);

            var newDesignMenuItem = new RadMenuItem() {Text = "New Design", Image = Properties.Resources.DesignMode_32px};
            newDesignMenuItem.Click += (sender, args) => CreateNewDesign?.Invoke(null, EventArgs.Empty);
            DesignsMenu.Items.Add(newDesignMenuItem);

            var newLoadContourMenuItem = new RadMenuItem() {Text = "New Load Contour", Image = Properties.Resources.StackedLineChart_24x};
            newLoadContourMenuItem.Click += (sender, args) => CreateNewLoadContour?.Invoke(null, EventArgs.Empty);
            LoadContoursMenu.Items.Add(newLoadContourMenuItem);

            var newCaseMenuItem = new RadMenuItem() { Text = "New Case", Image = Properties.Resources.VariableProperty_32x };
            newCaseMenuItem.Click += (sender, args) => CreateNewCase?.Invoke(null, EventArgs.Empty);
            CasesMenu.Items.Add(newCaseMenuItem);

            var newStudyMenuItem = new RadMenuItem() { Text = "New Study", Image = Properties.Resources.Watch_32x };
            newStudyMenuItem.Click += (sender, args) => CreateNewStudy?.Invoke(null, EventArgs.Empty);
            StudiesMenu.Items.Add(newStudyMenuItem);

            InitializeComponent();

            radTreeView1.AllowDefaultContextMenu = true;
            radTreeView1.NodeMouseDoubleClick += (sender, args) =>
            {
                var id = args.Node.Value as System.Guid?;
                if (id != null)
                {
                    NodeSelected?.Invoke(this, id.Value);
                }

            };
            radTreeView1.ContextMenuOpening += (sender, args) =>
            {
                if (args.Node.Name == "NodeMaterials")
                {
                    args.Cancel = true;
                    MaterialsMenu.Show(Cursor.Position);
                    return;
                }

                if (args.Node.Name == "NodeUmbilicals")
                {
                    args.Cancel = true;
                    UmbilicalsMenu.Show(Cursor.Position);
                    return;
                }

                if (args.Node.Name == "NodeDesigns")
                {
                    args.Cancel = true;
                    DesignsMenu.Show(Cursor.Position);
                    return;
                }

                if (args.Node.Name == "NodeLoadContours")
                {
                    args.Cancel = true;
                    LoadContoursMenu.Show(Cursor.Position);
                    return;
                }

                if (args.Node.Name == "NodeCases")
                {
                    args.Cancel = true;
                    CasesMenu.Show(Cursor.Position);
                    return;
                }

                if (args.Node.Name == "NodeProject")
                {
                    args.Cancel = true;
                    //ProjectMenu.Show(Cursor.Position);
                    return;
                }

                if (args.Node.Name == "NodeStudies")
                {
                    args.Cancel = true;
                    StudiesMenu.Show(Cursor.Position);
                    return;
                }

            };
            
        }

        private void OnLoad(object sender, EventArgs eventArgs)
        {
            OnViewLoading();
        }

        public void UpdateTreeView(List<Services.DomainObjects.Umbilical> umbilicals, List<Services.DomainObjects.Material> materials, List<Services.DomainObjects.Design> designs, List<Services.DomainObjects.LoadContour> LoadContours, List<Services.DomainObjects.Case> cases, List<Services.DomainObjects.Study> studies)
        {
            var rootNode = radTreeView1.Nodes.Where(n => n.Name == "NodeProject").FirstOrDefault();
            var materialsNode = rootNode.Nodes.Where(n => n.Name == "NodeMaterials").FirstOrDefault();
            var umbilicalsNode = rootNode.Nodes.Where(n => n.Name == "NodeUmbilicals").FirstOrDefault();
            var designsNode = rootNode.Nodes.Where(n => n.Name == "NodeDesigns").FirstOrDefault();
            var LoadContoursNode = rootNode.Nodes.Where(n => n.Name == "NodeLoadContours").FirstOrDefault();
            var casesNode = rootNode.Nodes.Where(n => n.Name == "NodeCases").FirstOrDefault();
            var studiesNode = rootNode.Nodes.Where(n => n.Name == "NodeStudies").FirstOrDefault();

            umbilicalsNode.Nodes.Clear();
            materialsNode.Nodes.Clear();
            designsNode.Nodes.Clear();
            LoadContoursNode.Nodes.Clear();
            casesNode.Nodes.Clear();
            studiesNode.Nodes.Clear();

            var umbilicalsNodes = umbilicals.Select(m => new RadTreeNode { Text = m.Name, Value = m.Id }).ToList();
            var materialsNodes = materials.Select(m => new RadTreeNode {Text = m.Name, Value = m.Id}).ToList();
            var designsNodes = designs.Select(d => new RadTreeNode {Text = d.Name, Value = d.Id}).ToList();
            var LoadContoursNodes = LoadContours.Select(d => new RadTreeNode {Text = d.Name, Value = d.Id}).ToList();
            var casesNodes = cases.Select(d => new RadTreeNode {Text = d.Name, Value = d.Id}).ToList();
            var studiesNodes = studies.Select(d => new RadTreeNode { Text = d.Name, Value = d.Id }).ToList();

            umbilicalsNode.Nodes.AddRange(umbilicalsNodes);
            materialsNode.Nodes.AddRange(materialsNodes);
            designsNode.Nodes.AddRange(designsNodes);
            LoadContoursNode.Nodes.AddRange(LoadContoursNodes);
            casesNode.Nodes.AddRange(casesNodes);
            studiesNode.Nodes.AddRange(studiesNodes);
        }

        public DialogResult ShowDialog()
        {
            throw new NotImplementedException();
        }
        protected virtual void OnViewLoading()
        {
            ViewLoading?.Invoke(this, EventArgs.Empty);
        }

        public void SetVisible(bool visible)
        {
            throw new NotImplementedException();
        }

        public bool GetVisible()
        {
            throw new NotImplementedException();
        }
    }
}
