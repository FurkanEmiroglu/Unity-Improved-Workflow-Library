using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using static UnityEngine.Debug;

namespace ImprovedWorkflow.UtilClasses
{
    public static class TerminalUtilities
    {
        // Bash
        private const string linux_bash_dir = "/bin/sh";
        private const string windows_bash_dir = "C:\\Program Files\\Git\\bin\\bash.exe";

        public static void RunCommand(string command)
        {
            RunCommand(command, out _, out _);
        }

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