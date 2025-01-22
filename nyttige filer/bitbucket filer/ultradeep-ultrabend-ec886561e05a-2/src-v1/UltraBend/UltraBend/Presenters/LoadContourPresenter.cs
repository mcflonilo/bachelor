using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PostSharp.Patterns.Model;
using Telerik.WinControls.UI.Docking;
using Telerik.Windows.Documents.Spreadsheet.Expressions.Functions;
using UltraBend.Common.MVP;
using UltraBend.Models;
using UltraBend.Services.DomainObjects;
using UltraBend.Services.Project;
using UltraBend.Views;

namespace UltraBend.Presenters
{
    public class LoadContourPresenter : Presenter<ILoadContourView, LoadContourViewModel>, IPropertySelectable, IButtonGroup, IWindowPresenter
    {
        protected LoadContoursService LoadContoursService { get; set; }

        public bool Active { get; set; } = false;
        
        public LoadContourPresenter(ILoadContourView view, Guid? id, LoadContoursService loadContoursService) : base(view)
        {
            this.LoadContoursService = loadContoursService;

            this.Model = new LoadContourViewModel(){LoadContour = null};

            //this.Model.Rows = new List<LoadContourViewModelRowEntry>
            //{
            //    new LoadContourViewModelRowEntry{}
            //}

            View.ViewVisibleChanged += (sender, b) => Active = b;
            View.ViewVisibleChanged += (sender, args) => OnVisibleChanged(args);
            View.PropertySelected += (sender, o) => OnPropertySelected(o);
            View.DataChanged += (sender, args) => { SaveChanges(); };

            if (id == null || id == new Guid())
            {
                id = Guid.NewGuid();
                loadContoursService.UpsertLoadContour(new LoadContour()
                    {Id = id.Value, Name = loadContoursService.GetNewName()});
            }

            LoadContoursService.RepositoryUpdated += ReloadModel;

            this.Model.LoadContour = loadContoursService.GetLoadContourById(id.Value);

            this.Model.Rows = new List<LoadContourViewModelRowEntry>();
            for (var i = 0;
                i < Model.LoadContour.UmbilicalTensions.Count && i < Model.LoadContour.DeflectionAngles.Count;
                i++)
            {
                this.Model.Rows.Add(new LoadContourViewModelRowEntry
                {
                    DeflectionAngle = Model.LoadContour.DeflectionAngles[i],
                    UmbilicalTension = Model.LoadContour.UmbilicalTensions[i]
                });
            }

            if (this.Model?.Rows?.Count == 0)
            {
                this.Model.Rows = new List<LoadContourViewModelRowEntry>
                {
                    new LoadContourViewModelRowEntry() {DeflectionAngle = 21 * Math.PI / 180.0, UmbilicalTension = 180},
                    new LoadContourViewModelRowEntry() {DeflectionAngle = 18 * Math.PI / 180.0, UmbilicalTension = 400},
                    new LoadContourViewModelRowEntry() {DeflectionAngle = 17 * Math.PI / 180.0, UmbilicalTension = 600},
                    new LoadContourViewModelRowEntry() {DeflectionAngle = 14 * Math.PI / 180.0, UmbilicalTension = 800},
                    new LoadContourViewModelRowEntry()
                        {DeflectionAngle = 13 * Math.PI / 180.0, UmbilicalTension = 1000},
                    new LoadContourViewModelRowEntry() {DeflectionAngle = 8 * Math.PI / 180.0, UmbilicalTension = 1200},
                    new LoadContourViewModelRowEntry() {DeflectionAngle = 5 * Math.PI / 180.0, UmbilicalTension = 1250},
                    new LoadContourViewModelRowEntry() {DeflectionAngle = 4 * Math.PI / 180.0, UmbilicalTension = 1300}
                };
            }

            View.DataChanged += View_DataChanged;

            Bind();
        }

        private void View_DataChanged(object sender, EventArgs e)
        {
            SaveChanges();
        }

        private void ReloadModel(object sender, EventArgs e)
        {
            this.Model.LoadContour = LoadContoursService.GetLoadContourById(this.Model.LoadContour.Id);
        }

        private void SaveChanges()
        {
            Model.LoadContour.UmbilicalTensions = Model.Rows.Select(i => i.UmbilicalTension).ToList();
            Model.LoadContour.DeflectionAngles = Model.Rows.Select(i => i.DeflectionAngle).ToList();
            LoadContoursService.UpsertLoadContour(Model.LoadContour);
            Bind();
        }
        public object GetProperty()
        {
            return Model.LoadContour;
        }
        public void Bind()
        {
            View.UpdateViewModel(this.Model);
            OnPropertySelected(Model.LoadContour);
        }

        public virtual void OnVisibleChanged(bool visible)
        {
            if (visible)
                OnPropertySelected(Model.LoadContour);

            VisibleChanged?.Invoke(this, visible);
        }

        [WeakEvent]
        public event EventHandler<bool> VisibleChanged;

        public virtual void OnPropertySelected(object selection)
        {
            PropertySelected?.Invoke(this, selection);
        }

        [WeakEvent]
        public event EventHandler<object> PropertySelected;

        public ButtonGroup GetButtonGroup()
        {
            return ButtonGroup.LoadContour;
        }

        public ButtonGroup GetButtonGroupToShowByDefault()
        {
            return ButtonGroup.LoadContour;
        }

        public string Title
        {
            get { return Model?.LoadContour.Name; }
        }


        public Guid Id
        {
            get { return Model?.LoadContour?.Id ?? Guid.NewGuid(); }
        }

        public void Update() { }

        public DockWindow Window { get; set; }

        public void SetVisible(bool visible)
        {
            Active = visible;
            View.SetVisible(visible);
            OnPropertySelected(Model.LoadContour);
        }

        public bool GetVisible()
        {
            return View.GetVisible();
        }
    }
}
