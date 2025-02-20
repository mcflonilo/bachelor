using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using LiveCharts;
using Telerik.WinControls.UI;
using UltraBend.Common;
using UltraBend.Common.Math;
using UltraBend.Controls;
using UltraBend.Services.DomainObjects;
using UltraBend.Views;
using PostSharp.Patterns.Model;

namespace UltraBend.ViewForms
{
    public partial class MaterialView : UserControl, IMaterialView
    {
        [WeakEvent]
        public event EventHandler ViewLoading;

        [WeakEvent]
        public event EventHandler<object> PropertySelected;

        [WeakEvent]
        public event EventHandler<bool> ViewVisibleChanged;

        [WeakEvent]
        public event EventHandler DataChanged;

        [WeakEvent]
        public event GridViewRowEventHandler DefaultValuesNeeded;

        public List<LiveCharts.Wpf.LineSeries> FitSeries { get; set; }
        public List<LiveCharts.Wpf.LineSeries> ExtrapolationSeries { get; set; }
        public List<LiveCharts.Wpf.ScatterSeries> DataSeries { get; set; }

        public MaterialView()
        {
            Dock = DockStyle.Fill;
            VisibleChanged += (sender, args) => OnViewVisibleChanged(this.Visible);

            DataSeries = new List<LiveCharts.Wpf.ScatterSeries>(); //new LiveCharts.Wpf.ScatterSeries() { Title = "Data Point", Values = new ChartValues<LiveCharts.Defaults.ObservablePoint>() };
            FitSeries = new List<LiveCharts.Wpf.LineSeries>();
            ExtrapolationSeries = new List<LiveCharts.Wpf.LineSeries>();

            InitializeComponent();
        }

        public DialogResult ShowDialog()
        {
            throw new NotImplementedException();
        }

        public void SetLinearControlState(bool nonlinear)
        {
            csvGridMaterialData.Enabled = nonlinear;
            cartesianChart1.Enabled = nonlinear;
        }

        public bool GetVisible()
        {
            return this.Visible;
        }
        public void SetVisible(bool visible)
        {
            this.Visible = visible;
            OnViewVisibleChanged(visible);
        }
        public void OnViewVisibleChanged(bool visible)
        {
            ViewVisibleChanged?.Invoke(this, visible);
        }

        public void OnPropertySelected(Material selection)
        {
            PropertySelected?.Invoke(this, selection);
        }
        protected virtual void OnViewLoading()
        {
            ViewLoading?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnDataChanged()
        {
            DataChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnDefaultValuesNeeded(GridViewRowEventArgs e)
        {
            DefaultValuesNeeded?.Invoke(this, e);
        }

        public void SetMaterialDataSource(CsvBindingList<MaterialData> data)
        {
            csvGridMaterialData.DataSource = data;
        }
        private void MaterialView_Load(object sender, EventArgs e)
        {
            cartesianChart1.Series = new SeriesCollection();
            cartesianChart1.Update(true,true);

            csvGridMaterialData.MultiSelect = true;
            csvGridMaterialData.SelectionMode = GridViewSelectionMode.CellSelect;
            csvGridMaterialData.ClipboardPasteMode = GridViewClipboardPasteMode.EnableWithNotifications;
            csvGridMaterialData.TableElement.BackColor = Color.Transparent;
            csvGridMaterialData.DefaultValuesNeeded += (o, args) => OnDefaultValuesNeeded(args);
            csvGridMaterialData.OnPaste += (o, args) => OnDataChanged();
            csvGridMaterialData.RowsChanged += (o, args) => OnDataChanged();
            csvGridMaterialData.CellValueChanged += (o, args) =>
            {
                if (args.Row is GridViewNewRowInfo == false)
                {
                    OnDataChanged();
                }
            };

            csvGridMaterialData.MasterTemplate.Columns.Add(new GridViewDecimalColumn()
            {
                Name = "Strain",
                HeaderText = "Strain [-]",
                FieldName = "Strain",
                DecimalPlaces = 16,
                FormatString = "{0:g}",
                FormatInfo = CultureInfo.CurrentCulture,
                DataType = typeof(double)
            });
            csvGridMaterialData.MasterTemplate.Columns.Add(new GridViewDecimalColumn()
            {
                Name = "Stress",
                HeaderText = "Stress [Pa]",
                FieldName = "Stress",
                DecimalPlaces = 16,
                FormatString = "{0:g}",
                FormatInfo = CultureInfo.CurrentCulture,
                DataType = typeof(double)
            });
            csvGridMaterialData.MasterTemplate.Columns.Add(new GridViewDecimalColumn()
            {
                Name = "Temperature",
                HeaderText = "Temperature [K]",
                FieldName = "Temperature",
                DecimalPlaces = 16,
                FormatString = "{0:g}",
                FormatInfo = CultureInfo.CurrentCulture,
                DataType = typeof(double)
            });

            //csvGridCoefficients.DataSource = Material.Regression.Coefficients;
            //csvGridCoefficients.MasterTemplate.Columns.Add(new GridViewTextBoxColumn()
            //{
            //    Name = "Coefficient",
            //    HeaderText = "Coefficient",
            //    FieldName = "Coefficient"
            //});
            //csvGridCoefficients.MasterTemplate.Columns.Add(new GridViewTextBoxColumn()
            //{
            //    Name = "Expression",
            //    HeaderText = "Term",
            //    FieldName = "ExpressionString"
            //});




            OnViewLoading();
        }
        public void Plot(CsvBindingList<MaterialData> data, ExpressionRegression regression, bool allowTemperatureExtrapolation, double temperatureExtrapolationMax, double temperatureExtrapolationMin)
        {

            // plots won't show if plotting too early (before axis), so just drop back out for now
            if (cartesianChart1.AxisX.Count == 0)
                return;

            foreach (var series in DataSeries)
                series.Values.Clear();

            foreach (var series in FitSeries)
                series.Values.Clear();

            foreach (var series in ExtrapolationSeries)
                series.Values.Clear();

            var temperatures = data.Select(d => d.Temperature).Distinct().ToList();

            for (var i = 0; i < temperatures.Count; i++)
            {
                if (FitSeries.Count == i)
                {
                    FitSeries.Add(new LiveCharts.Wpf.LineSeries() { Values = new ChartValues<LiveCharts.Defaults.ObservablePoint>(), Fill = System.Windows.Media.Brushes.Transparent, PointGeometry = null, Stroke = System.Windows.Media.Brushes.DarkGray });
                    cartesianChart1.Series.Add(FitSeries[i]);
                }

                if (DataSeries.Count == i)
                {
                    DataSeries.Add(new LiveCharts.Wpf.ScatterSeries() { Values = new ChartValues<LiveCharts.Defaults.ObservablePoint>(), Fill = System.Windows.Media.Brushes.Black, Stroke = System.Windows.Media.Brushes.Black });
                    cartesianChart1.Series.Add(DataSeries[i]);
                }

                DataSeries[i].Title = $"T = {temperatures[i]:N2}";
                foreach (MaterialData materialData in data)
                {
                    if (Math.Abs(materialData.Temperature - temperatures[i]) < float.Epsilon)
                    {
                        DataSeries[i].Values.Add(new LiveCharts.Defaults.ObservablePoint(materialData.Strain, materialData.Stress / 1.0E6));
                    }
                }

                var xmin = Math.Min(regression.x[0].Min(), 0);
                var xmax = regression.x[0].Max();

                if (regression.Coefficients.Any())
                {
                    FitSeries[i].Title = $"T = {temperatures[i]:N2}";
                    for (var j = 0; j < 50; j++)
                    {
                        var xi = xmin + j * (xmax - xmin) / 50;
                        var yfit = regression.Eval(new[] { xi, temperatures[i] });
                        FitSeries[i].Values.Add(new LiveCharts.Defaults.ObservablePoint(xi, yfit.RoundToSignificantDigits(5) / 1.0E6));
                    }
                }
            }

            if (data.Any() && allowTemperatureExtrapolation)
            {
                var tmax = temperatureExtrapolationMax;
                var tmin = temperatureExtrapolationMin;

                var trueTMax = data.Select(d => d.Temperature).Max();
                var trueTMin = data.Select(d => d.Temperature).Min();

                if (tmax == 0)
                    tmax = 1.25 * trueTMax;

                if (tmin == 0)
                    tmin = 0.75 * trueTMin;

                int extrapolationBoundCount = 10;
                for (var i = 0; i <= extrapolationBoundCount; i++)
                {

                    var xmin = Math.Min(regression.x[0].Min(), 0);
                    var xmax = regression.x[0].Max();

                    var ti = tmin + i * (tmax - tmin) / extrapolationBoundCount;

                    if (ExtrapolationSeries.Count == i)
                    {
                        var StrokeBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(175, 200, 200, 200));

                        if (ti > trueTMax || ti < trueTMin)
                        {
                            StrokeBrush =
                                new System.Windows.Media.SolidColorBrush(
                                    System.Windows.Media.Color.FromArgb(75, 255, 0, 0));
                        }

                        ExtrapolationSeries.Add(new LiveCharts.Wpf.LineSeries()
                        {
                            Values = new ChartValues<LiveCharts.Defaults.ObservablePoint>(),
                            Fill = System.Windows.Media.Brushes.Transparent,
                            PointGeometry = null,
                            Stroke = StrokeBrush,
                            StrokeDashArray = new System.Windows.Media.DoubleCollection { 2 },
                            StrokeThickness = 2,
                            //DataLabels = true
                        });
                        cartesianChart1.Series.Add(ExtrapolationSeries[i]);
                    }


                    if (regression.Coefficients.Any())
                    {
                        if (ti <= trueTMax && ti >= trueTMin)
                        {
                            ExtrapolationSeries[i].Visibility = Visibility.Hidden;
                        }

                        ExtrapolationSeries[i].Title = $"T = {ti:N2}";
                        for (var j = 0; j < 50; j++)
                        {
                            var xi = xmin + j * (xmax - xmin) / 50;
                            var yfit = regression.Eval(new[] { xi, ti });
                            ExtrapolationSeries[i].Values.Add(new LiveCharts.Defaults.ObservablePoint(xi, yfit.RoundToSignificantDigits(5) / 1.0E6));
                        }
                    }
                }
            }

            if (data.Any())
            {
                foreach (var xaxis in cartesianChart1.AxisX)
                {
                    xaxis.MinValue = Math.Min(data.Select(d => d.Strain).Min(), 0);
                    xaxis.MaxValue = Math.Max(data.Select(d => d.Strain).Max(), 0);
                    xaxis.Title = "Strain [-]";
                }

                foreach (var yaxis in cartesianChart1.AxisY)
                {
                    yaxis.MinValue = Math.Min(data.Select(d => d.Stress).Min() / 1.0E6, 0);
                    yaxis.MaxValue = Math.Max(data.Select(d => d.Stress).Max() / 1.0E6, 0);
                    yaxis.Title = "Stress [MPa]";
                }
            }
        }
    }
}
