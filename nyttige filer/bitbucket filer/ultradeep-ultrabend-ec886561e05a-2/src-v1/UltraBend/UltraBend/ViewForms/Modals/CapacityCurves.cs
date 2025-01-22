using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.Charting;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace UltraBend.ViewForms.Modals
{
    public partial class CapacityCurves : Telerik.WinControls.UI.RadForm
    {
        public class DesignSeries
        {
            public string Name { get; set; }
            public double[] xs { get; set; }
            public double[] ys { get; set; }
        }

        protected readonly List<DesignSeries> Designs;

        public CapacityCurves(List<DesignSeries> designs)
        {
            Designs = designs;

            InitializeComponent();
        }

        

        private void CapacityCurves_Load(object sender, EventArgs e)
        {
            radChartView1.Series.Clear();

            foreach (var design in Designs)
            {
                var series = new ScatterLineSeries {LegendTitle = design.Name};
                for (var i = 0; i < Math.Min(design.xs.Length, design.ys.Length); i++)
                {
                    series.DataPoints.Add(design.xs[i], design.ys[i]);
                }

                radChartView1.Series.Add(series);
            }
        }
    }
}
