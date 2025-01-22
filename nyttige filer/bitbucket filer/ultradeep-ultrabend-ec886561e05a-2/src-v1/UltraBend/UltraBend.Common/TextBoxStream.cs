using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace UltraBend.Common
{
    public class TextBoxStream : Stream
    {
        protected TextBox textBox { get; set; }
        protected ConcurrentQueue<string> builder { get; set; } = new ConcurrentQueue<string>();
        protected StringBuilder stringBuilder = new StringBuilder();

        protected Timer timer = new Timer();
        protected Timer timer2 = new Timer();

        public TextBoxStream(TextBox textBox)
        {
            this.textBox = textBox;
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
                    this.textBox.AppendText(stringBuilder.ToString());
                    stringBuilder.Clear();
                }
            };
            timer2.Start();
        }

        public override void Flush()
        {
            // do nothing
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            var text = System.Text.Encoding.Default.GetString(buffer, offset, count);
            builder.Enqueue(text);
            //builder.CompleteAdding();
            timer.Start();
        }

        public override bool CanRead { get => false; }
        public override bool CanSeek { get => false; }
        public override bool CanWrite { get => !this.textBox.IsDisposed; }
        public override long Length { get => textBox.Text.Length; }
        public override long Position { get; set; }
    }
}
