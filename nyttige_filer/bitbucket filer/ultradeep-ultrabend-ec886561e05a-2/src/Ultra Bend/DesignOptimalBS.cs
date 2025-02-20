using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using Ultra_Bend.Common;
using Ultra_Bend.Common.Data;
using Ultra_Bend.Common.Data.TestCases;

namespace Ultra_Bend
{
    public partial class DesignOptimalBS : Telerik.WinControls.UI.RadForm
    {
        public DesignOptimalBS()
        {
            InitializeComponent();
        }

        private void radButton6_Click(object sender, EventArgs e)
        {

            using (var form = new RunAnalysis())
            {
                form.ShowDialog(this);
            }
        }

        private void radButton5_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void radButtonProjectInformation_Click(object sender, EventArgs e)
        {
            using (var form = new DataEntry("Project Information", () => ApplicationState.Project.ProjectInformation))
            {
                form.ShowDialog(this);
            }
        }

        private void radButtonInputRiserInformation_Click(object sender, EventArgs e)
        {
            using (var form = new DataEntry("Riser Information", () => ApplicationState.Project.RiserInformation))
            {
                form.ShowDialog(this);
            }

        }

        private void radButtonInputRiserCapacities_Click(object sender, EventArgs e)
        {
            using (var form = new DataEntry("Riser Capacities", () => ApplicationState.Project.RiserCapacities))
            {
                form.ShowDialog(this);
            }
        }

        private void radButtonInputRiserResponse_Click(object sender, EventArgs e)
        {
            using (var form = new DataEntry("Riser Responses", () => ApplicationState.Project.RiserResponses))
            {
                form.ShowDialog(this);
            }
            
        }

        private void radButtonInputBendStiffenerInformation_Click(object sender, EventArgs e)
        {
            using (var form = new DataEntry("Bend Stiffener", () => ApplicationState.Project.BendStiffener))
            {
                form.ShowDialog(this);
            }
        }

        private void radButtonLoadSampleBendStiffener_Click(object sender, EventArgs e)
        {
            ApplicationState.Project = NexansBacalhau.GetProject();

        }
    }
}
