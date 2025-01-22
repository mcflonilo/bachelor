using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ultra_Bend.BSEngine
{
    public class OutputsService
    {
        private ConcurrentBag<TextWriter> TextWriters { get; set; } = new ConcurrentBag<TextWriter>();
        private bool WriteToConsoleAsWell { get; }

        public async Task WriteLineAsync(string line = "")
        {
            var toRemove = new List<TextWriter>();

            if (WriteToConsoleAsWell)
                await Console.Out.WriteLineAsync(line);

            foreach (var writer in TextWriters)
            {
                try
                {
                    await writer.WriteLineAsync(line);
                    await writer.FlushAsync();
                }
                catch (ObjectDisposedException)
                {
                    toRemove.Add(writer);
                }
            }

            if (toRemove.Any())
            {
                var currentList = TextWriters.ToList();
                TextWriters = new ConcurrentBag<TextWriter>();
                foreach (var writer in currentList)
                    TextWriters.Add(writer);
            }
        }

        public void AddTextWriter(TextWriter writer)
        {
            TextWriters.Add(TextWriter.Synchronized(writer));
        }

        public OutputsService(bool writeToConsoleAsWell = true)
        {
            WriteToConsoleAsWell = writeToConsoleAsWell;
        }
    }
}
