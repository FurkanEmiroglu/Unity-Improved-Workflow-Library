using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using static UnityEngine.Debug;

namespace ImprovedWorkflow.UtilClasses
{
    public static class TerminalUtilities
    {
        /// <summary>
        /// Bash direction for linux.
        /// </summary>
        private const string linux_bash_dir = "/bin/sh";
        
        /// <summary>
        /// Git bash direction for windows
        /// </summary>
        private const string windows_bash_dir = "C:\\Program Files\\Git\\bin\\bash.exe";

        /// <summary>
        /// Runs a command in the git bash terminal in windows or bash terminal in linux.
        /// </summary>
        /// <param name="command">Command to run</param>
        public static void RunCommand(string command)
        {
            RunCommand(command, out _, out _);
        }

        /// <summary>
        /// Runs a command in the git bash terminal in windows or bash terminal in linux. Returns the output and error.
        /// </summary>
        /// <param name="command">Command to run</param>
        /// <param name="output">Regular output of process if any</param>
        /// <param name="error">Error output of process if any</param>
        public static void RunCommand(string command, out StreamReader output, out StreamReader error)
        {
            output = null;
            error = null;

            OSPlatform? currentPlatform = GetCurrentOs();
            if (currentPlatform == null)
            {
                LogError("Unsupported OS");
                return;
            }

            string currentFileName = currentPlatform == OSPlatform.Linux ? linux_bash_dir : windows_bash_dir;

            Process process = new();
            process.StartInfo.FileName = currentFileName;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            process.StandardInput.WriteLine(command);
            process.StandardInput.Flush();
            process.StandardInput.Close();
            process.WaitForExit();

            output = process.StandardOutput;
            error = process.StandardError;
        }

        /// <summary>
        /// Gets the current OS.
        /// </summary>
        /// <returns>Current operating system</returns>
        private static OSPlatform? GetCurrentOs()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return OSPlatform.Linux;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return OSPlatform.Windows;

            return null;
        }
    }
}