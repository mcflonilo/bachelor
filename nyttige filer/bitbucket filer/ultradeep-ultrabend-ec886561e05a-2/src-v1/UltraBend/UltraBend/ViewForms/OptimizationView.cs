using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UltraBend.Views;
using PostSharp.Patterns.Model;

namespace UltraBend.ViewForms
{
    public partial class OptimizationView : UserControl, IOptimizationView
    {
        public OptimizationView()
        {
            InitializeComponent();
        }

        [WeakEvent]
        public event EventHandler<bool> ViewVisibleChanged;

        [WeakEvent]
        public event EventHandler<object> PropertySelected;

        public void SetVisible(bool visible)
        {
            throw new NotImplementedException();
        }
    }
}
