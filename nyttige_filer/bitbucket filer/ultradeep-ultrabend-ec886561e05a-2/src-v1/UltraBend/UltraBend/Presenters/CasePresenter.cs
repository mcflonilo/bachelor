using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gu.Units;
using Nito.AsyncEx;
using PostSharp.Patterns.Model;
using Telerik.WinControls.UI.Docking;
using UltraBend.Common.MVP;
using UltraBend.Engine.BSEngine;
using UltraBend.Models;
using UltraBend.Services;
using UltraBend.Services.BSEngine.Input;
using UltraBend.Services.BSEngine.Output;
using UltraBend.Services.DomainObjects;
using UltraBend.Services.Project;
using UltraBend.Views;
using Force = UltraBend.Services.BSEngine.Input.Force;
using Material = UltraBend.Services.BSEngine.Input.Material;
using Segment = UltraBend.Services.BSEngine.Input.Segment;

namespace UltraBend.Presenters
{
    public class CasePresenter : Presenter<ICaseView, CaseViewModel>, IPropertySelectable, IButtonGroup, IWindowPresenter
    {
        protected CasesService casesService { get; set; }
        protected OutputsService outputsService { get; set; }
        protected DesignsService designsService { get; set; }
        protected ResultsService resultsService { get; set; }

        public bool Active { get; set; } = false;

        public CasePresenter(ICaseView view, Guid? id, CasesService casesService, DesignsService designsService, OutputsService outputsService, ResultsService resultsService) : base(view)
        {
            this.casesService = casesService;
            this.outputsService = outputsService;
            this.designsService = designsService;
            this.resultsService = resultsService;

            this.Model = new CaseViewModel(){Case = null};
            
            View.ViewVisibleChanged += (sender, b) => Active = b;
            //View.ViewVisibleChanged += (sender, b) => Bind();
            View.ViewVisibleChanged += (sender, args) => OnVisibleChanged(args);
            View.PropertySelected += (sender, o) => OnPropertySelected(o);
            View.DataChanged += (sender, args) =>
            {
                SaveChanges();
            };

            if (id == null)
            {
                id = Guid.NewGuid();
                Debug.Assert(casesService != null, nameof(this.casesService) + " != null");
                casesService.UpsertCase(new Case() {Id = id.Value, Name = casesService.GetNewName()});
            }

            CasesService.RepositoryUpdated += ReloadModel;
            DesignsService.RepositoryUpdated += ReloadModel;
            ResultsService.RepositoryUpdated += ReloadModel;

            this.Model.Case = casesService.GetCaseById(id.Value);

            Bind();
        }

        private void ReloadModel(object sender, EventArgs e)
        {
            this.Model.Case = casesService.GetCaseById(this.Model.Case.Id);
        }

        private void SaveChanges()
        {
            casesService.UpsertCase(Model.Case);
        }

        public object GetProperty()
        {
            return Model.Case;
        }
        public void PropertyValueChanged(object sender, string category)
        {
            SaveChanges();
        }
        public void Bind()
        {
            View.UpdateViewModel(this.Model);
            OnPropertySelected(Model.Case);
            SaveChanges();
        }
        
        public virtual void OnVisibleChanged(bool visible)
        {
            if (visible)
                OnPropertySelected(Model.Case);

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
            return ButtonGroup.Case;
        }

        public ButtonGroup GetButtonGroupToShowByDefault()
        {
            return ButtonGroup.Case;
        }
        
        public string Title
        {
            get { return Model?.Case.Name; }
        }
        
        
        private Guid _id = Guid.NewGuid();
        public Guid Id
        {
            get { return Model?.Case?.Id ?? _id; }
        }
        public void SetVisible(bool visible)
        {
            Active = visible;
            View.SetVisible(visible);
            OnPropertySelected(Model.Case);
        }

        public bool GetVisible()
        {
            return View.GetVisible();
        }

        public DockWindow Window { get; set; }

        
        public void StartSimulation()
        {
            var engine = new BsEngine(outputsService);

            var input = Model.Case.GetConfiguration(Model.Case.Design);

            BendStiffenerOutput output = null;

            AsyncContext.Run(async () =>
            {
                try
                {
                    output = await engine.ExecuteAsync(input).ConfigureAwait(false);
                }
                catch(Exception e)
                {
                    throw e;
                }
            });

            var result = resultsService.UpsertResult(new Result
            {
                Output = output,
                Id = Guid.NewGuid(),
                Name = "Result"
            });
            
            Model.Case.Result = result;
            Model.Case.ResultId = result.Id;

            SaveChanges();
            View.UpdateViewModel(this.Model);

            //UltraBend.Reports.DataSources.BendStiffenerResult.Data = output.BendStiffenerResults.Select(r =>
            //{
            //    return new UltraBend.Reports.DataSources.BendStiffenerResult()
            //    {
            //        Curvature = r.Curvature,
            //        Moment = r.Moment,
            //        Strain = r.Strain,
            //        OuterDiameter = r.OuterDiameter,
            //        ShearForce = r.ShearForce,
            //        LengthCoordinate = r.LengthCoordinate
            //    };
            //}).ToList();

            //View.UpdateReport();
        }
    }
}
