using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using Ultra_Bend.Common;

namespace Ultra_Bend
{
    public partial class RunAnalysis : Telerik.WinControls.UI.RadForm
    {
        public RunAnalysis()
        {
            InitializeComponent();
        }

        private void radButtonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void radButtonChangeRiserModelParameters_Click(object sender, EventArgs e)
        {

            using (var form = new DataEntry("Riser Model Parameters", () => ApplicationState.Project.BendStiffener.BendStiffenerConstraints))
            {
                form.ShowDialog(this);
            }
        }

        private void radButtonChangeFEAnalysisParameters_Click(object sender, EventArgs e)
        {
            using (var form = new DataEntry("Finite Element Analysis Parameters", () => ApplicationState.Project.FiniteElementAnalysisParameters))
            {
                form.ShowDialog(this);
            }

        }

        private void radButtonFindOptimalBS_Click(object sender, EventArgs e)
        {
            using (var form = new RunMonitor())
            {
                form.ShowDialog(this);
            }
        }
    }
}
