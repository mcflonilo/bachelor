using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telerik.WinControls;
using Ultra_Bend.BSEngine;
using Ultra_Bend.BSEngine.BSEngine;
using Ultra_Bend.Common;
using UltraBend.Services.BSEngine.Input;

namespace Ultra_Bend
{

    public partial class RunMonitor : Telerik.WinControls.UI.RadForm
    {
        private OutputsService _outputsService { get; set; }
        private TextBoxWriter _textBoxWriter { get; set; }

        private BackgroundWorker _backgroundWorker { get; set; }

        public RunMonitor()
        {
            InitializeComponent();
        }

        private void RunMonitor_Load(object sender, EventArgs e)
        {
            _outputsService = new OutputsService();
            _textBoxWriter = new TextBoxWriter(radTextBox1);
            _outputsService.AddTextWriter(_textBoxWriter);
            _backgroundWorker = new BackgroundWorker();


            AsyncContext.Run(async () =>
            {
                var solver = new Solver();

                await solver.FindOptimalBS(ApplicationState.Project, _outputsService);
            });
        }
    }
}
