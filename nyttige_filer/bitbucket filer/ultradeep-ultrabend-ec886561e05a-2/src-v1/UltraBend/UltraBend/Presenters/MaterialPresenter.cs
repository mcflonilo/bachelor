using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telerik.WinControls.UI;
using Telerik.WinControls.UI.Docking;
using UltraBend.Common.Math;
using UltraBend.Common.MVP;
using UltraBend.Controls;
using UltraBend.Models;
using UltraBend.Services.DomainObjects;
using UltraBend.Services.Project;
using UltraBend.Views;
using LineSeries = LiveCharts.Wpf.LineSeries;
using ScatterSeries = LiveCharts.Wpf.ScatterSeries;

namespace UltraBend.Presenters
{
    public class MaterialPresenter : Presenter<IMaterialView, MaterialViewModel>, IPropertySelectable, IButtonGroup, IWindowPresenter
    {
        protected MaterialsService materialsService { get; set; }

        
        public string Title
        {
            get { return Model?.Material.Name; }
        }
        
        
        private Guid _id = Guid.NewGuid();
        public Guid Id
        {
            get { return Model?.Material?.Id ?? _id; }
        }
        
        public DockWindow Window { get; set; }

        public object GetProperty()
        {
            return Model.Material;
        }

        public ButtonGroup GetButtonGroup()
        {
            return ButtonGroup.Material;
        }

        public ButtonGroup GetButtonGroupToShowByDefault()
        {
            return ButtonGroup.Material;
        }

        public bool Active { get; set; } = false;
        
        private static object regressionlock = new object();

        private static bool _hasSkippedRegression = false;

        public void SetVisible(bool visible)
        {
            Active = visible;
            View.SetVisible(visible);
            OnPropertySelected(Model.Material);
        }

        public bool GetVisible()
        {
            return View.GetVisible();
        }
        public MaterialPresenter(IMaterialView view, Guid? id, MaterialsService materialsService) : base(view)
        {
            this.materialsService = materialsService;

            this.Model = new MaterialViewModel(){ Material = null};

            View.ViewVisibleChanged += (sender, b) => Active = b;
            //View.ViewVisibleChanged += (sender, b) => Bind();
            View.ViewVisibleChanged += (sender, args) => OnVisibleChanged(args);
            View.DefaultValuesNeeded+=ViewOnDefaultValuesNeeded;
            View.PropertySelected += (sender, o) => OnPropertySelected(o);
            View.DataChanged += (sender, args) =>
            {
                PerformRegression(true);
                SaveChanges();
            };

            if (id == null)
            {
                id = Guid.NewGuid();
                materialsService.UpsertMaterial(new Material() {Id = id.Value, Name = materialsService.GetNewName()});
            }

            this.Model.Material = materialsService.GetMaterialById(id.Value);

            View.ViewLoading += (sender, args) => Bind();
        }

        private void SaveChanges()
        {
            materialsService.UpsertMaterial(Model.Material);
        }

        private void ViewOnDefaultValuesNeeded(object sender1, GridViewRowEventArgs gridViewRowEventArgs)
        {
            if (gridViewRowEventArgs.Row is GridViewNewRowInfo)
            {
                if (Model.Material.Data.Any() && Model.Material.Data.Select(d => d.Temperature).Count() > 1)
                {
                    var temperatures = Model.Material.Data.Select(d => d.Temperature).ToArray();
                    gridViewRowEventArgs.Row.Cells["Temperature"].Value = temperatures[temperatures.Length - 1];
                }
            }
        }
        public void PropertyValueChanged(object sender, string category)
        {
            //if (category.IndexOf("Interpolation", StringComparison.OrdinalIgnoreCase) >= 0)
            //{
                PerformRegression(true);
                SaveChanges();
            //}
        }

        public void Bind()
        {
            View.SetMaterialDataSource(new CsvBindingList<MaterialData>(Model.Material.Data));
            OnPropertySelected(Model.Material);
            PerformRegression(true);

            View.SetLinearControlState(Model.Material.NonLinear);

            SaveChanges();
        }


        public void PerformRegression(bool plot)
        {
            if (Monitor.TryEnter(regressionlock, 0))
            {
                try
                {
                    MaterialsService.PerformRegression(Model.Material);

                    //csvGridCoefficients.DataSource = typeof(List<RegressionCoefficient>);
                    //csvGridCoefficients.DataSource = Material.Regression.Coefficients;

                    OnPropertySelected(Model.Material);
                    

                    if (plot)
                    {
                        View.Plot(new CsvBindingList<MaterialData>(Model.Material.Data), Model.Material.Regression, Model.Material.AllowTemperatureExtrapolation, Model.Material.TemperatureExtrapolationMax, Model.Material.TemperatureExtrapolationMin);
                    }
                }
                finally
                {
                    Monitor.Exit(regressionlock);
                    if (_hasSkippedRegression)
                    {
                        PerformRegression(true);
                        _hasSkippedRegression = false;
                    }
                }
            }
            else
            {
                _hasSkippedRegression = true;
            }

        }

        public virtual void OnVisibleChanged(bool visible)
        {
            if (visible)
                OnPropertySelected(Model.Material);

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

        public void SetDefaults()
        {
            Model.Material.RegressionOrder = 7;
        }
    }
}
