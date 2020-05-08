using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace _cleanup
{
    /// <remarks>
    /// ReSharper disable AssignNullToNotNullAttribute
    /// ReSharper disable PossibleNullReferenceException
    /// ReSharper disable EmptyGeneralCatchClause
    /// </remarks>>
    static class Program
    {
        private const string _GSDATA_ = "_gsdata_";
        private const string _CLEANUP = "_cleanup.xml";
        private const string COMMAND = "command";
        private const string JOB = "job";
        private const string PATTERN = "pattern";
        private const string REMAINS = "remains";

        /// <summary>
        /// Writes logs to _gsdata_/_cleanup.log.
        /// </summary>
        private static StreamWriter _writer;

        /// <summary>
        /// Parsed configuration in _gsdata_/_cleanup.xml.
        /// </summary>
        private static XElement _cleanup;

        /// <summary>
        /// GoodSync directory.
        /// </summary>
        private static string _target;

        /// <summary>
        /// GoodSync service directory.
        /// </summary>
        private static string _gsdata_;

        /// <summary>
        /// Path to log file
        /// </summary>
        private static string _log;

        private static int TryParse(this string s)
        {
            int i; int.TryParse(s, out i); return i;
        }

        private static int? Parse(this string s)
        {
            return string.IsNullOrEmpty(s) ? (int?)null : s.TryParse();
        }

        /// <summary>
        /// Config file entry: command/job.
        /// </summary>
        private class Entry
        {
            /// <summary>
            /// Command text or log file name pattern.
            /// </summary>
            public string Pattern { get; private set; }

            /// <summary>
            /// Number of log files to keep. Ignored for commands.
            /// </summary>
            public int? Remains { get; private set; }

            public Entry(string pattern, string remains)
            {
                Pattern = pattern;
                Remains = remains.Parse();
            }

            public override string ToString() { return $"{Pattern} ({Remains.GetValueOrDefault()})"; }
        }

        /// <summary>
        /// Reads config file section and converts elements to Entries.
        /// </summary>
        /// <param name="x">Section name base.</param>
        private static Entry[] EnumerateSection(string x)
        {
            var entries = new List<Entry>();
            var section = x + "s";

            var root = _cleanup.Element(section);

            if (null != root)
            {
                var nodes = root.Elements(x).ToArray();

                var attributes = new[] { PATTERN, REMAINS, };

                foreach (var node in nodes)
                {
                    var map = attributes
                        .Select(a => new { name = a, remains = node.Attribute(a)?.Value })
                        .ToDictionary(a => a.name, a => a.remains);

                    var pattern = map[PATTERN];
                    var remains = map[REMAINS];

                    var entry = new Entry(pattern, remains);

                    entries.Add(entry);
                }
            }

            WriteLine($"Section {section}: {entries.Count} entries");

            return entries.ToArray();
        }

        /// <summary>
        /// Executes method in try/catch block, reporting an exception, if any.
        /// </summary>
        private static void Guarded<T>(Action<T> method, T value)
        {
            try { method(value); }
            catch (Exception x)
            {
                WriteLine(x.Message);
                WriteLine(x.StackTrace);
            }
        }

        /// <summary>
        /// Cleans left or right directory.
        /// </summary>
        /// <param name="target">Directory path.</param>
        private static void CleanupTarget(string target)
        {
            _target = Path.GetFullPath(target);
            _gsdata_ = Path.Combine(_target, _GSDATA_);

            Environment.CurrentDirectory = _target;

            //
            //    Existence of _target and _gsdata_ are pre-conditions
            //
            if (!(new[] {_target, _gsdata_}).All(Directory.Exists))
                return;

            _log = Path.Combine(_gsdata_, "_cleanup.log");

            using (_writer = new StreamWriter(_log))
            {
                WriteLine($"Cleanup of {_target} started");
                WriteLine($"Gsdata in {_gsdata_}");

                var xml = Path.Combine(_gsdata_, _CLEANUP);

                if (File.Exists(xml))
                {
                    WriteLine($"Config in {xml}");

                    Guarded(e => { _cleanup = XElement.Parse(File.ReadAllText(e)); }, xml);

                    if (null != _cleanup)
                    {
                        WriteLine("Configuration parsed OK");

                        foreach (var command in EnumerateSection(COMMAND)) 
                        {
                            Guarded(RunCommand, command);
                        }

                        foreach (var job in EnumerateSection(JOB))
                        {
                            Guarded(CleanupJob, job);
                        }
                    }

                    WriteLine($"Cleanup of {_target} finished");
                }
                else
                {
                    WriteLine($"No xml file at {xml}");
                }
            }
        }

        /// <summary>
        /// Deletes GoodSync log files for a job.
        /// </summary>
        private static void CleanupJob(Entry job)
        {
            var vall = Directory.EnumerateFiles(_gsdata_).ToArray();
            var all = vall.Where(l => l.EndsWith(job.Pattern)).OrderBy(l => l).ToArray();

            var cut = all.Length - job.Remains.GetValueOrDefault();
            var candidates = all.Take(cut).ToArray();

            foreach (var candidate in candidates)
            {
                File.Delete(candidate);
            }

            WriteLine($"Job {job}: deleted {candidates.Length} of all {all.Length}");
        }

        /// <summary>
        /// Runs a command in target directory.
        /// </summary>
        private static void RunCommand(Entry command)
        {
            //
            //    Options to run command without console window
            //
            var pinfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                FileName = "cmd.exe",
                Arguments = "/c " + command.Pattern,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            var process = Process.Start(pinfo);

            using (StreamReader reader = process.StandardOutput)
            {
                string outline = null;
                while (null != (outline = reader.ReadLine()))
                {
                    WriteLine(outline);
                }
            }
            //
            //    Wait for exit to report exit code
            //
            process.WaitForExit();

            WriteLine($"Command {command.Pattern} exit code {process.ExitCode}");
        }

        /// <summary>
        /// Writes message prefixed with current date/time.
        /// </summary>
        private static void WriteLine(string message)
        {
            if (null == _writer)
                return;

            var snow = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
            var output = $"{snow} {message}";

            _writer.WriteLine(output);
        }

        /// <summary>
        /// Cleanup GoodSync logs and conflicted files.
        /// </summary>
        [STAThread]
        static void Main(string [] targets)
        {
            foreach (var target in targets)
            {
                CleanupTarget(target);
            }
        } 
    }
}

