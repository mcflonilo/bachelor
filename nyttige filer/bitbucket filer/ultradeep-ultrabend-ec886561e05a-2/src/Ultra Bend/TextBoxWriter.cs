using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telerik.WinControls.UI;

namespace Ultra_Bend
{
    public class TextBoxWriter : TextWriter
    {
        private RadTextBox _control;
        protected ConcurrentQueue<string> builder { get; set; } = new ConcurrentQueue<string>();
        protected StringBuilder stringBuilder = new StringBuilder();
        protected Timer timer = new Timer();
        protected Timer timer2 = new Timer();

        public TextBoxWriter(RadTextBox control)
        {
            _control = control;
            _control.VisibleChanged += _control_VisibleChanged;

            timer.Interval = 1;
            timer.Tick += (sender, args) =>
            {
                if (builder.Count > 0)
                {
                    while (builder.Count > 0)
                    {
                        string line;
                        if (builder.TryDequeue(out line))
                            stringBuilder.Append(line);
                        //this.textBox.AppendText(line);
                    }
                }

                timer.Stop();
            };

            timer2.Interval = 100;
            timer2.Tick += (sender, EventArgs) =>
            {
                if (stringBuilder.Length > 0)
                {
                    this._control.AppendText(stringBuilder.ToString());
                    stringBuilder.Clear();
                }
            };
            timer2.Start();


        }

        private void _control_VisibleChanged(object sender, EventArgs e)
        {
            _control.SelectionStart = _control.TextLength;
            _control.ScrollToCaret();
        }

        public override Encoding Encoding => Encoding.Default;

        public override void Write(char value)
        {
            builder.Enqueue(value.ToString());
            _control.Invoke(new MethodInvoker(delegate ()
            {
                timer.Start();
            }));
            //_control.Invoke(new MethodInvoker(delegate () {
            //    _control.AppendText(value.ToString());
            //    //_control.Text += value;
            //    _control.Update();
            //}));
        }
        public override void Write(string value)
        {
            builder.Enqueue(value);
            _control.Invoke(new MethodInvoker(delegate ()
            {
                timer.Start();
            }));
            //_control.Invoke(new MethodInvoker(delegate () {
            //    _control.AppendText(value);
            //    _control.Update();
            //}));
        }
        public override Task WriteAsync(char value)
        {
            builder.Enqueue(value.ToString());
            _control.Invoke(new MethodInvoker(delegate ()
            {
                timer.Start();
            }));
            //_control.Invoke(new MethodInvoker(delegate () {
            //    //_control.Text += value;
            //    _control.AppendText(value.ToString());
            //    _control.Update();
            //}));
            return Task.CompletedTask;
        }
        public override Task WriteAsync(string value)
        {
            builder.Enqueue(value.ToString());
            _control.Invoke(new MethodInvoker(delegate ()
            {
                timer.Start();
            }));
            //_control.Invoke(new MethodInvoker(delegate () {
            //    _control.AppendText(value);
            //    //_control.Update();
            //}));
            return Task.CompletedTask;
        }
    }
}
