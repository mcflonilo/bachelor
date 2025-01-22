using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using AutoMapper;
using Newtonsoft.Json;
using Telerik.WinControls;
using Ultra_Bend.Common;

namespace Ultra_Bend
{
    public partial class DataEntry : Telerik.WinControls.UI.RadForm
    {
        private object _dataSource { get; set; }
        private object _dataSourceClone { get; set; }
        private bool _closingClean = false;

        public DataEntry(string title, Func<object> dataSource)
        {
            InitializeComponent();

            radLabel1.Text = $"{title}:";
            Text = title;

            _dataSource = dataSource();
            _dataSourceClone = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(_dataSource));

            //radPropertyGrid.auto
            radPropertyGrid.SelectedObject = _dataSource;
            radPropertyGrid.ExpandAllGridItems();
            
        }

        private void radButtonOK_Click(object sender, EventArgs e)
        {
            _closingClean = true;
            DialogResult = DialogResult.OK;
        }

        private void radButtonCancel_Click(object sender, EventArgs e)
        {

            ApplicationState.Mapper.Map(_dataSourceClone, _dataSource);

            DialogResult = DialogResult.Cancel;
        }

        private void DataEntry_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_closingClean)
            {
                ApplicationState.Mapper.Map(_dataSourceClone, _dataSource);
            }
        }
    }
}
