using PostSharp.Patterns.Model;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using Telerik.WinControls.UI.Docking;
using UltraBend.Common;
using UltraBend.Views;

namespace UltraBend.ViewForms
{
    public partial class MainView : RadRibbonForm, IMainView
    {
        public MainView()
        {
            InitializeComponent();

            radDock1.ActiveWindowChanged += (sender, args) => ActiveWindowChanged?.Invoke(sender, args.DockWindow);
        }

        [WeakEvent]
        public event EventHandler ViewLoading;

        [WeakEvent]
        public event EventHandler ViewClosing;

        [WeakEvent]
        public event EventHandler AddBendStiffenerSection;

        [WeakEvent]
        public event EventHandler RemoveActiveStiffenerSection;

        public void SetRecentProjects(StringCollection projectFiles)
        {
            while(radRibbonBar1.StartMenuRightColumnItems.Count > 2)
                radRibbonBar1.StartMenuRightColumnItems.RemoveAt(2);

            foreach (var file in projectFiles)
            {
                var item = new Telerik.WinControls.UI.RadMenuItem()
                {
                    Text = file.EllipsisString(120)
                };
                item.Click += (sender, args) => OpenProject?.Invoke(this, file);
                radRibbonBar1.StartMenuRightColumnItems.Add(item);
            }
        }

        [WeakEvent]
        public event EventHandler<string> PropertyGridValueChanged;

        [WeakEvent]
        public event EventHandler CreateNewDesign;

        [WeakEvent]
        public event EventHandler CreateNewMaterial;

        [WeakEvent] public event EventHandler CreateNewLoadContour;

        [WeakEvent]
        public event EventHandler CreateNewCase;

        [WeakEvent]
        public event EventHandler CreateNewStudy;

        [WeakEvent]
        public event EventHandler CreateNewProject;

        [WeakEvent]
        public event EventHandler<string> OpenProject;

        [WeakEvent]
        public event EventHandler SaveProject;

        [WeakEvent]
        public event EventHandler SaveProjectAs;

        [WeakEvent]
        public event EventHandler CloseProject;

        [WeakEvent]
        public event EventHandler StartSimulation;

        [WeakEvent]
        public event EventHandler StartAllSimulations;

        [WeakEvent] 
        public event EventHandler ShowCapacityCurves;

        [WeakEvent]
        public event EventHandler<DockWindow> ActiveWindowChanged;

        [WeakEvent]
        public event EventHandler Exit;

        public string Title
        {
            get => Text;
            set => Text = value;
        }

        public void UpdateButtonGroup(ButtonGroup buttonGroup) //, bool active)
        {
            ribbonTabDesign.Visibility = ElementVisibility.Hidden;
            ribbonTabCase.Visibility = ElementVisibility.Hidden;
            ribbonTabStudy.Visibility = ElementVisibility.Hidden;

            if ((buttonGroup & ButtonGroup.BendStiffenerDesign) == ButtonGroup.BendStiffenerDesign)
            {
                ribbonTabDesign.Visibility = ElementVisibility.Visible;
                ribbonTabDesign.IsSelected = true;
            }

            if ((buttonGroup & ButtonGroup.Case) == ButtonGroup.Case)
            {
                ribbonTabCase.Visibility = ElementVisibility.Visible;
                ribbonTabCase.IsSelected = true;
            }

            if ((buttonGroup & ButtonGroup.Study) == ButtonGroup.Study)
            {
                ribbonTabStudy.Visibility = ElementVisibility.Visible;
                ribbonTabStudy.IsSelected = true;
            }
        }


        public void AddDocumentWindow(DocumentWindow window)
        {
            radDock1.AddDocument(window);
            window.Select();
        }

        public bool HasDocumentWindow(DockWindow window)
        {
            return radDock1.DocumentManager.DocumentArray.Any(i => i == window);
        }

        public void SetOutputControl(UserControl outputControl)
        {
            ClearOutputControl();
            toolWindowOutput.Controls.Add(outputControl);
            outputControl.Dock = DockStyle.Fill;
        }

        public void ClearOutputControl()
        {
            toolWindowOutput.Controls.Clear();
        }

        public void SetProjectControl(UserControl projectControl)
        {
            radRibbonBar1.MaximumSize = new Size(0, 0);
            radRibbonBar1.RibbonBarElement.TabStripElement.Visibility = ElementVisibility.Visible;
            toolWindowProject.Controls.Add(projectControl);
            projectControl.Dock = DockStyle.Fill;
        }

        public void ClearProjectControl()
        {
            radRibbonBar1.MaximumSize = new Size(0, 60);
            radRibbonBar1.RibbonBarElement.TabStripElement.Visibility = ElementVisibility.Collapsed;
            //radRibbonBar1.CommandTabs.ForEach(t => t.Enabled = false);
            toolWindowProject.Controls.Clear();
        }


        public void ConfirmLoaded()
        {
            //RadMessageBox.Show("Loaded");
        }

        public void PropertySelect(object selectedObject)
        {
            try
            {
                if (radPropertyGrid1.SelectedObject != selectedObject)
                {
                    radPropertyGrid1.SelectedObject = selectedObject;
                    radPropertyGrid1.ExpandAllGridItems();
                }
                else
                {
                    radPropertyGrid1.SelectedObject = selectedObject;
                }
            }
            catch (Exception e)
            {
                e.Trace();
            }
        }

        public void RestoreWindow(Point point, Size size, bool maximized)
        {
            WindowState = maximized ? FormWindowState.Maximized : FormWindowState.Normal;

            if (size.Width > 0 && size.Height > 0)
            {
                Location = point;
                Size = size;
            }
        }

        public Tuple<Point, Size, bool> GetWindowState()
        {
            return new Tuple<Point, Size, bool>(Location, Size, WindowState == FormWindowState.Maximized);
        }


        private void radRibbonBar1_Click(object sender, EventArgs e)
        {
        }

        private void ProjectForm_Load(object sender, EventArgs e)
        {
            radButtonElementAddSection.Click += (o, args) => OnAddBendStiffenerSection();
            radButtonElementRemoveSection.Click += (o, args) => OnRemoveActiveStiffenerSection();
            radButtonElementNewMaterial.Click += (o, args) => OnCreateNewMaterial();
            radPropertyGrid1.PropertyValueChanged += (o, args) => OnPropertyGridValueChanged(((PropertyGridItem) args.Item).Category);

            OnViewLoading();
        }

        protected virtual void OnViewLoading()
        {
            ViewLoading?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnViewClosing()
        {
            ViewClosing?.Invoke(this, EventArgs.Empty);
        }

        public virtual void OnAddBendStiffenerSection()
        {
            AddBendStiffenerSection?.Invoke(this, EventArgs.Empty);
        }

        public virtual void OnRemoveActiveStiffenerSection()
        {
            RemoveActiveStiffenerSection?.Invoke(this, EventArgs.Empty);
        }

        public virtual void OnPropertyGridValueChanged(string category)
        {
            PropertyGridValueChanged?.Invoke(this, category);
        }

        public virtual void OnCreateNewDesign()
        {
            CreateNewDesign?.Invoke(this, EventArgs.Empty);
        }

        public virtual void OnCreateNewCase()
        {
            CreateNewCase?.Invoke(this, EventArgs.Empty);
        }

        public virtual void OnCreateNewStudy()
        {
            CreateNewStudy?.Invoke(this, EventArgs.Empty);
        }

        public virtual void OnCreateNewMaterial()
        {
            CreateNewMaterial?.Invoke(this, EventArgs.Empty);
        }

        private void radButtonElementDesign_Click(object sender, EventArgs e)
        {
            OnCreateNewDesign();
        }


        private void radMenuItemNewProject_Click(object sender, EventArgs e)
        {
            CreateNewProject?.Invoke(this, EventArgs.Empty);
        }


        private void radMenuItemOpenProject_Click(object sender, EventArgs e)
        {
            OpenProject?.Invoke(this, null);
        }

        private void radMenuItemSave_Click(object sender, EventArgs e)
        {
            SaveProject?.Invoke(this, EventArgs.Empty);
        }

        private void radMenuItemSaveAs_Click(object sender, EventArgs e)
        {
            SaveProjectAs?.Invoke(this, EventArgs.Empty);
        }

        private void radMenuItemClose_Click(object sender, EventArgs e)
        {
            CloseProject?.Invoke(this, EventArgs.Empty);
        }

        private void radMenuItemExit_Click(object sender, EventArgs e)
        {
            Exit?.Invoke(this, EventArgs.Empty);
        }

        private void radButtonElementStart_Click(object sender, EventArgs e)
        {
            StartSimulation?.Invoke(this, EventArgs.Empty);
        }

        private void MainView_FormClosing(object sender, FormClosingEventArgs e)
        {
            OnViewClosing();
        }

        private void radButtonElement2_Click(object sender, EventArgs e)
        {

        }

        public void SetVisible(bool visible)
        {
            throw new NotImplementedException();
        }

        public bool GetVisible()
        {
            throw new NotImplementedException();
        }

        private void radButtonElementStartAll_Click(object sender, EventArgs e)
        {
            StartAllSimulations?.Invoke(this, EventArgs.Empty);
        }

        private void radButtonElementCapacityCurves_Click(object sender, EventArgs e)
        {
            ShowCapacityCurves?.Invoke(this, EventArgs.Empty);
        }

        private void toolWindowOutput_Click(object sender, EventArgs e)
        {

        }
    }
}