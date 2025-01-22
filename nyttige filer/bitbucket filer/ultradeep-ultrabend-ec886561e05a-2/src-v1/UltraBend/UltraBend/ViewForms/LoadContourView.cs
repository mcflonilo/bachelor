using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PostSharp.Patterns.Model;
using Telerik.WinControls.UI;
using UltraBend.Common.Units;
using UltraBend.Models;
using UltraBend.Views;

namespace UltraBend.ViewForms
{
    public partial class LoadContourView : UserControl, ILoadContourView
    {
        public LoadContourView()
        {
            Dock = DockStyle.Fill;
            InitializeComponent();
            radGridViewLoadContour.Columns["DeflectionAngle"].DataTypeConverter = new DimensionTypeConverter<AngleDimension>();
            radGridViewLoadContour.Columns["UmbilicalTension"].DataTypeConverter = new DimensionTypeConverter<ForceDimension>();
        }

        public DialogResult ShowDialog()
        {
            throw new NotImplementedException();
        }

        public void SetVisible(bool visible)
        {
            this.Visible = visible;
            OnViewVisibleChanged(visible);
        }

        public bool GetVisible()
        {
            throw new NotImplementedException();
        }

        [WeakEvent]
        public event EventHandler ViewLoading;
        [WeakEvent]
        public event EventHandler<object> PropertySelected;

        [WeakEvent]
        public event EventHandler<bool> ViewVisibleChanged;

        [WeakEvent]
        public event EventHandler DataChanged;

        protected LoadContourViewModel Model { get; set; }

        public void UpdateViewModel(LoadContourViewModel model)
        {
            Model = model;

            radGridViewLoadContour.BindingContext = new System.Windows.Forms.BindingContext();
            radGridViewLoadContour.DataSource = Model?.Rows;

            Plot();
        }

        private void Plot()
        {
            radChartView1.Series.Clear();

            var series = new ScatterLineSeries {LegendTitle = Model?.LoadContour?.Name};
            for (var i = 0; i < Model?.Rows.Count; i++)
            {
                series.DataPoints.Add(Model.Rows[i].DeflectionAngle*180.0/Math.PI, Model.Rows[i].UmbilicalTension);
            }

            radChartView1.Series.Add(series);
            radChartView1.ShowGrid = true;
            radChartView1.Axes[0].Title = "Deflection angle [deg]";
            radChartView1.Axes[1].Title = "Tension [kN]";
        }

        public void UpdateReport()
        {
            throw new NotImplementedException();
        }

        public void OnViewVisibleChanged(bool visible)
        {
            ViewVisibleChanged?.Invoke(this, visible);
        }

        private void loadContourBindingSource_DataMemberChanged(object sender, EventArgs e)
        {
            //Plot();
        }

        private void radGridViewLoadContour_RowsChanged(object sender, GridViewCollectionChangedEventArgs e)
        {
            DataChanged?.Invoke(this, EventArgs.Empty);
        }

        private void radGridViewLoadContour_EditorRequired(object sender, EditorRequiredEventArgs e)
        {

        }
    }
}
