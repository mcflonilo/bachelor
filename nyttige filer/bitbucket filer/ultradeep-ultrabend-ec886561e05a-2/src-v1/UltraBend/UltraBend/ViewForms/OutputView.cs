using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telerik.WinControls.UI.Docking;
using UltraBend.Common;
using UltraBend.Properties;
using UltraBend.Views;
using PostSharp.Patterns.Model;

namespace UltraBend.ViewForms
{
    public partial class OutputView : UserControl, IOutputView
    {
        protected Dictionary<string, DockWindow> DockWindows = new Dictionary<string, DockWindow>();
        protected Dictionary<string, Stream> Streams = new Dictionary<string, Stream>();

        public OutputView()
        {
            Dock = DockStyle.Fill;
            this.Load+=OnLoad;
            InitializeComponent();
        }

        private void OnLoad(object sender, EventArgs eventArgs)
        {
            OnViewLoading();
        }

        public DialogResult ShowDialog()
        {
            throw new NotImplementedException();
        }

        [WeakEvent]
        public event EventHandler ViewLoading;

        public Stream AddChannel(string channel)
        {
            if (!DockWindows.ContainsKey(channel))
            {
                var newDockWindow = new DocumentWindow(channel);
                newDockWindow.ParentChanged += (sender, args) =>
                {
                    if (newDockWindow.FloatingParent != null)
                        newDockWindow.FloatingParent.Icon = Resources.UltraBend_16px1;
                };
                var newTextBox = new TextBox()
                {
                    Multiline = true,
                    ScrollBars = ScrollBars.Vertical,
                    Dock = DockStyle.Fill,
                    ReadOnly = true,
                    Font = new Font(new FontFamily("Consolas"), 10),
                    MaxLength = 2147483646
                };
                newTextBox.VisibleChanged += (sender, args) =>
                {
                    newTextBox.SelectionStart = newTextBox.TextLength;
                    newTextBox.ScrollToCaret();
                };
                var stream = new TextBoxStream(newTextBox);
                newTextBox.Disposed += (sender, args) =>
                {
                    DockWindows.Remove(channel);
                    Streams.Remove(channel);
                };

                newDockWindow.Controls.Add(newTextBox);
                radDock1.AddDocument(newDockWindow);
                newDockWindow.Select();
                return stream;
            }

            if (!Streams.ContainsKey(channel))
            {
                if (!(DockWindows[channel] is DocumentWindow dockWindow))
                    throw new Exception("Unexpected type on DockWindow instance");
                
                DockWindows[channel].ParentChanged += (sender, args) =>
                {
                    if (DockWindows[channel].FloatingParent != null)
                        DockWindows[channel].FloatingParent.Icon = Resources.UltraBend_16px1;
                };

                var newTextBox = new TextBox()
                {
                    Multiline = true,
                    ScrollBars = ScrollBars.Vertical,
                    Dock = DockStyle.Fill,
                    ReadOnly = true,
                    Font = new Font(new FontFamily("Consolas"), 10)
                };
                newTextBox.VisibleChanged += (sender, args) =>
                {
                    newTextBox.SelectionStart = newTextBox.TextLength;
                    newTextBox.ScrollToCaret();
                };
                var stream = new TextBoxStream(newTextBox);
                newTextBox.Disposed += (sender, args) =>
                {
                    DockWindows.Remove(channel);
                    Streams.Remove(channel);
                };

                dockWindow.Controls.Add(newTextBox);
                return stream;
            }

            return Streams[channel];
        }

        protected virtual void OnViewLoading()
        {
            ViewLoading?.Invoke(this, EventArgs.Empty);
        }

        public void RemoveChannel(string channel)
        {
            if (Streams.ContainsKey(channel))
                Streams.Remove(channel);

            if (DockWindows.ContainsKey(channel))
            {
                radDock1.RemoveWindow(DockWindows[channel]);
                DockWindows.Remove(channel);
            }
        }

        public void SetVisible(bool visible)
        {
            throw new NotImplementedException();
        }

        public bool GetVisible()
        {
            throw new NotImplementedException();
        }
    }
}
