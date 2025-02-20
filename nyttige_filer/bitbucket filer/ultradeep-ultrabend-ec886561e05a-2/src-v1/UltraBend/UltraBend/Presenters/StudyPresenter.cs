using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Accord.IO;
using Nito.AsyncEx;
using PostSharp.Patterns.Model;
using Telerik.WinControls;
using Telerik.WinControls.UI.Docking;
using UltraBend.Common.MVP;
using UltraBend.Engine.BSEngine;
using UltraBend.Models;
using UltraBend.Services;
using UltraBend.Services.BSEngine.Output;
using UltraBend.Services.DomainObjects;
using UltraBend.Services.Project;
using UltraBend.ViewForms.Modals;
using UltraBend.Views;

namespace UltraBend.Presenters
{
    public class StudyPresenter : Presenter<IStudyView, StudyViewModel>, IPropertySelectable, IButtonGroup, IWindowPresenter
    {
        protected StudiesService studiesService { get; set; }
        protected OutputsService outputsService { get; set; }
        protected ResultsService resultsService { get; set; }

        public bool Active { get; set; } = false;

        public StudyPresenter(IStudyView view, Guid? id, StudiesService studiesService, OutputsService outputsService, ResultsService resultsService) : base(view)
        {
            this.studiesService = studiesService;
            this.outputsService = outputsService;
            this.resultsService = resultsService;

            this.Model = new StudyViewModel() { Study = null };
            if (id == null)
            {
                id = Guid.NewGuid();
                studiesService.UpsertStudy(new Services.DomainObjects.Study() { Id = id.Value, Name = studiesService.GetNewName() });
            }

            this.Model.Study = studiesService.GetStudyById(id.Value);
            //this.Model.Cases.Clear();
            //this.Model.Study.Cases.ForEach(c => this.Model.Cases.Add(c));

            View.ViewLoading += (sender, args) => Bind();
            View.DataChanged += (sender, args) => SaveChanges();

            View.UpdateViewModel(this.Model);
        }

        public string Title 
        {
            get { return Model?.Study.Name; }
        }

        private Guid _id = Guid.NewGuid();
        public Guid Id
        {
            get { return Model?.Study?.Id ?? _id; }
        }


        public DockWindow Window { get; set; }

        public ButtonGroup GetButtonGroup()
        {
            return ButtonGroup.Study;
        }

        [WeakEvent]
        public event EventHandler<object> PropertySelected;
        public virtual void OnPropertySelected(object selection)
        {
            PropertySelected?.Invoke(this, selection);
        }

        public void SetVisible(bool visible)
        {
            Active = visible;
            View.SetVisible(visible);
            OnPropertySelected(Model.Study);
        }

        public bool GetVisible()
        {
            return View.GetVisible();
        }

        public ButtonGroup GetButtonGroupToShowByDefault()
        {
            return ButtonGroup.Case;
        }

        public object GetProperty()
        {
            return Model.Study;
        }

        public void Bind()
        {

            SaveChanges();
        }

        private void SaveChanges()
        {
            //Model.Study.Cases = Model.Cases.ToList();
            studiesService.UpsertStudy(Model.Study);
        }

        public void ShowCapacityCurves()
        {
            var designs = new List<ViewForms.Modals.CapacityCurves.DesignSeries>();

            var xs = Model.Study.Cases.Select(i => i.Result.Output.KeyResults.MaximumBSCurvature)
                .ToArray();
            var ys = Model.Study.Cases.Select(i => i.Result.Output.KeyResults.RiserTensionAtRootEnd)
                .ToArray();

            designs.Add(new CapacityCurves.DesignSeries
            {
                Name = "Test",
                xs = xs,
                ys = ys
            });
            
            using (var form = new ViewForms.Modals.CapacityCurves(designs))
            {
                form.ShowDialog();
            }
        }

        public void StartAllSimulations()
        {
            var validationResult = this.Model.Study.Validate(new ValidationContext(this));
            if (validationResult.Any())
            {
                var message = string.Join("\r\n", validationResult.Select(i => i.ErrorMessage).ToArray()).TrimStart();
                RadMessageBox.Show(message, "Validation Error(s)");
                return;
            }
            var engine = new BsEngine(outputsService);

            //var tasks = new List<Task>();
            foreach (var @case in Model.Study.Cases)
            {
                var input = @case.GetConfiguration(Model.Study.Design);
                BendStiffenerOutput output = null;

                AsyncContext.Run(async () =>
                {
                    try
                    {
                        //tasks.Add(Task.Run(async () =>
                        //{
                            output = await engine.ExecuteAsync(input).ConfigureAwait(false);
                            @case.Result = new Result
                            {
                                Output = output,
                                Id = Guid.NewGuid(),
                                Name = @case.Name
                            };
                            //}));
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                });

                //var result = resultsService.UpsertResult(new Result
                //{
                //    Output = output,
                //    Id = Guid.NewGuid(),
                //    Name = @case.Name
                //});

                //break;
            }

            foreach (var @case in Model.Study.Cases)
            {
                var result = resultsService.UpsertResult(@case.Result);
                @case.ResultId = result.Id;
            }
            //Task.WaitAll(tasks.ToArray());


            SaveChanges();
            View.UpdateViewModel(this.Model);
        }
    }
}
