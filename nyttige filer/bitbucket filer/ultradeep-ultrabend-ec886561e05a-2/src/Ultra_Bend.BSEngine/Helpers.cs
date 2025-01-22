using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Ultra_Bend.BSEngine
{
    public static class Helpers
    {
        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public static string GetHashSha256(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);

            byte[] hash = new SHA256Managed().ComputeHash(bytes);

            var sb = new StringBuilder();

            foreach (byte x in hash)
            {
                sb.Append($"{x:x2}");
            }
            return sb.ToString();
        }

        public static string GetHashMd5(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);

            byte[] hash = new MD5Cng().ComputeHash(bytes);

            var sb = new StringBuilder();

            foreach (byte x in hash)
            {
                sb.Append($"{x:x2}");
            }
            return sb.ToString();
        }

        public static Task RunProcessAsync(string fileName, string workingDirectory, string arguments, OutputsService output, bool printOutput = false)
        {
            if (printOutput)
            {
                Task.Run(async () =>
                {
                    await output.WriteLineAsync($"File name: {fileName}");
                    await output.WriteLineAsync($"Working directory: {workingDirectory}");
                    await output.WriteLineAsync($"Arguments: {arguments}");
                });
            }

            // there is no non-generic TaskCompletionSource
            var tcs = new TaskCompletionSource<bool>();

            var process = new Process
            {
                StartInfo =
                {
                    FileName = fileName,
                    WorkingDirectory = workingDirectory,
                    Arguments = arguments,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                },
                EnableRaisingEvents = true
            };

            process.Exited += async (sender, args) =>
            {
                if (printOutput)
                {
                    var standardError = await process.StandardError.ReadToEndAsync();
                    var standardOutput = await process.StandardOutput.ReadToEndAsync();

                    await output.WriteLineAsync($"{standardError}");
                    await output.WriteLineAsync($"{standardOutput}");
                }

                tcs.SetResult(true);
                process.Dispose();
            };

            process.Start();

            return tcs.Task;
        }

        const int MAX_PATH = 80;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern int GetShortPathName(
            [MarshalAs(UnmanagedType.LPTStr)]
            string path,
            [MarshalAs(UnmanagedType.LPTStr)]
            StringBuilder shortPath,
            int shortPathLength
        );

        public static string GetShortPath(string path)
        {
            var shortPath = new StringBuilder(MAX_PATH);
            GetShortPathName(path, shortPath, MAX_PATH);
            return shortPath.ToString();
        }

        public static Func<A, R> ThreadsafeMemoize<A, R>(this Func<A, R> f)
        {
            var cache = new ConcurrentDictionary<A, R>();

            return argument => cache.GetOrAdd(argument, f);
        }
    }
}
