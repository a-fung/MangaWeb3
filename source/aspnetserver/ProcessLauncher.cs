using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;

namespace afung.MangaWeb3.Server
{
    public class ProcessLauncher
    {
        private const int MaxNotRespondingCount = 120; // 2 minutes

        public static bool Run(string programPath, string argument, out string output, out int exitCode)
        {
            exitCode = -1;
            output = "";

            StringBuilder outputSB = new StringBuilder();
            Console.WriteLine("Starting process {0} {1}", programPath, argument);
            ProcessStartInfo info = new ProcessStartInfo(programPath, argument);
            info.WindowStyle = ProcessWindowStyle.Minimized;
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;

            bool error = false;
            long peakPagedMem = 0;
            long peakWorkingSet = 0;
            long peakVirtualMem = 0;
            int notRespondingCount = 0;

            using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
            {
                using (Process process = Process.Start(info))
                {
                    process.PriorityClass = ProcessPriorityClass.BelowNormal;
                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (e.Data == null)
                        {
                            outputWaitHandle.Set();
                        }
                        else
                        {
                            outputSB.AppendLine(e.Data);
                        }
                    };
                    process.BeginOutputReadLine();

                    // Check if the process hangs
                    do
                    {
                        if (!process.HasExited)
                        {
                            // Refresh the current process property values.
                            process.Refresh();

                            // Update the values for the overall peak memory statistics.
                            peakPagedMem = process.PeakPagedMemorySize64;
                            peakVirtualMem = process.PeakVirtualMemorySize64;
                            peakWorkingSet = process.PeakWorkingSet64;

                            if (process.Responding)
                            {
                                notRespondingCount = 0;
                            }
                            else
                            {
                                notRespondingCount++;
                                Console.WriteLine("Process {0} is not responding for {1} second(s)", process.ToString(), notRespondingCount);

                                if (notRespondingCount > MaxNotRespondingCount)
                                {
                                    Console.WriteLine("Process is not responding for too long time.");
                                    error = true;
                                    break;
                                }
                            }
                        }
                    }
                    while (!(process.WaitForExit(1000) && outputWaitHandle.WaitOne(1000)));

                    if (notRespondingCount > MaxNotRespondingCount)
                    {
                        do
                        {
                            try
                            {
                                Console.WriteLine("Try killing process {0}...", process.ToString());
                                process.Kill();
                            }
                            catch (InvalidOperationException)
                            {
                                Console.WriteLine("InvalidOperationException");
                            }
                            catch (System.ComponentModel.Win32Exception)
                            {
                                Console.WriteLine("Win32Exception");
                            }
                        }
                        while (!process.WaitForExit(1000));
                    }

                    Console.WriteLine("Process exit code: {0}", exitCode = process.ExitCode);
                    if (process.ExitCode != 0)
                    {
                        Console.WriteLine("Process exited with exit code: {0}", process.ExitCode);
                        error = true;
                    }
                    else
                    {
                        output = outputSB.ToString();
                    }

                    // Display peak memory statistics for the process.
                    Console.WriteLine("Peak physical memory usage of the process: {0}", peakWorkingSet);
                    Console.WriteLine("Peak paged memory usage of the process: {0}", peakPagedMem);
                    Console.WriteLine("Peak virtual memory usage of the process: {0}", peakVirtualMem);
                    process.Close();
                }
            }

            return !error;
        }
    }
}