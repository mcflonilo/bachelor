using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace Ultra_Bend
{
    public partial class Main : Telerik.WinControls.UI.RadForm
    {
        public Main()
        {
            InitializeComponent();
        }

        private void radButtonDesignOptimalBS_Click(object sender, EventArgs e)
        {
            using (var form = new DesignOptimalBS())
            {
                form.ShowDialog(this);
            }
        }
    }
}
