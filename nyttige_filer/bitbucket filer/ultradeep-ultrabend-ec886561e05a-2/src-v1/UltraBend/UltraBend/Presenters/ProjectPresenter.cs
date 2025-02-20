using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PostSharp.Patterns.Model;
using Telerik.WinControls.UI.Docking;
using UltraBend.Common.MVP;
using UltraBend.Models;
using UltraBend.Services;
using UltraBend.Services.Project;
using UltraBend.Views;

namespace UltraBend.Presenters
{
    public class ProjectPresenter : Presenter<IProjectView, ProjectViewModel>, IWindowPresenter
    {
        [WeakEvent]
        public event EventHandler<object> PropertySelected;

        protected ProjectService projectService;
        protected UmbilicalsService umbilicalsService;
        protected MaterialsService materialsService;
        protected DesignsService designsService;
        protected LoadContoursService loadContoursService;
        protected CasesService casesService;
        protected StudiesService studiesService;

        [WeakEvent] public event EventHandler CreateNewUmbilical;

        [WeakEvent] public event EventHandler CreateNewDesign;

        [WeakEvent] public event EventHandler CreateNewMaterial;

        [WeakEvent] public event EventHandler CreateNewLoadContour;

        [WeakEvent] public event EventHandler CreateNewCase;

        [WeakEvent] public event EventHandler CreateNewStudy;

        [WeakEvent] public event EventHandler<Guid> NodeSelected;
        
        public string Title => "";
        
        private Guid _id = Guid.NewGuid();
        public Guid Id
        {
            get { return _id; }
        }
        
        public DockWindow Window { get; set; }

        public ProjectPresenter(IProjectView view, 
            ProjectService projectService, 
            UmbilicalsService umbilicalsService, 
            MaterialsService materialsService, 
            DesignsService designsService, 
            LoadContoursService loadContoursService, 
            CasesService casesService, 
            StudiesService studiesService) : base(view)
        {
            this.projectService = projectService;
            this.umbilicalsService = umbilicalsService;
            this.materialsService = materialsService;
            this.designsService = designsService;
            this.loadContoursService = loadContoursService;
            this.casesService = casesService;
            this.studiesService = studiesService;

            Model = new ProjectViewModel(){  };

            view.CreateNewUmbilical += (sender, args) => CreateNewUmbilical?.Invoke(null, EventArgs.Empty);
            view.CreateNewDesign += (sender, args) => CreateNewDesign?.Invoke(null, EventArgs.Empty);
            view.CreateNewMaterial += (sender, args) => CreateNewMaterial?.Invoke(null, EventArgs.Empty);
            view.CreateNewLoadContour += (sender, args) => CreateNewLoadContour?.Invoke(null, EventArgs.Empty);
            view.CreateNewCase += (sender, args) => CreateNewCase?.Invoke(null, System.EventArgs.Empty);
            view.CreateNewStudy += (sender, args) => CreateNewStudy?.Invoke(null, System.EventArgs.Empty);

            ProjectService.RepositoryUpdated += (sender, args) =>
                view.UpdateTreeView(umbilicalsService.GetUmbilicals(), materialsService.GetMaterials(),
                    designsService.GetDesigns(), loadContoursService.GetLoadContours(), casesService.GetCases(),
                    studiesService.GetStudies());

            view.UpdateTreeView(umbilicalsService.GetUmbilicals(), materialsService.GetMaterials(),
                designsService.GetDesigns(),
                loadContoursService.GetLoadContours(), casesService.GetCases(), studiesService.GetStudies());

            view.NodeSelected += (sender, guid) => { NodeSelected?.Invoke(sender, guid); };
        }

        public void Update()
        {
            View.UpdateTreeView(umbilicalsService.GetUmbilicals(), materialsService.GetMaterials(), designsService.GetDesigns(),
                loadContoursService.GetLoadContours(), casesService.GetCases(), studiesService.GetStudies());
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
