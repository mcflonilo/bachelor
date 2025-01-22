using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UltraBend.Common;
using System.Windows.Forms;

namespace UltraBend.Services
{
    public class OutputsService
    {
        public event EventHandler<string> ChannelAdded;
        public event EventHandler<string> ChannelRemoved;

        protected Dictionary<string, List<Stream>> Streams { get; set; } = new Dictionary<string, List<Stream>>();

        public void SubscribeToChannel(string channel, ref Stream stream)
        {
            AddChannel(channel);

            if (Streams[channel].Contains(stream) == false)
                Streams[channel].Add(stream);
        }

        public void AddChannel(string channel)
        {
            if (!Streams.ContainsKey(channel))
            {
                Streams.Add(channel, new List<Stream>());
                ChannelAdded?.Invoke(this, channel);
            }
        }

        public void RemoveChannel(string channel)
        {
            if (Streams.ContainsKey(channel))
            {
                Streams.Remove(channel);
                ChannelRemoved?.Invoke(this, channel);
            }
        }

        private void PublishToChannel(string channel, string message)
        {
            // prevent killing the stream with a null terminator. 
            message = message.Replace("\0", string.Empty);

            AddChannel(channel);
            
            if (Streams.ContainsKey(channel) && Streams[channel] != null)
            {
                var streamsToRemove = new List<Stream>();
                foreach (var stream in Streams[channel])
                {
                    if (stream != null && stream.CanWrite)
                    {
                        using (var writer = new StreamWriter(stream))
                        {
                            writer.Write(message);
                            
                            // note this trace costs 100x writer.Write to execute.
                            //System.Diagnostics.Trace.WriteLine(message);
                        }
                    }
                    else
                    {
                        streamsToRemove.Add(stream);
                    }
                }

                foreach (var stream in streamsToRemove)
                    Streams[channel].Remove(stream);

                if (Streams[channel].Count == 0)
                    RemoveChannel(channel);
            }

            Application.DoEvents();
        }

        public void PublishLineToChannel(string channel, string message)
        {
            PublishToChannel(channel, message + Environment.NewLine);
        }

        public List<string> GetChannels()
        {
            return Streams.Keys.Select(k => k).ToList();
        }

    }
}
