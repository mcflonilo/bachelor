using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Nito.AsyncEx;
using PostSharp.Patterns.Caching;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using Telerik.WinControls.UI.Docking;
using UltraBend.Common.DomainObjects.Geometry;
using UltraBend.Common.MVP;
using UltraBend.Engine.BSEngine;
using UltraBend.Models;
using UltraBend.Properties;
using UltraBend.Services;
using UltraBend.Services.Project;
using UltraBend.ViewForms;
using UltraBend.Views;

namespace UltraBend.Presenters
{
    public class MainPresenter : Presenter<IMainView, MainViewModel>, IWindowPresenter
    {
        // References to windows (key) and various Presenters (value)
        protected Dictionary<DockWindow, IPresenter> DockWindowPresenters = new Dictionary<DockWindow, IPresenter>();

        /// <summary>
        /// Services owned by this presenter
        /// </summary>
        protected ProjectPresenter ProjectPresenter;
        protected OutputPresenter OutputPresenter;
        protected ProjectService ProjectService;
        protected UmbilicalsService UmbilicalsService;
        protected MaterialsService MaterialsService;
        protected DesignsService DesignsService;
        protected LoadContoursService LoadContoursService;
        protected CasesService CasesService;
        protected StudiesService StudiesService;
        protected OutputsService OutputsService;
        
        
        public string Title => "Ultra Bend";

        private Guid _id = Guid.NewGuid();
        public Guid Id => _id;

        public DockWindow Window { get; set; }
        

        public MainPresenter(IMainView view) : base(view)
        {
            this.Model = new MainViewModel();

            ProjectService = null;
            MaterialsService = null;
            DesignsService = null;
            LoadContoursService = null;
            CasesService = null;
            StudiesService = null;
            OutputsService = null;

            View.ViewLoading += View_ViewLoading;
            View.ViewClosing += ViewOnViewClosing;

            View.CreateNewDesign += (sender, args) => CreateNewBendStiffenerDesignerWindow(null);
            View.CreateNewMaterial += (sender, args) => CreateNewMaterialWindow(null);
            View.CreateNewLoadContour += (sender, args) => CreateNewLoadContourWindow(null);
            View.CreateNewCase += (sender, args) => CreateNewCaseWindow(null);
            View.CreateNewStudy += (sender, args) => CreateNewStudyWindow(null);
            
            View.CreateNewProject += (sender, args) => CreateNewProject();
            View.OpenProject += (sender, args) => OpenProject(args);
            View.SaveProject += (sender, args) => SaveProject();
            View.SaveProjectAs += (sender, args) => SaveProjectAs();
            View.CloseProject += (sender, args) => CloseProject();
            View.Exit += (sender, args) => Exit();


            View.ActiveWindowChanged += ViewOnActiveWindowChanged;

            ApplicationState.ProjectHasChangesChanged += (sender, args) => View.Title = ApplicationState.ProjectHasChanges ? Title + "*" : Title;

            // default to closed state
            CloseProject();
        }


        /// <summary>
        /// Update the property grid to edit the active window if it has properties
        /// </summary>
        /// <param name="sender1"></param>
        /// <param name="dockWindow"></param>
        private void ViewOnActiveWindowChanged(object sender1, DockWindow dockWindow)
        {
            if (DockWindowPresenters.ContainsKey(dockWindow))
            {
                var title = DockWindowPresenters[dockWindow] as IWindowPresenter;
                if (title != null)
                {
                    dockWindow.Text = title.Title;
                }

                var presenter = DockWindowPresenters[dockWindow] as IPropertySelectable;
                if (presenter != null)
                {
                    View.PropertySelect(presenter.GetProperty());
                }
                else
                {
                    View.PropertySelect(null);
                }

                var buttons = DockWindowPresenters[dockWindow] as IButtonGroup;
                if (buttons != null)
                {
                    View.UpdateButtonGroup(buttons.GetButtonGroup());
                }
                else
                {
                    View.UpdateButtonGroup(ButtonGroup.Default);
                }
            }
        }

        private void ActivateWindowById(Guid id)
        {
            var presenter = DockWindowPresenters.Values.Select(k => k as IWindowPresenter).Where(k => k != null).Where(k => k.Id == id).FirstOrDefault();
            if (presenter != null && View.HasDocumentWindow(presenter.Window as DockWindow))
            {
                //presenter.SetVisible(true);
                //View.AddDocumentWindow(presenter.Window as DocumentWindow);
                presenter.Window.Select();
            }
            else
            {
                var umbilical = UmbilicalsService.GetUmbilicalById(id);
                if (umbilical != null)
                {
                    CreateNewUmbilicalWindow(id);
                    return;
                }

                var design = DesignsService.GetDesignById(id);
                if (design != null)
                {
                    CreateNewBendStiffenerDesignerWindow(id);
                    return;
                }

                var material = MaterialsService.GetMaterialById(id);
                if (material != null)
                {
                    CreateNewMaterialWindow(id);
                    return;
                }

                var LoadContour = LoadContoursService.GetLoadContourById(id);
                if (LoadContour != null)
                {
                    CreateNewLoadContourWindow(id);
                    return;
                }

                var @case = CasesService.GetCaseById(id);
                if (@case != null)
                {
                    CreateNewCaseWindow(id);
                    return;
                }

                var study = StudiesService.GetStudyById(id);
                if (study != null)
                {
                    CreateNewStudyWindow(id);
                    return;
                }
            }
        }
        

        private void CreateNewProject()
        {
            if (!CloseProject()) return;

            ApplicationState.HasProject = true;
            ApplicationState.ProjectHasChanges = false;
            ApplicationState.CurrentProjectTemporaryFile = Path.GetTempFileName();
            ApplicationState.CurrentProjectFile = null;

            ProjectService = new ProjectService(ApplicationState.CurrentProjectTemporaryFile, true);
            UmbilicalsService = new UmbilicalsService(ApplicationState.CurrentProjectTemporaryFile);
            MaterialsService = new MaterialsService(ApplicationState.CurrentProjectTemporaryFile);
            DesignsService = new DesignsService(ApplicationState.CurrentProjectTemporaryFile);
            LoadContoursService = new LoadContoursService(ApplicationState.CurrentProjectTemporaryFile);
            CasesService = new CasesService(ApplicationState.CurrentProjectTemporaryFile);
            StudiesService = new StudiesService(ApplicationState.CurrentProjectTemporaryFile);
            OutputsService = new OutputsService();
            OutputsService.AddChannel("General");
            

            ProjectPresenter = new ProjectPresenter(new ProjectView(), ProjectService, UmbilicalsService, MaterialsService, DesignsService, LoadContoursService, CasesService, StudiesService);
            ProjectPresenter.CreateNewUmbilical += (sender, args) => CreateNewUmbilical();
            ProjectPresenter.CreateNewDesign += (sender, args) => CreateNewDesign();
            ProjectPresenter.CreateNewMaterial += (sender, args) => CreateNewMaterial();
            ProjectPresenter.CreateNewLoadContour += (sender, args) => CreateNewLoadContour();
            ProjectPresenter.CreateNewCase += (sender, args) => CreateNewCase();
            ProjectPresenter.CreateNewStudy += (sender, args) => CreateNewStudy();
            ProjectPresenter.NodeSelected += (sender, guid) => ActivateWindowById(guid);
            
            View.SetProjectControl(ProjectPresenter.View as UserControl);
            
            OutputPresenter = new OutputPresenter(new OutputView(), OutputsService);

            View.SetOutputControl(OutputPresenter.View as UserControl);

            OutputsService.PublishLineToChannel("General", "New Project Created");
        }

        private void CreateNewUmbilical()
        {
            CreateNewUmbilicalWindow(null);
        }

        private void CreateNewMaterial()
        {
            CreateNewMaterialWindow(null);
        }

        private void CreateNewDesign()
        {
            CreateNewBendStiffenerDesignerWindow(null);
        }

        private void CreateNewLoadContour()
        {
            CreateNewLoadContourWindow(null);
        }

        private void CreateNewCase()
        {
            CreateNewCaseWindow(null);
        }

        private void CreateNewStudy()
        {
            CreateNewStudyWindow(null);
        }

        private void OpenProject(string fileName)
        {
            if (!CloseProject()) return;

            if (string.IsNullOrWhiteSpace(fileName))
            {

                var dialog = new OpenFileDialog();
                dialog.Filter = "Ultra Bend Project (*.ubproj)|*.ubproj|All files (*.*)|*.*";
                dialog.Title = "Select a ubproj file";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    fileName = dialog.FileName;
                }
            }

            if (!string.IsNullOrWhiteSpace(fileName) && File.Exists(fileName))
            {
                ApplicationState.CurrentProjectTemporaryFile = Path.GetTempFileName();
                ProjectService = new ProjectService(fileName, false);
                ProjectService.SaveAs(ApplicationState.CurrentProjectTemporaryFile, true);

                ApplicationState.HasProject = true;
                ApplicationState.ProjectHasChanges = false;
                ApplicationState.CurrentProjectFile = fileName;
                ProjectService = new ProjectService(ApplicationState.CurrentProjectTemporaryFile, false);
                UmbilicalsService = new UmbilicalsService(ApplicationState.CurrentProjectTemporaryFile);
                MaterialsService = new MaterialsService(ApplicationState.CurrentProjectTemporaryFile);
                DesignsService = new DesignsService(ApplicationState.CurrentProjectTemporaryFile);
                LoadContoursService = new LoadContoursService(ApplicationState.CurrentProjectTemporaryFile);
                CasesService = new CasesService(ApplicationState.CurrentProjectTemporaryFile);
                StudiesService = new StudiesService(ApplicationState.CurrentProjectTemporaryFile);
                OutputsService = new OutputsService();
                OutputsService.AddChannel("General");

                ProjectPresenter = new ProjectPresenter(new ProjectView(), ProjectService, UmbilicalsService, MaterialsService,
                    DesignsService, LoadContoursService, CasesService, StudiesService);
                ProjectPresenter.CreateNewDesign += (sender, args) => CreateNewDesign();
                ProjectPresenter.CreateNewUmbilical += (sender, args) => CreateNewUmbilical();
                ProjectPresenter.CreateNewMaterial += (sender, args) => CreateNewMaterial();
                ProjectPresenter.CreateNewLoadContour += (sender, args) => CreateNewLoadContour();
                ProjectPresenter.CreateNewCase += (sender, args) => CreateNewCase();
                ProjectPresenter.CreateNewStudy += (sender, args) => CreateNewStudy();
                ProjectPresenter.NodeSelected += (sender, guid) => ActivateWindowById(guid);
                View.SetProjectControl(ProjectPresenter.View as UserControl);

                OutputPresenter = new OutputPresenter(new OutputView(), OutputsService);

                View.SetOutputControl(OutputPresenter.View as UserControl);
                
                if (Settings.Default.RecentProjects == null)
                    Settings.Default.RecentProjects = new StringCollection();
                Settings.Default.RecentProjects.Insert(0,fileName);
                Settings.Default.Save();
                
                UpdateRecentProjects();
            }
        }

        private bool SaveProjectAs()
        {
            if (ProjectService != null)
            {
                var dialog = new SaveFileDialog();
                dialog.Filter = "Ultra Bend Project (*.ubproj)|*.ubproj|All files (*.*)|*.*";
                dialog.Title = "Select a ubproj file";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    ApplicationState.CurrentProjectFile = dialog.FileName;
                    ProjectService.SaveAs(ApplicationState.CurrentProjectFile, true);
                    ApplicationState.ProjectHasChanges = false;
                    return true;
                }
            }

            return false;
        }

        private bool SaveProject()
        {
            if (ProjectService != null)
            {
                if (!String.IsNullOrEmpty(ApplicationState.CurrentProjectFile))
                {
                    ProjectService.SaveAs(ApplicationState.CurrentProjectFile, true);
                    ApplicationState.ProjectHasChanges = false;
                    return true;
                }
                else
                {
                    return SaveProjectAs();
                }
            }

            return false;
        }

        private bool CloseProject()
        {
            if (!ConfirmSaveProjectIfNeeded()) return false;

            ApplicationState.HasProject = false;
            ApplicationState.ProjectHasChanges = false;
            ApplicationState.CurrentProjectTemporaryFile = null;
            ApplicationState.CurrentProjectFile = null;

            foreach (var dockWindowPresenter in DockWindowPresenters)
            {
                dockWindowPresenter.Value.Dispose();
            }

            View.ClearProjectControl();

            ProjectService = null;
            ProjectPresenter = null;
            OutputPresenter = null;

            View.PropertySelect(null);

            foreach (var dockWindow in DockWindowPresenters.Keys)
            {
                dockWindow.Close();
            }
            DockWindowPresenters = new Dictionary<DockWindow, IPresenter>();

            CachingServices.DefaultBackend.Clear();

            return true;
        }

        private void Exit()
        {
            if (!ConfirmSaveProjectIfNeeded()) return;
            Application.Exit();
        }


        private bool ConfirmSaveProjectIfNeeded()
        {
            if (ApplicationState.ProjectHasChanges)
            {
                var result = RadMessageBox.Show("Want to save changes to your project?", "UltraBend", MessageBoxButtons.YesNoCancel);

                if (result == DialogResult.Yes)
                {
                    return SaveProject();
                }

                if (result == DialogResult.Cancel)
                {
                    return false;
                }
            }

            return true;
        }

        private void View_ViewLoading(object sender, EventArgs e)
        {
            View.RestoreWindow(Settings.Default.WindowLocation, Settings.Default.WindowSize, Settings.Default.WindowMaximized);

            UpdateRecentProjects();
            View.ConfirmLoaded();
        }

        private void UpdateRecentProjects()
        {
            if (Settings.Default.RecentProjects == null)
                Settings.Default.RecentProjects = new StringCollection();

            var recentProjects = Settings.Default.RecentProjects.Cast<string>().ToList();
            foreach (var project in recentProjects.Where(p => !File.Exists(p)).ToList())
            {
                Settings.Default.RecentProjects.Remove(project);
            }

            var distinct = Settings.Default.RecentProjects.Cast<string>().Distinct().ToArray();
            Settings.Default.RecentProjects.Clear();
            Settings.Default.RecentProjects.AddRange(distinct);
            Settings.Default.Save();

            View.SetRecentProjects(Settings.Default.RecentProjects);
        }

        private void ViewOnViewClosing(object sender1, EventArgs eventArgs)
        {
            var window = View.GetWindowState();
            Settings.Default.WindowLocation = window.Item1;
            Settings.Default.WindowMaximized = window.Item3;
            Settings.Default.WindowSize = window.Item2;
            Settings.Default.Save();
        }

        private void CreateNewUmbilicalWindow(Guid? id)
        {
            var newDocumentWindow = new DocumentWindow("Umbilical");
            newDocumentWindow.ParentChanged += (sender, args) =>
            {
                if (newDocumentWindow.FloatingParent != null)
                    newDocumentWindow.FloatingParent.Icon = Resources.Watch;
            };
            var umbilicalPresenter = new UmbilicalPresenter(new UmbilicalView(), id, new UmbilicalsService(ApplicationState.CurrentProjectTemporaryFile));

            DockWindowPresenters.Add(newDocumentWindow, umbilicalPresenter);

            View.PropertyGridValueChanged += (o, args) =>
            {
                if (umbilicalPresenter.Active)
                    umbilicalPresenter.Bind();
                newDocumentWindow.Text = umbilicalPresenter.Title;
                ProjectPresenter.Update();
            };

            newDocumentWindow.Controls.Add(umbilicalPresenter.View as UserControl);
            newDocumentWindow.VisibleChanged += (o, args) => umbilicalPresenter.SetVisible(newDocumentWindow.Visible);
            newDocumentWindow.HandleDestroyed += (o, args) =>
            {
                umbilicalPresenter.SetVisible(false);
            };

            umbilicalPresenter.Window = newDocumentWindow;
            umbilicalPresenter.PropertySelected += (sender, o) =>
            {
                View.PropertySelect(o);
            };

            View.AddDocumentWindow(newDocumentWindow);
        }

        private void CreateNewStudyWindow(Guid? id)
        {
            var newDocumentWindow = new DocumentWindow("Bend Stiffener Study");
            newDocumentWindow.ParentChanged += (sender, args) =>
            {
                if (newDocumentWindow.FloatingParent != null)
                    newDocumentWindow.FloatingParent.Icon = Resources.Watch;
            };
            var studyPresenter = new StudyPresenter(new StudyView(), id, new StudiesService(ApplicationState.CurrentProjectTemporaryFile), OutputsService, new ResultsService(ApplicationState.CurrentProjectTemporaryFile));
            
            DockWindowPresenters.Add(newDocumentWindow, studyPresenter);

            View.StartAllSimulations += (sender, args) => studyPresenter.StartAllSimulations();
            View.ShowCapacityCurves += (sender, args) => studyPresenter.ShowCapacityCurves();

            View.PropertyGridValueChanged += (o, args) =>
            {
                if (studyPresenter.Active)
                    studyPresenter.Bind();
                newDocumentWindow.Text = studyPresenter.Title;
                ProjectPresenter.Update();
            };

            newDocumentWindow.Controls.Add(studyPresenter.View as UserControl);
            newDocumentWindow.VisibleChanged += (o, args) => studyPresenter.SetVisible(newDocumentWindow.Visible);
            newDocumentWindow.HandleDestroyed += (o, args) =>
            {
                studyPresenter.SetVisible(false);
            };

            studyPresenter.Window = newDocumentWindow;
            studyPresenter.PropertySelected += (sender, o) => View.PropertySelect(o);

            View.AddDocumentWindow(newDocumentWindow);
        }

        private void CreateNewLoadContourWindow(Guid? id)
        {
            var newDocumentWindow = new DocumentWindow("Load Contour");
            newDocumentWindow.ParentChanged += (sender, args) =>
            {
                if (newDocumentWindow.FloatingParent != null)
                    newDocumentWindow.FloatingParent.Icon = Resources.UltraBend_16px1;
            };
            var LoadContourPresenter = new LoadContourPresenter(new LoadContourView(), id,
                new LoadContoursService(ApplicationState.CurrentProjectTemporaryFile));

            DockWindowPresenters.Add(newDocumentWindow, LoadContourPresenter);

            View.PropertyGridValueChanged += (o, args) =>
            {
                if (LoadContourPresenter.Active)
                    LoadContourPresenter.Bind();
                newDocumentWindow.Text = LoadContourPresenter.Title;
                ProjectPresenter.Update();
            };
            newDocumentWindow.Controls.Add(LoadContourPresenter.View as UserControl);
            newDocumentWindow.VisibleChanged += (o, args) => LoadContourPresenter.SetVisible(newDocumentWindow.Visible);
            newDocumentWindow.HandleDestroyed += (o, args) =>
            {
                LoadContourPresenter.SetVisible(false);
            };

            LoadContourPresenter.Window = newDocumentWindow;
            LoadContourPresenter.PropertySelected += (sender, o) => View.PropertySelect(o);

            View.AddDocumentWindow(newDocumentWindow);
        }

        private void CreateNewCaseWindow(Guid? id)
        {
            var newDocumentWindow = new DocumentWindow("Bend Stiffener Case");
            newDocumentWindow.ParentChanged += (sender, args) =>
            {
                if (newDocumentWindow.FloatingParent != null)
                    newDocumentWindow.FloatingParent.Icon = Resources.UltraBend_16px1;
            };
            var casePresenter = new CasePresenter(new CaseDesignView(), id, new CasesService(ApplicationState.CurrentProjectTemporaryFile), new DesignsService(ApplicationState.CurrentProjectTemporaryFile), OutputsService, new ResultsService(ApplicationState.CurrentProjectTemporaryFile));
            
            DockWindowPresenters.Add(newDocumentWindow, casePresenter);
            
            View.PropertyGridValueChanged += (o, args) =>
            {
                if (casePresenter.Active)
                    casePresenter.Bind();
                newDocumentWindow.Text = casePresenter.Title;
                ProjectPresenter.Update();
            };
            
            View.StartSimulation += (sender, args) => casePresenter.StartSimulation();
            newDocumentWindow.Controls.Add(casePresenter.View as UserControl);
            newDocumentWindow.VisibleChanged += (o, args) => casePresenter.SetVisible(newDocumentWindow.Visible);
            newDocumentWindow.HandleDestroyed += (o, args) =>
            {
                casePresenter.SetVisible(false);
            };

            casePresenter.Window = newDocumentWindow;
            casePresenter.PropertySelected += (sender, o) => View.PropertySelect(o);

            View.AddDocumentWindow(newDocumentWindow);

        }

        private void CreateNewBendStiffenerDesignerWindow(Guid? id)
        {
            var newDocumentWindow = new DocumentWindow("Bend Stiffener Design");
            newDocumentWindow.ParentChanged += (sender, args) =>
            {
                if (newDocumentWindow.FloatingParent != null)
                    newDocumentWindow.FloatingParent.Icon = Resources.UltraBend_16px1;
            };
            var bendStiffenerDesignPresenter = new BendStiffenerDesignPresenter(new BendStiffenerDesignView(), id, new DesignsService(ApplicationState.CurrentProjectTemporaryFile));
            
            DockWindowPresenters.Add(newDocumentWindow, bendStiffenerDesignPresenter);

            if (bendStiffenerDesignPresenter.Model.Design.Sections.Count == 0)
                bendStiffenerDesignPresenter.AddSampleDesign();

            View.PropertyGridValueChanged += (o, args) =>
            {
                if (bendStiffenerDesignPresenter.Active)
                    bendStiffenerDesignPresenter.Bind();
                newDocumentWindow.Text = bendStiffenerDesignPresenter.Title;
                ProjectPresenter.Update();
            };
            View.AddBendStiffenerSection += (o, args) => bendStiffenerDesignPresenter.AddBendStiffenerSection();
            View.RemoveActiveStiffenerSection += (o, args) => bendStiffenerDesignPresenter.RemoveActiveStiffenerSection();
            newDocumentWindow.Controls.Add(bendStiffenerDesignPresenter.View as UserControl);
            newDocumentWindow.VisibleChanged += (o, args) => bendStiffenerDesignPresenter.SetVisible(newDocumentWindow.Visible);
            newDocumentWindow.HandleDestroyed += (o, args) =>
            {
                bendStiffenerDesignPresenter.SetVisible(false);
            };

            bendStiffenerDesignPresenter.Window = newDocumentWindow;
            bendStiffenerDesignPresenter.PropertySelected += (sender, o) => View.PropertySelect(o);

            View.AddDocumentWindow(newDocumentWindow);
        }



        private void CreateNewMaterialWindow(Guid? id)
        {
            var newDocumentWindow = new DocumentWindow("Material");
            newDocumentWindow.ParentChanged += (sender, args) =>
            {
                if (newDocumentWindow.FloatingParent != null)
                    newDocumentWindow.FloatingParent.Icon = Resources.UltraBend_16px1;
            };
            var materialPresenter = new MaterialPresenter(new MaterialView(), id, new MaterialsService(ApplicationState.CurrentProjectTemporaryFile));
            DockWindowPresenters.Add(newDocumentWindow, materialPresenter);

            materialPresenter.SetDefaults();
            
            View.PropertyGridValueChanged += (sender, args) =>
            {
                if (materialPresenter.Active)
                    materialPresenter.Bind();
                materialPresenter.PropertyValueChanged(sender, args);
                newDocumentWindow.Text = materialPresenter.Title;
                ProjectPresenter.Update();
            };
            newDocumentWindow.Controls.Add(materialPresenter.View as UserControl);
            newDocumentWindow.VisibleChanged += (o, args) => materialPresenter.SetVisible(newDocumentWindow.Visible);
            newDocumentWindow.HandleDestroyed += (o, args) =>
            {
                materialPresenter.SetVisible(false);
            };
            
            materialPresenter.Window = newDocumentWindow;
            materialPresenter.Bind();
            materialPresenter.PropertySelected += (sender, o) => View.PropertySelect(o);

            View.AddDocumentWindow(newDocumentWindow);
        }

        public void SetVisible(bool visible)
        {
            throw new NotImplementedException();
        }

        public bool GetVisible()
        {
            return View.GetVisible();
        }
    }
}
